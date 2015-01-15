#include "./gamelogic.cpp"


coord_t nullplayer(dna_t d, view_t v) {
    return {};
}

coord_t randplayer(dna_t d, view_t v) {
    return {1, rand() % 3 - 1};
}

//port of python LinearCombinationPlayer
coord_t lcplayer(dna_t d, view_t v) {
    int sum = 3333333;
    //assumes 50 bits in genome, view range of 2
    for(int i = 0; i < 25; i++) {
        sum += (d[i] + d[i+1]*2) * v(i%5-2, i/5-2);
    }
    return {1, sum % 3 - 1};
}

int main() {
    runsimulation(lcplayer);
}