package game;

import game.traps.DeathTrap;
import game.traps.EmptyTrap;
import game.traps.TeleportationTrap;
import game.traps.WallTrap;

import java.awt.*;
import java.util.*;
import java.util.List;

public class ColorCode {
    public static final int NUM_COLORS = EmptyTrap.NUMBER_OF_EMPTY_TRAPS + WallTrap.NUMBER_OF_WALL_TRAPS +
            TeleportationTrap.NUMBER_OF_TELEPORTATION_TRAPS + DeathTrap.NUMBER_OF_DEATH_TRAPS;
    public static final List<ColorCode> ALL_COLOR_CODES = new ArrayList<ColorCode>(NUM_COLORS);
    static {
        for (int i = 0; i < NUM_COLORS; i++) {
            ALL_COLOR_CODES.add(new ColorCode(i));
        }
        Collections.shuffle(ALL_COLOR_CODES);
    }
    public static final ListIterator<ColorCode> colorAssigner = ALL_COLOR_CODES.listIterator();
    public final Color color;
    public final int number;
    public ColorCode(int number){
        this.number = number;
        color = Color.getHSBColor(Game.random.nextFloat(), Game.random.nextFloat(), Game.random.nextFloat());
    }
}
