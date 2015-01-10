from __future__ import print_function

from board import Board
from random import Random
from specimen import Specimen
from player import Player
from trap import Trap

NUMBER_OF_BOARDS = 20
BOARD_WIDTH = 200
BOARD_HEIGHT = 10**6

TRAP_INITIAL_FREQ = .1
TRAP_FINAL_FREQ = .5

INITIAL_SPECIMENS = 50

DNA_LENGTH = 50
LIFE_SPAN = 200

SEED = 13722829

def initialize_boards():
    board = Board(random.randrange(0, 100000000))
    for __ in xrange(INITIAL_SPECIMENS):
        board.specimens[(random.randrange(0, BOARD_WIDTH), 0)] \
            = Specimen(player, random.getrandbits(DNA_LENGTH))
    for height in xrange(BOARD_HEIGHT):
        trap_frequency = (height / BOARD_HEIGHT) * \
                         (TRAP_FINAL_FREQ - TRAP_INITIAL_FREQ) \
                         + TRAP_INITIAL_FREQ
        for width in xrange(BOARD_WIDTH):
            if random.random() < trap_frequency:
                board.traps[(width, height)] = Trap(board)
    return board


if __name__ == "__main__":
    random = Random(SEED)
    player = Player()
    for __ in xrange(NUMBER_OF_BOARDS):
        boards = initialize_boards()

