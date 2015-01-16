#include "./gamelogic.cpp"


coord_t nullplayer(dna_t d, view_t v) {
    return {};
}

coord_t randplayer(dna_t d, view_t v) {
    return {1, rand() % 3 - 1};
}

//port of python LinearCombinationPlayer
coord_t oldlcplayer(dna_t d, view_t v) {
    int sum = 3333333;
    //assumes 50 bits in genome, view range of 2
    for(int i = 0; i < 25; i++) {
        sum += (d[i] + d[i+1]*2) * v(i%5-2, i/5-2);
    }
    return {1, sum % 3 - 1};
}

coord_t lcplayer2(dna_t d, view_t v) {
    int sum = 0;
    //assumes 50 bits in genome, view range of 2
    for(int i = 0; i < 25; i++) {
        sum += (d[i] + d[i+1]*2) * v(i%5-2, i/5-2);
    }
    int nok = 0, ok[9];
    for(int i = 0; i < 9; i++)
        if(v(i%3-1, i/3-1) != OUT_OF_BOUNDS)
            ok[nok++] = i;
    sum += nok << 10; //because % operator isn't mod
    int choice = ok[sum % nok];
    return {choice%3-1, choice/3-1};
}

coord_t fearplayer(dna_t d, view_t v) {
    //assumes at least N_COLORS bits in genome
    //assumes view distance of at least 2
    const int skip = DNA_BITS / N_COLORS;
    //1 in 12 chance of a totally random move
    if(rand() % 12 == 0) {
        return {rand() % 3 - 1, rand() % 3 - 1};
    }
    int fear[3][3] = {};
    fear[1][1] = 1; //bias against sitting in place
    fear[0][0] = fear[0][1] = fear[0][2] = 1; //bias against going backwards
    for(int x = -1; x <= 1; x++) 
      for(int y = -1; y <= 1; y++) 
        for(int dx = -1; dx <= 1; dx++)
          for(int dy = -1; dy <= 1; dy++) {
              color_t c = v(x+dx, y+dy);
              if(c == OUT_OF_BOUNDS) {
                  if(!dx && !dy) fear[x+1][y+1] += 100;
              } else {
                  fear[x+1][y+1] += d[c*skip];
              }
          }
    int minfear = 9999;
    coord_t cmin;
    for(int x = 2; x >= 0; x--) {
        for(int iy = 3, y = rand()%3; iy--; y = (y+1) % 3) {
            if (fear[x][y] < minfear) {
                cmin = {x-1, y-1};
                minfear = fear[x][y];
            }
        }
    }
    return cmin;
}

int main() {
    slog << "Average score: " << runsimulation(lcplayer2);
}