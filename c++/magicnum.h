//Game constants


const int 
    DNA_BITS = 50,
    GRID_X = 50,//200,
    GRID_Y = 15,//50,
    VIEW_DIST = 2,
    TELE_DIST = 4,
    TRAP_DIST = 1, //changing this would not 100% work
    MAX_LIFE = 100,
    N_INITIAL_BOTS = 15,
    SPAWN_PERIOD = 1,
    SPAWN_AMOUNT = 10,
    N_COLORS_SAFE = 10,
    N_COLORS_TELE = 1,
    N_COLORS_TRAP = 1,
    N_COLORS_WALL = 1,
    GAME_DURATION = 10*1000,
    N_GAMES = 10;
const double     
    PROB_MUTATION = 0.01,
    PROB_CROSSOVER = 0.01;


// Logging 
    
const int N_TURNS_PRINTINFO = 1000;
std::ostream &slog = std::clog;



/*

//Game constants

const int 
    DNA_BITS = 50,
    GRID_X = 50,//200,
    GRID_Y = 15,//50,
    VIEW_DIST = 2,
    TELE_DIST = 4,
    TRAP_DIST = 1, //changing this would not 100% work
    MAX_LIFE = 100,
    N_INITIAL_BOTS = 15,
    SPAWN_PERIOD = 1,
    SPAWN_AMOUNT = 10,
    N_COLORS_SAFE = 10,
    N_COLORS_TELE = 1,
    N_COLORS_TRAP = 1,
    N_COLORS_WALL = 1,
    GAME_DURATION = 100*1000,
    N_GAMES = 5;
const double     
    PROB_MUTATION = 0.05,
    PROB_CROSSOVER = 0.1;


// Logging 
    
const int N_TURNS_PRINTINFO = 5000;
std::ostream &slog = std::clog;
*/