#include <vector>
#include <cstdlib>
#include <bitset>
#include <chrono>
#include <iostream>
#include <random>
#include <cstring>

#include "./magicnum.h"

const int N_COLORS = N_COLORS_SAFE + N_COLORS_TELE + 
    N_COLORS_TRAP + N_COLORS_WALL;

class board_t;

typedef int color_t;
const color_t OUT_OF_BOUNDS = -1;

enum colortype_t {
    C_SAFE,
    C_TELE,
    C_TRAP,
    C_WALL
};


typedef std::bitset<DNA_BITS> dna_t;
struct coord_t {
    int x,y;
};
coord_t operator+ (coord_t a, coord_t b) {
    return {a.x + b.x, a.y + b.y};
}

const coord_t DEST_DEATH = {-1}, DEST_GOAL = {-2};


typedef unsigned long long ull;

struct bot_t {
    dna_t dna;
    coord_t position;
    unsigned lifetime;
    unsigned goals;
    
    int fitness() {
        return position.x + goals * GRID_X;
    }
};



const int maxdist = VIEW_DIST > TELE_DIST ? VIEW_DIST: TELE_DIST;

class view_t {
private:
    board_t &board;
    coord_t pos;
public:
    view_t(board_t *b, coord_t p);
    color_t operator() (int x, int y);
};


typedef coord_t(*player_t)(dna_t, view_t);  //defines player_t


class board_t {
    friend class view_t;

    std::vector<bot_t> bots;
    player_t movefunc;
    int spawntimer;
    std::vector<int> spawnY;
    color_t colorgrid[GRID_X + VIEW_DIST][GRID_Y + 1]; //valid x: 1 to GRID_X-1+viewdist; y: 1 to GRID_Y
    coord_t destgrid[GRID_X + 1][GRID_Y + 2]; // valid all
    int totalscore;
    
    std::mt19937_64 rndeng;
    
    void breed() {
        if(bots.size() < 2) return;
        ull sum = 0;
        for(int i = 0; i < bots.size(); i++) {
            sum += bots[i].fitness();
        }
        dna_t parents[2], child;
        long long r = randlong(sum);
        int i = 0;
        while( (r -= bots[i].fitness()) >= 0) i++;
        parents[0] = bots[i].dna;
        sum -= bots[i].fitness();
        r = randlong(sum);
        int i2 = 0;
        while( (r -= bots[i2].fitness() * (i != i2)) >= 0) i++;
        parents[1] = bots[i2].dna;
        
        int par = randint(2);
        for(int j = 0; j < DNA_BITS; j++) {
            child[j] = parents[par][j] ^ (randdouble() < PROB_MUTATION);
            par ^= randdouble() < PROB_CROSSOVER;
        }
        bots.push_back({child, randomspawnloc()});
    }
    
    coord_t randomspawnloc() {
        return {1, spawnY[randint(spawnY.size())]};
    }
    
