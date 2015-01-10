from __future__ import print_function

from board import Board
from random import Random
from specimen import Specimen
from player import Player

NUMBER_OF_GAMES = 20

NUMBER_OF_BOARDS = 20
BOARD_WIDTH = 200
BOARD_HEIGHT = 10**6

INITIAL_SPECIMENS = 50

DNA_LENGTH = 50

SEED = 13722829

if __name__ == "__main__":
    random = Random(SEED)
    player = Player()
    for __ in xrange(NUMBER_OF_GAMES):
        for __ in xrange(NUMBER_OF_BOARDS):
            board = Board(random.randrange(0, 100000000))
            for __ in xrange(INITIAL_SPECIMENS):
                board.specimens[(random.randrange(0, BOARD_WIDTH), 0)] \
                    = Specimen(player, random.getrandbits(DNA_LENGTH))

