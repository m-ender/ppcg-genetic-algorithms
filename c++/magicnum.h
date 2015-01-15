const int 
    DNA_BITS = 50,
    PROB_MUTATION = 0.01,
    PROB_CROSSOVER = 0.01,
    GRID_X = 200,
    GRID_Y = 50,
    VIEW_DIST = 2,
    TELE_DIST = 4,
    TRAP_DIST = 1, //changing this would not 100% work
    MAX_LIFE = 500,
    N_INITIAL_BOTS = 50,
    SPAWN_PERIOD = 1,
    N_COLORS_SAFE = 4,
    N_COLORS_TELE = 4,
    N_COLORS_TRAP = 0,
    N_COLORS_WALL = 0,
    GAME_DURATION =3* 100*1000,
    N_GAMES = 20;
 
std::ostream &slog = std::clog;