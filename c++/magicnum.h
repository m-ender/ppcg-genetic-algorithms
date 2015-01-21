//Game constants


const int 
    DNA_BITS = 100,
    GRID_X = 50,
    GRID_Y = 15,
    VIEW_DIST = 2,
    TELE_DIST = 4,
    TRAP_DIST = 1, //changing this would not 100% work
    MAX_LIFE = 100,
    N_INITIAL_BOTS = 15,
    MIN_START_CELLS = 10,
    SPAWN_PERIOD = 1,
    SPAWN_AMOUNT = 10,
    N_COLORS_SAFE = 8,
    N_COLORS_TELE = 4,
    N_COLORS_TRAP = 2,
    N_COLORS_WALL = 2,
    GAME_DURATION = 10*1000,
    N_GAMES = 50;
const double     
    PROB_MUTATION = 0.01,
    PROB_CROSSOVER = 0.05;


//Fixed seed?

// #define RANDOM_SEED <number>

//Multithreading

#define USE_MULTITHREADING 0
const int N_THREADS = 4;
    

//Logging 
    
const int N_TURNS_PRINTINFO = 2500;
const bool PRINT_SQUARE_INFO = true,
           PRINT_GRID = false,
           PRINT_X = false;
std::ostream &slog = std::clog;

