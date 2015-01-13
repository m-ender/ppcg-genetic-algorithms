package game.traps;

import game.ColorCode;

public class WallTrap extends Trap{
    public static final int NUMBER_OF_WALL_TRAPS = 4;
    public static void initialize() {
        for (int i = 0; i < NUMBER_OF_WALL_TRAPS; i++) {
            allTraps.put(ColorCode.colorAssigner.next(), new WallTrap());
        }
    }
}
