# All constants in one place for clarity

from __future__ import print_function

BOARD_WIDTH = 50
BOARD_HEIGHT = 15
UNSAFE_BOARD_WIDTH = BOARD_WIDTH - 1
BOARD_EXTENDED_WIDTH = BOARD_WIDTH + 3

OUT_OF_BOUNDS_COLOR = -1
NUMBER_OF_SAFE_COLORS = 10
NUMBER_OF_KILLERS = 3
NUMBER_OF_TELEPORTERS = 3
NUMBER_OF_WALLS = 3
MAX_TELEPORT_RADIUS = 4

from board import Board
from random import Random
from specimen import Specimen
import player as Player
import sys
import trap
import coordinates
import time

if sys.version_info >= (3,):
    xrange = range

#Display related constants:
TITLE = "The Genetic Rat Race"
CELL_SCALAR = 8
EMPTY_COLOR = (255, 255, 255)
SPECIMEN_COLOR = (0, 0, 0)
DEATH_COLOR = (255, 128, 128)
TELEPORT_COLOR = (128, 128, 255)
WALL_COLOR = (128, 128, 128)

#Pick one of the following:
#from graphical_display import Display  #Requires pygame
#from tkinter_display import Display #Requires tkinter
#from text_display import Display
from no_display import Display

NUMBER_OF_BOARDS = 3

NUMBER_OF_COLORS = sum([trap_type.max_traps for trap_type in trap.trap_types])\
                   + NUMBER_OF_SAFE_COLORS

NUMBER_OF_TURNS = 10000

INITIAL_SPECIMENS = 15
SPECIMEN_LIFESPAN = 100
REPRODUCTION_RATE = 10
NUM_PARENTS = 2

DNA_LENGTH = 50
DNA_MAX_VALUE = (1 << DNA_LENGTH) - 1
DNA_CROSSOVER_RATE = .1
DNA_MUTATION_RATE = .01

VISION_WIDTH = 5
VISION_DISTANCE = int(VISION_WIDTH/2)
VISION = [[coordinates.Coordinate(x, y)
           for x in xrange(-VISION_DISTANCE, VISION_DISTANCE+1)
           ]
          for y in xrange(-VISION_DISTANCE, VISION_DISTANCE+1)
          ]

RANDOM_SEED = 13722829
