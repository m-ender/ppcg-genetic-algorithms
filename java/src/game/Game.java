package game;

import game.display.Display;
import game.players.Player;

import static game.Constants.*;

import java.util.*;
import java.awt.*;
import java.util.List;

public class Game {

    public static void main(String[] args){
        run();
    }
    public static void run(){
        Player player = Player.currentPlayer;
        List<Integer> gameRecords = new ArrayList<Integer>(NUMBER_OF_BOARDS);
        for (int boardNumber = 0; boardNumber < NUMBER_OF_BOARDS; boardNumber++){
            int totalPoints = 1;
            System.out.println("Running board #"+(boardNumber+1)+"/"+NUMBER_OF_BOARDS);
            Board board = new Board();
            long startTime = System.nanoTime();
            Display display = new Display(board);
            for (int turnNumber = 0; turnNumber < NUMBER_OF_TURNS; turnNumber++){
                totalPoints += takeTurn(board, turnNumber, player);
                if (!lifeExists(board))
                    break;
                breed(board, turnNumber, REPRODUCTION_RATE);
                board.updateSpecimen();
                display.repaint();
                if (turnNumber % (NUMBER_OF_TURNS/100) == 0){
                    int population = 0;
                    for (Point point: board.getSpecimenLocations()){
                        population += board.getSpecimen(point).size();
                    }
                    System.out.printf("%3d%% ", turnNumber*100/ NUMBER_OF_TURNS);
                    System.out.printf("%5.4f sec ",(System.nanoTime()-startTime)/1000000000.0);
                    System.out.printf("%10d ",totalPoints);
                    System.out.printf("Pop %5d ", population);
                    System.out.printf("Fit ");
                    System.out.printf("Avg %11.3f ", Statistics.getTotalFitness()*1.0/population);
                    System.out.printf("Max %5d ", Statistics.getMaxFitness());
                    System.out.printf("AllTimeMax %5d ", Statistics.getAllTimeMaxFitness());
                    System.out.println();
                }
            }
            for (Point coordinate: board.getSpecimenLocations()){
                if (Board.atFinish(coordinate)){
                    totalPoints += board.getSpecimen(coordinate).size();
                }
            }
            System.out.println("Your bot got "+totalPoints+" points");
            gameRecords.add(totalPoints);
            display.dispose();
        }
        if (NUMBER_OF_BOARDS > 1){
            System.out.println("=========================================");
            System.out.print("Individual scores: ");
            double total = 0;
            for (int score: gameRecords){
                System.out.print(score+", ");
                total += Math.log(score);
            }
            System.out.println("\nYour final score is "+Math.exp(total / gameRecords.size()));
        }

    }
    public static int takeTurn(Board board, int turnNumber, Player player){
        int points = 0;
        for (Point location: board.getSpecimenLocations()){
            if (Board.atFinish(location)){
                for (Specimen specimen: board.getSpecimen(location)){
                    Point newLocation = Utils.pickOne(board.startingSquares);
                    specimen.birthTurn = turnNumber;
                    specimen.bonusFitness += BOARD_WIDTH;
                    board.addSpecimen(specimen, newLocation);
                    points += 1;
                }
                continue;
            }
            for (Specimen specimen: board.getSpecimen(location)){
                if (turnNumber == specimen.birthTurn + SPECIMEN_LIFESPAN){
                    continue;
                }
                Map<Point, Integer> colors = new HashMap<Point, Integer>();
                for (Point offset:Utils.createArea(VISION_WIDTH)){
                    colors.put(offset, board.getSquare(Utils.add(offset, location)).colorCode.number);
                }
                Point direction = player.takeTurn(specimen.genome, colors);
                if (direction.x*direction.x + direction.y*direction.y > 2){
                    throw new RuntimeException("Direction out of bounds");
                }
                Point newLocation = Utils.add(direction, location);
                Square newSquare = board.getSquare(newLocation);
                if (newSquare.isWall) {
                    newSquare = board.getSquare(location);
                    newLocation = location;
                }
                Point teleported = Utils.add(newSquare.teleportsTo, newLocation);
                if (board.getSquare(teleported).kills){
                    continue;
                }
                board.addSpecimen(specimen, teleported);
            }
        }
        return points;
    }
    public static boolean lifeExists(Board board){
        if (board.getSpecimenLocations().size() > NUM_PARENTS)
            return true;
        int population = 0;
        for (Point point: board.getSpecimenLocations()){
            population += board.getSpecimen(point).size();
            if (population >= NUM_PARENTS){
                return true;
            }
        }
        return false;
    }

    public static int scoreSpecimen(Point coordinate, Specimen specimen){
        return coordinate.x + specimen.bonusFitness + 1;
    }

    public static void breed(Board board, int turnNumber, int numberOffspring){
        Statistics.restart();
        for (Point point: board.getSpecimenLocations()){
            for (Specimen specimen: board.getSpecimen(point)){
                int fitness = scoreSpecimen(point, specimen);
                Statistics.updateFitness(fitness);
            }
        }
        for (int i = 0; i < numberOffspring; i++){
            List<Specimen> selectedSpecimen = new ArrayList<Specimen>(NUM_PARENTS);
            long remainingTotal = Statistics.getTotalFitness();
            for (int j = 0; j < NUM_PARENTS; j++){
                long countDown;
                try {
                    countDown = random.nextLong()%remainingTotal;
                } catch (IllegalArgumentException e){
                    System.out.println(Statistics.getTotalFitness());
                    throw e;
                }
                for (Point point: board.getSpecimenLocations()){
                    for (Specimen specimen: board.getSpecimen(point)){
                        if (selectedSpecimen.contains(specimen))
                            continue;
                        int score = scoreSpecimen(point, specimen);
                        countDown -= score;
                        if (countDown <= 0){
                            selectedSpecimen.add(specimen);
                            remainingTotal -= score;
                            break;
                        }
                    }
                    if (countDown <= 0){
                        break;
                    }
                }
            }
            Specimen currentParent = Utils.pickOne(selectedSpecimen);
            StringBuilder newGenome = new StringBuilder();
            for (int j = 0; j < GENOME_LENGTH; j++){
                if (random.nextDouble() < GENOME_CROSSOVER_RATE){
                    currentParent = Utils.pickOne(selectedSpecimen);
                }
                int bit = currentParent.bitAt(j);
                if (random.nextDouble() < GENOME_MUTATION_RATE){
                    bit = -bit+1;
                }
                newGenome.append(bit);
            }
            board.addSpecimen(new Specimen(newGenome.toString(), turnNumber), Utils.pickOne(board.startingSquares));
        }
    }
}
