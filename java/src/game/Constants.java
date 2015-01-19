package game;

import java.awt.*;
import java.util.Random;

public class Constants {
    public static final int OUT_OF_BOUNDS_COLOR = -1;
    public static final int NUM_SAFE_COLORS = 8;
    public static final int NUM_KILLER_COLORS = 2;
    public static final int NUM_TELEPORTER_COLORS = 4;
    public static final int NUM_WALL_COLORS = 2;
    public static final int NUM_COLORS = NUM_SAFE_COLORS + NUM_KILLER_COLORS + NUM_TELEPORTER_COLORS + NUM_WALL_COLORS;
    public static final int MAX_TELEPORTER_RADIUS = 4;

    public static final int BOARD_WIDTH = 50;
    public static final int BOARD_HEIGHT = 15;
    public static final int UNSAFE_BOARD_WIDTH = BOARD_WIDTH - 1;
    public static final int BOARD_EXTENDED_WIDTH = UNSAFE_BOARD_WIDTH + MAX_TELEPORTER_RADIUS;


    public static final String TITLE = "The Genetic Rat Race";
    public static final int CELL_SCALAR = 8;
    public static final Color EMPTY_COLOR = Color.WHITE;
    public static final Color SPECIMEN_COLOR = Color.BLACK;
    public static final Color DEATH_COLOR = Color.RED;
    public static final Color TELEPORT_COLOR = Color.BLUE;
    public static final Color WALL_COLOR = Color.GRAY;

    public static final int NUMBER_OF_BOARDS = 20;

    public static final int NUMBER_OF_TURNS = 10000;

    public static final int INITIAL_SPECIMENS = 15;
    public static final int SPECIMEN_LIFESPAN = 100;
    public static final int REPRODUCTION_RATE = 10;
    public static final int NUM_PARENTS = 2;

    public static final int GENOME_LENGTH = 100;
    public static final double GENOME_CROSSOVER_RATE = .05;
    public static final double GENOME_MUTATION_RATE = .01;

    public static final int VISION_WIDTH = 5;

    public static final int RANDOM_SEED = 1778884;

    public static final Random random = new Random(RANDOM_SEED);

}
