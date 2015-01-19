package game.traps;

import game.ColorCode;
import game.Utils;
import static game.Constants.*;

import java.awt.*;
import java.util.*;
import java.util.List;


public class TeleportationTrap extends Trap{
    public static void initialize() {
        java.util.List<Point> possibleDirections = Utils.createArea(9);
        Collections.shuffle(possibleDirections, random);
        List<Point> directionAssigner = possibleDirections.subList(0, NUM_TELEPORTER_COLORS);
        for (int i = 0; i < NUM_TELEPORTER_COLORS; i+=2) {
            Point direction = Utils.pickOne(directionAssigner);
            Point oppositeDirection = new Point(-1*direction.x, -1*direction.y);
            allTraps.put(ColorCode.colorAssigner.next(), new TeleportationTrap(direction));
            allTraps.put(ColorCode.colorAssigner.next(), new TeleportationTrap(oppositeDirection));
        }
    }
    public final Point direction;
    public TeleportationTrap(Point direction){
        this.direction = direction;
    }
}
