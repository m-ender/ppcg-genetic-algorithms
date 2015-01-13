package game;

import game.display.Display;
import game.players.Player;

import java.util.*;
import java.awt.*;
import java.util.List;

public class Game {
    public static final int NUMBER_OF_BOARDS = 1;
    public static final int NUMBER_OF_TURNS = 10000;
    public static final int INITIAL_SPECIMEN = 50;
    public static final int SPECIMEN_LIFESPAN = 500;
    public static final int REPRODUCTION_RATE = 1;
    public static final int NUM_PARENTS = 2;
    public static final int DNA_LENGTH = 50;
    public static final long MAX_DNA = Math.round(Math.pow(2, DNA_LENGTH - 1));
    public static final long MIN_DNA = -MAX_DNA-1;
    public static final double CROSSOVER_RATE = .1;
    public static final double DNA_MUTATION_RATE = .01;
    public static final int VISION_WIDTH = 5;
    public static final int RANDOM_SEED = 1778884;

    public static final Random random = new Random(RANDOM_SEED);

    public static void main(String[] args){
        run();
    }
    public static void run(){
        Player player = new Player();
        int totalPoints = 0;
        for (int boardNumber = 0; boardNumber < NUMBER_OF_BOARDS; boardNumber++){
            System.out.println("Running board #"+(boardNumber+1)+"/"+NUMBER_OF_BOARDS);
            Board board = new Board();
            Display display = new Display(board);
            int reproductionCounter = 0;
            for (int turnNumber = 0; turnNumber < NUMBER_OF_TURNS; turnNumber++){
                totalPoints += takeTurn(board, turnNumber, player);
                if (!lifeExists(board))
                    break;
                reproductionCounter += REPRODUCTION_RATE;
                for (;reproductionCounter > 0; reproductionCounter--){
                    breed(board, turnNumber);
                }
                display.repaint();
            }
            for (Point coordinate: board.getSpecimenLocations()){
                if (Board.atFinish(coordinate)){
                    totalPoints += board.getSpecimen(coordinate).size();
                }
            }
        }
        System.out.println("Your bot got "+totalPoints+" points");
    }
    public static int takeTurn(Board board, int turnNumber, Player player){
        int points = 0;
        for (Point location: board.getSpecimenLocations()){
            for (Specimen specimen: board.getSpecimen(location)){
                if (turnNumber == specimen.birthTurn + SPECIMEN_LIFESPAN){
                    if (Board.atFinish(location)){
                        points += 1;
                    }
                    continue;
                }
                if (Board.atFinish(location)){
                    board.addSpecimen(specimen, location);
                    continue;
                }
                List<Integer> colors = new ArrayList<Integer>();
                for (Point offset:Utils.createArea(VISION_WIDTH)){
                    colors.add(board.getSquare(Utils.add(offset, location)).colorCode.number);
                }
                Point direction = player.takeTurn(specimen, colors);
                Point newLocation = Utils.add(direction, location);
                Square newSquare = board.getSquare(newLocation);
                if (newSquare.isWall){
                    newSquare = board.getSquare(location);
                    newLocation = location;
                }
                Point teleported = Utils.add(newSquare.teleportsTo, newLocation);
                if (board.getSquare(teleported).kills && !Board.atFinish(teleported)){
                    continue;
                }
                board.addSpecimen(specimen, teleported);
            }
        }
        board.updateSpecimen();
        return points;
    }
    public static boolean lifeExists(Board board){
        return board.getSpecimenLocations().size() > NUM_PARENTS;
    }
    public static void breed(Board board, int turnNumber){
        int total = 0;
        for (Point point: board.getSpecimenLocations()){
            total += (point.x+1)*board.getSpecimen(point).size();
        }
        Map<Point, Integer> selectedCoordinates = new HashMap<Point, Integer>();
        for (int i = 0; i < NUM_PARENTS; i++) {
            int countDown = random.nextInt(total);
            for (Point point : board.getSpecimenLocations()) {
                int alreadySelected = 0;
                if (selectedCoordinates.containsKey(point)) {
                    alreadySelected = selectedCoordinates.get(point);
                }
                countDown -= (point.x + 1) * (board.getSpecimen(point).size() - alreadySelected);
                if (countDown < 0) {
                    selectedCoordinates.put(point, alreadySelected + 1);
                    break;
                }
            }
        }
        ArrayList<Specimen> chosenSpecimen = new ArrayList<Specimen>(NUM_PARENTS);
        for (Point point: selectedCoordinates.keySet()){
            List<Specimen> specimen = board.getSpecimen(point);
            Collections.shuffle(specimen);
            chosenSpecimen.addAll(specimen.subList(0, selectedCoordinates.get(point)));
        }
        Specimen currentParent = Utils.pickOne(chosenSpecimen);
        long newDNA= 0;
        for (int j = DNA_LENGTH-1; j >=0; j--){
            if (random.nextDouble() < CROSSOVER_RATE){
                currentParent = Utils.pickOne(chosenSpecimen);
            }
            int bit = currentParent.bitAt(j);
            if (random.nextDouble() < DNA_MUTATION_RATE){
                bit = -bit+1;
            }
            newDNA = (newDNA+bit) <<2;
        }
        board.addSpecimen(new Specimen(newDNA, turnNumber), Utils.pickOne(board.startingSquares));

    }
}