    void gengrid() {
        memset(colorgrid, -1, sizeof(colorgrid));
        memset(colortypes, -1, sizeof(colortypes));
        color_t safes[N_COLORS_SAFE];
        int remc = N_COLORS;
        #define DISTRIBUTECOLORS(C) \
            for(int i=N_COLORS_##C,k;k=0,i--;colortypes[k]=C_##C) for(int j=randint(remc--);j||~colortypes[k];k++) j-=!~colortypes[k];

        DISTRIBUTECOLORS(SAFE)
        DISTRIBUTECOLORS(TELE)
        DISTRIBUTECOLORS(TRAP)
        DISTRIBUTECOLORS(WALL)
     
        for(int i=0, c=0; i < N_COLORS; i++) if(colortypes[i] == C_SAFE) safes[c++] = i;
        
        for(int x = GRID_X; x < GRID_X + VIEW_DIST; x++)
            for(int y = 1; y <= GRID_Y; y++)
                colorgrid[x][y] = safes[randint(N_COLORS_SAFE)];

        
        for(int x = 0; x <= GRID_X; x++) destgrid[x][0] = DEST_DEATH;
        for(int x = 0; x <= GRID_X; x++) destgrid[x][GRID_Y+1] = DEST_DEATH;
        for(int y = 1; y <= GRID_Y; y++) destgrid[0][y] = DEST_DEATH;

        bool tmp[GRID_X+1][GRID_Y+1];

        do {  //loop until grid is solvable
            for(int x = 1; x < GRID_X; x++)
                for(int y = 1; y <= GRID_Y; y++)
                    colorgrid[x][y] = randint(N_COLORS);

            memset(tmp, 0, sizeof(tmp));
            //determine trap, tele vectors
            coord_t offset[N_COLORS];
            for(int i = 0; i < N_COLORS; i++) {
                if(colortypes[i] == C_TRAP) {
                    offset[i].x = randint(-TRAP_DIST,TRAP_DIST);
                    offset[i].y = randint(-TRAP_DIST,TRAP_DIST);
                } else if(colortypes[i] == C_TELE) { 
                    int w = 2 * TELE_DIST + 1;
                    int r = randint(w*w - 1);  //no null teleporters
                    offset[i].x = r % w - TELE_DIST;
                    offset[i].y = r / w - TELE_DIST;
                    if(!offset[i].x && !offset[i].y) {
                        offset[i] = {TELE_DIST, TELE_DIST};
                    }
                }
            }
            for(int x = 1; x < GRID_X; x++)
                for(int y = 1; y <= GRID_Y; y++) {
                    color_t colr = colorgrid[x][y];
                    switch(colortypes[colr]) {
                      case C_TRAP: {
                        coord_t target = coord_t{x, y} + offset[colr];
                        if(target.x > 0 && target.x <= GRID_X && target.y > 0 && target.y <= GRID_Y) {
                            tmp[target.x][target.y] = true;
                        }
                        break;
                      }
                      case C_WALL:
                        tmp[x][y] = true;
                        break;
                    }
                }
            for(int x = 1; x <= GRID_X; x++) {
                for(int y = 1; y <= GRID_Y; y++) {
                    coord_t end;
                    if(colortypes[colorgrid[x][y]] == C_TELE) {
                        end = coord_t{x, y} + offset[colorgrid[x][y]];
                        if(end.x < 1 || end.y < 1 || end.y > GRID_Y) end = DEST_DEATH;
                    } else {
                        end = {x, y};
                    }
                    
                    if(end.x > GRID_X) {
                        end = DEST_GOAL;
                    } else if(end.x > 0 && tmp[end.x][end.y]) {
                        end = DEST_DEATH;
                    } else if(end.x == GRID_X) {
                        end = DEST_GOAL;
                    }
                    destgrid[x][y] = end;
                }
            }
printgrid(0);printgrid(1);printgrid(2);
        } while(!calcsolvability());

    }
    
    unsigned calcsolvability() {
        for(int y0 = 1; y0 <= GRID_Y; y0++) {
            bool seen[GRID_X][GRID_Y] = {};
            std::vector<coord_t> q;
            seen[1][y0] = true;
            q.push_back({1, y0});
            int i = 0;
            for(int i = 0, t = MAX_LIFE; t--; ) {
                for(int j = q.size(); i < j; i++) {
                    coord_t s = q[i];
                    for(int x = -1; x <= 1; x++)
                        for(int y = -1; y <= 1; y++) {
                            if((x || y) && colortypes[colorgrid[s.x+x][s.y+y]] == C_WALL)
                                continue;
                            coord_t next = destgrid[s.x + x][s.y + y];
                            if(next.x == DEST_GOAL.x) goto win;
                            if(next.x > 0 && !seen[next.x][next.y]) {
                                seen[next.x][next.y] = true;
                                q.push_back(next);
                            }
                        }
                }
            }
            continue;
          win:
            spawnY.push_back(y0);
        }
        slog << spawnY.size() << " starting points\n";
        return spawnY.size();
    }
public:
    colortype_t colortypes[N_COLORS];
    board_t(player_t player, 
           ull rndseed = std::chrono::high_resolution_clock().now().time_since_epoch().count()): 
            rndeng(rndseed), 
            movefunc(player),
            totalscore(0),
            spawntimer(0) {
        
        slog << "Board created with random seed " << rndseed << '\n';
        gengrid();

        for(int i = N_INITIAL_BOTS; i--; ) {
            dna_t dna;
            for(int j = 0; j < DNA_BITS; j++) dna[j] = randint(2);
            bots.emplace_back(bot_t{dna, randomspawnloc()});
       }
    }
        
    
    
    color_t color(int x, int y) {
        if(x < 1 || x >= GRID_X+VIEW_DIST || y < 1 || y > GRID_Y) {
            return OUT_OF_BOUNDS;
        }
        return colorgrid[x][y];
    }
    
    
    
    void runframe() {
        for(int i = 0; i < bots.size(); i++) {
            coord_t move = movefunc(bots[i].dna, view_t(this, bots[i].position));
            if(abs(move.x) > 1 || abs(move.y) > 1) {
                move = {0, 0};
                slog << "Player attempted illegal move\n";
            }
            coord_t newpos = bots[i].position + move;
            if(colortypes[colorgrid[newpos.x][newpos.y]] == C_WALL)
                newpos = bots[i].position;
            coord_t dest = destgrid[newpos.x][newpos.y];
            if(dest.x == DEST_DEATH.x) {
                bots[i] = bots.back();
                bots.pop_back();
                i--;
            } else if(dest.x == DEST_GOAL.x) {
                bots[i].goals++;
                totalscore++;
                bots[i].position = randomspawnloc();
                bots[i].lifetime = 0;
            } else {
                if(++bots[i].lifetime == MAX_LIFE) {
                    bots[i] = bots.back();
                    bots.pop_back();
                    i--;
                } else {
                    bots[i].position = dest;
                }
            }
        }
        if(++spawntimer == SPAWN_PERIOD) {
            spawntimer = 0;
            for(int n = SPAWN_AMOUNT; n--; ) {
                breed();
            }
        }
    }
    
    int score() {
        return totalscore;
    }
    
    int n_alive() {
        return bots.size();
    }
    
    int randint(int n) {
        return std::uniform_int_distribution<>(0, n - 1)(rndeng);
    }
    int randint(int min, int max) {
        return std::uniform_int_distribution<>(min, max)(rndeng);
    }
    ull randlong(ull n) {
        return std::uniform_int_distribution<long long>(0, n - 1)(rndeng);
    }
    double randdouble() {
        return std::uniform_real_distribution<>(0., 1.)(rndeng);
    }
    
    void printgrid(int m) {
        bool m2 = m == 2;
        for(int y = m2?0:1; y <= GRID_Y+m2; y++) {
            for(int x = m2?0:1; x < GRID_X + (m2?1:VIEW_DIST); x++) {
                color_t c;
                colortype_t t;
                if (x>0&&x<GRID_X+VIEW_DIST&&y>0&&y<=GRID_Y) {
                    c = colorgrid[x][y];
                    if(c < 0 || c >= N_COLORS) {
                        slog << "bad color";
                        return;
                    }
                    t = colortypes[c];
                } else {
                    t = C_SAFE;
                }
                if(m == 0)
                    slog << char(c<10?48+c:96+c-9);
                if(m == 1)
                    slog << "STRW"[t];
                if(m == 2)
                    slog << char(t == C_WALL ? 'w' 
                            : destgrid[x][y].x == DEST_DEATH.x ? '!'
                            : destgrid[x][y].x == DEST_GOAL.x ? 'G' 
                            : '.');
            }
            slog << '\n';
       }
    }
    
    void printx() {
        for(bot_t bot:bots) slog<<bot.position.x<<' ';
        slog<<'\n';
    }
};

view_t::view_t(board_t *b, coord_t p):
    pos(p),
    board(*b) {} 

color_t view_t::operator() (int x, int y) {
    if(abs(x) > VIEW_DIST || abs(y) > VIEW_DIST) {
        slog << "Attempted to view square out of range\n";
        return OUT_OF_BOUNDS;
    }
	int X = pos.x + x, Y = pos.y + y;
    if(X < 1 || Y < 1 || Y > GRID_Y)
        return OUT_OF_BOUNDS;
    return board.colorgrid[X][Y];
}

int rungame(player_t player) {
    board_t b(player/*, 1421320494465294900LL*/);
    for(int i = 0; i < GAME_DURATION; i++) {
        if(i % N_TURNS_PRINTINFO == 0) {
            slog << "Turns:" << i << " Specimens: " << b.n_alive() 
                << " Score: " << b.score() << '\n';
b.printx();
        }
        b.runframe();
    }
    return b.score();
}

double runsimulation(player_t player) {
    int sum = 0;
    int scores[N_GAMES];
    for(int i = 0; i < N_GAMES; i++) {
        int sc = rungame(player);
        slog << "Scored " << sc << " in game " << i << '\n';
        sum += sc;
        scores[i] = sc;
    }
    slog << "Scores:";
    for(int i = 0; i < N_GAMES; i++)
        slog << ' ' << scores[i];
    slog<<'\n';
    return sum / (double)N_GAMES;
}


    