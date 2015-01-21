package game;

import game.traps.*;

import java.awt.Point;
import java.util.*;
import java.util.List;
import static game.Constants.*;

public class Board {
    private HashMap<Point, List<Specimen>> specimens;
    private HashMap<Point, List<Specimen>> nextSpecimens;
    private List<List<Square>> squares;
    public final List<Point> startingSquares;
    private final Square outOfBoundsSquare;
    private final List<Square> finishLine;



    public Board(){
        specimens = new HashMap<Point, List<Specimen>>();
        nextSpecimens = new HashMap<Point, List<Specimen>>();
        squares = new ArrayList<List<Square>>();
        startingSquares = new ArrayList<Point>();
        outOfBoundsSquare = new Square(new ColorCode(-1));
        finishLine = new ArrayList<Square>();
        Trap.initialize();
        for (ColorCode colorCode: EmptyTrap.EMPTY_TRAPS.keySet()){
            finishLine.add(new Square(colorCode));
        }
        generateSquares();
        ensureCrossable();
        addSpecimens();

    }

    public Set<Point> getSpecimenLocations(){
        return specimens.keySet();
    }

    public List<Specimen> getSpecimen(Point point){
        if (specimens.containsKey(point)){
            return specimens.get(point);
        } else {
            return new ArrayList<Specimen>();
        }
    }

    public boolean hasSpecimen(Point point){
        return specimens.containsKey(point);
    }

    private void addSpecimens(){
        for (int i = 0; i < INITIAL_SPECIMENS; i++){
            StringBuilder builder = new StringBuilder();
            for (int j = 0; j < GENOME_LENGTH; j++){
                builder.append(random.nextBoolean()?"1":"0");
            }
            this.addSpecimen(new Specimen(builder.toString(), 0), Utils.pickOne(startingSquares));
        }
        updateSpecimen();
    }

    private void ensureCrossable(){
        for (int y = 0; y < BOARD_HEIGHT; y++){
            Point startPoint = new Point(0, y);
            HashSet<Point> currentSquares = new HashSet<Point>();
            HashSet<Point> nextSquares = new HashSet<Point>();
            HashSet<Point> visitedSquares = new HashSet<Point>();
            currentSquares.add(startPoint);
            visitedSquares.add(startPoint);
            boolean foundExit = false;
            while (true){
                List<Point> possibleMovements = new ArrayList<Point>();
                for (Point square: currentSquares){
                    for (Point movement: Utils.createArea(3)) {
                        Point neighbor = Utils.add(movement, square);
                        Point teleported = Utils.add(neighbor,getSquare(neighbor).teleportsTo);
                        possibleMovements.add(teleported);
                        if (!getSquare(teleported).kills && !visitedSquares.contains(teleported)){
                            nextSquares.add(teleported);
                            visitedSquares.add(teleported);
                        }
                    }
                }
                for (Point square: possibleMovements){
                    if (atFinish(square) || startingSquares.contains(square)){
                        foundExit = true;
                    }
                }
                if (foundExit){
                    startingSquares.add(startPoint);
                    break;
                }
                if (nextSquares.size() == 0){
                    break;
                }
                currentSquares = nextSquares;
                nextSquares = new HashSet<Point>();
            }
        }
        if (startingSquares.size() < 10){
            startingSquares.clear();
            generateSquares();
            ensureCrossable();
        }
    }

    public static boolean atFinish(Point point){
        return !outOfBounds(point) && point.x >= UNSAFE_BOARD_WIDTH;
    }

    public static boolean outOfBounds(Point point){
        return point.x < 0 || point.y < 0 || point.y >= BOARD_HEIGHT;
    }

    private void generateSquares(){
        for (int y = 0; y < BOARD_HEIGHT; y++){
            List<Square> currentRow = new ArrayList<Square>();
            for (int x = 0; x < BOARD_WIDTH; x++){
                ColorCode randomColorCode = Utils.pickOne(ColorCode.ALL_COLOR_CODES);
                currentRow.add(new Square(randomColorCode));
            }
            squares.add(currentRow);
        }
        for (Point point: Utils.createArea(BOARD_WIDTH, BOARD_HEIGHT)){
            Square square = getSquare(point);
            Trap trap = Trap.allTraps.get(square.colorCode);
            if (trap instanceof DeathTrap){
                getSquare(Utils.add(((DeathTrap) trap).direction, point)).kills = true;
            } else if (trap instanceof TeleportationTrap){
                square.teleportsTo=((TeleportationTrap) trap).direction;
            } else if (trap instanceof WallTrap){
                square.isWall = true;
                square.kills = true;
            }
        }
    }
    public Square getSquare(Point point){
        if (outOfBounds(point)){
            return outOfBoundsSquare;
        } if (atFinish(point)){
            return Utils.pickOne(this.finishLine);
        }
        return squares.get(point.y).get(point.x);
    }
    public void addSpecimen(Specimen specimen, Point location){
        if (nextSpecimens.containsKey(location)){
            nextSpecimens.get(location).add(specimen);
        } else {
            List<Specimen> specimens = new ArrayList<Specimen>();
            specimens.add(specimen);
            nextSpecimens.put(location, specimens);
        }
    }
    public void updateSpecimen(){

        this.specimens = this.nextSpecimens;
        this.nextSpecimens = new HashMap<Point, List<Specimen>>();
    }

}
