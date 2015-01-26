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

#if USE_MULTITHREADING
#include <sstream>
#include <atomic>
#if defined (_MSC_VER)
#define thread_local __declspec( thread )
#elif defined (__GCC__)
#define thread_local __thread
#endif

class logger {

public:
    logger(std::ostream & out) : out(out) {}
    ~logger() { flush(); }
    struct dummy {};
    void init(void) const { if (line == nullptr) line = new std::ostringstream; }
    template <typename T> logger& operator << (const T & data)   { init(); *line << data; return *this; }

    logger& operator << (const dummy & data) { init(); *line << std::endl;  flush(); return *this;}
    static dummy endl;

private:
    std::ostream & out;
    static thread_local std::ostringstream * line;
    void flush(void) { if (line == nullptr) return; out << line->str(); line->str(""); }
};

thread_local std::ostringstream * logger::line = nullptr;
logger::dummy logger::endl;

logger slog (std::cout);
logger::dummy slog_flush = logger::endl;

#else // single thread
std::ostream &slog = std::clog;
#define slog_flush std::endl
#endif // USE_MULTITHREADING

