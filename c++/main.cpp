#include "./gamelogic.cpp"


//moves in a random forward direction
coord_t randomplayer(dna_t d, view_t v) { 
    return {1, v.rng.rint(-1, 1)};
}

//takes (len) bits of DNA as a binary number
int dnarange(dna_t &d, int start, int len) {
    int res = 0;
    for(int i = start; i < start+len; i++) {
        res = (res << 1) | d[i];
    }
    return res;
}

/* linear combination player:
  makes some number from the sum of
  color indices multiplied by pieces of
  the genome, and indexing into a list
  of moves that aren't out of bounds */
coord_t lcplayer(dna_t d, view_t v) {
    const int chunk = DNA_BITS / N_COLORS;
    int sum = 0;

    for(int i = 0; i < 25; i++) {
        sum += dnarange(d, i*chunk, chunk) * v(i%5-2, i/5-2);
    }
    int nok = 0, ok[3];
    for(int i = 0; i < 3; i++)
        if(v(1, i-1) != OUT_OF_BOUNDS)
            ok[nok++] = i;
    sum += nok << 10; //because % operator isn't mod
    int choice = ok[sum % nok];
    return {1, choice%3-1};
}




coord_t colorScorePlayer(dna_t d, view_t v) {
    const int chunklen = DNA_BITS / N_COLORS;
    int ymax[3], nmax, smax = -1;
    for(int y = -1; y <= 1; y++) {
        if(v(1, y) == OUT_OF_BOUNDS) continue;
        int score = dnarange(d, v(1, y)*chunklen, chunklen);
        if(score > smax) {
            smax = score;
            nmax = 0;
        }
        if(score == smax) ymax[nmax++] = y;
    }
    return {1, ymax[v.rng.rint(nmax)]};
}

int main() {
    slog << "Geometric mean score: " << runsimulation(colorScorePlayer);
}