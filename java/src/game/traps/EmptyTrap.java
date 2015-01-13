package game.traps;

import game.ColorCode;

import java.util.HashMap;
import java.util.Map;

public class EmptyTrap extends Trap{
    public static final int NUMBER_OF_EMPTY_TRAPS = 4;
    public static final Map<ColorCode, EmptyTrap> EMPTY_TRAPS = new HashMap<ColorCode, EmptyTrap>(NUMBER_OF_EMPTY_TRAPS);
    public static void initialize() {
        EmptyTrap emptyTrap = new EmptyTrap();
        for (int i = 0; i < NUMBER_OF_EMPTY_TRAPS; i++) {
            ColorCode nextColor = ColorCode.colorAssigner.next();
            allTraps.put(nextColor, emptyTrap);
            EMPTY_TRAPS.put(nextColor, emptyTrap);
        }
    }
}
