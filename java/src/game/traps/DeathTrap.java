package game.traps;

import game.ColorCode;
import game.Utils;
import static game.Constants.*;

import java.awt.Point;
import java.util.*;

public class DeathTrap extends Trap{
    public static void initialize() {
        List<Point> possibleDirections = Utils.createArea(3);
        Collections.shuffle(possibleDirections, random);
        List<Point> directionAssigner = possibleDirections.subList(0, NUM_KILLER_COLORS);
        for (int i = 0; i < NUM_KILLER_COLORS; i++) {
            allTraps.put(ColorCode.colorAssigner.next(), new DeathTrap(Utils.pickOne(directionAssigner)));
        }
    }
    public final Point direction;
    public DeathTrap(Point direction){
        this.direction = direction;
    }
}
