package game.traps;

import game.ColorCode;

import java.util.HashMap;
import java.util.Map;

public abstract class Trap {
    public static final Map<ColorCode, Trap> allTraps = new HashMap<ColorCode, Trap>();
    public static void initialize() {
        allTraps.clear();
        ColorCode.initialize();
        DeathTrap.initialize();
        EmptyTrap.initialize();
        TeleportationTrap.initialize();
        WallTrap.initialize();
    }
}
