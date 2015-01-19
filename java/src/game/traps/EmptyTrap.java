package game.traps;

import game.ColorCode;
import static game.Constants.*;

import java.util.HashMap;
import java.util.Map;

public class EmptyTrap extends Trap{
    public static final Map<ColorCode, EmptyTrap> EMPTY_TRAPS = new HashMap<ColorCode, EmptyTrap>(NUM_SAFE_COLORS);
    public static void initialize() {
        EmptyTrap emptyTrap = new EmptyTrap();
        for (int i = 0; i < NUM_SAFE_COLORS; i++) {
            ColorCode nextColor = ColorCode.colorAssigner.next();
            allTraps.put(nextColor, emptyTrap);
            EMPTY_TRAPS.put(nextColor, emptyTrap);
        }
    }
}
