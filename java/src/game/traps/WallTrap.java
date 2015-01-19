package game.traps;

import game.ColorCode;
import static game.Constants.*;

public class WallTrap extends Trap{

    public static void initialize() {
        for (int i = 0; i < NUM_WALL_COLORS; i++) {
            allTraps.put(ColorCode.colorAssigner.next(), new WallTrap());
        }
    }
}
