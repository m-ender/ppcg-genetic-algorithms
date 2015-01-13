package game.traps;

import game.ColorCode;
import game.Game;
import game.Utils;

import java.awt.*;
import java.util.*;

public class TeleportationTrap extends Trap{
    public static final int NUMBER_OF_TELEPORTATION_TRAPS = 4;
    public static void initialize() {
        java.util.List<Point> possibleDirections = Utils.createArea(9);
        Collections.shuffle(possibleDirections, Game.random);
        ListIterator<Point> directionAssigner = possibleDirections.subList(0, NUMBER_OF_TELEPORTATION_TRAPS).listIterator();
        for (int i = 0; i < NUMBER_OF_TELEPORTATION_TRAPS; i++) {
            allTraps.put(ColorCode.colorAssigner.next(), new TeleportationTrap(directionAssigner.next()));
        }
    }
    public final Point direction;
    public TeleportationTrap(Point direction){
        this.direction = direction;
    }
}
