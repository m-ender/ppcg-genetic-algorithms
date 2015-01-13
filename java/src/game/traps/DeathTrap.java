package game.traps;

import game.ColorCode;
import game.Game;
import game.Utils;

import java.awt.Point;
import java.util.*;

public class DeathTrap extends Trap{
    public static final int NUMBER_OF_DEATH_TRAPS = 4;
    public static void initialize() {
        List<Point> possibleDirections = Utils.createArea(3);
        Collections.shuffle(possibleDirections, Game.random);
        ListIterator<Point> directionAssigner = possibleDirections.subList(0, NUMBER_OF_DEATH_TRAPS).listIterator();
        for (int i = 0; i < NUMBER_OF_DEATH_TRAPS; i++) {
            allTraps.put(ColorCode.colorAssigner.next(), new DeathTrap(directionAssigner.next()));
        }
    }
    public final Point direction;
    public DeathTrap(Point direction){
        this.direction = direction;
    }
}
