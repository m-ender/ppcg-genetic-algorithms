from __future__ import print_function

from board import Board
from random import Random
from specimen import Specimen
from player import Player
import sys
import trap
import itertools
import coordinates

if sys.version_info >= (3,):
    xrange = range

NUMBER_OF_BOARDS = 1
BOARD_WIDTH = 50
BOARD_HEIGHT = 200

TRAP_FREQUENCY = .2

SPECIMEN_LIFESPAN = 500

NUMBER_OF_TURNS = 100000

INITIAL_SPECIMENS = 50

REPRODUCTION_RATE = 1

DNA_LENGTH = 50
DNA_CROSSOVER_RATE = .1

LIFE_SPAN = 200

VISION_WIDTH = 5

NUM_PARENTS = 2

VISION_DISTANCE = int(VISION_WIDTH/2)

VISION = [coordinates.Coordinate(x, y)
          for x in xrange(-VISION_DISTANCE, VISION_DISTANCE)
          for y in xrange(VISION_DISTANCE, -VISION_DISTANCE-1, -1)]

SEED = 13722829

NUMBER_COLORS = 50  # needs to be bigger than 27

random = Random(SEED)


def initialize_board():
    board = Board(random.randrange(0, 100000000))
    #Build each type of trap
    colors = list(range(NUMBER_COLORS))
    random.shuffle(colors)
    all_traps = itertools.product(coordinates.directions, trap.trap_types)
    traps = [trap_type(direction, board, color)
             for (direction, trap_type), color in zip(all_traps, colors)]
    board.unused_colors = \
        colors[len(coordinates.directions)*len(trap.trap_types):]

    #add specimens
    for __ in xrange(INITIAL_SPECIMENS):
        board.add_specimen(
            Specimen(random.getrandbits(DNA_LENGTH), 0),
            coordinates.Coordinate(random.randrange(0, BOARD_WIDTH), 0))

    #add traps
    for height in xrange(BOARD_HEIGHT):
        for width in xrange(BOARD_WIDTH):
            if random.random() < TRAP_FREQUENCY:
                board.add_trap(random.choice(traps),
                               coordinates.Coordinate(width, height))
    return board


def take_turn(board, turn_number, player):
    points = 0
    for coordinate, specimens in board.specimens.items():
        for specimen in specimens:
            if turn_number == specimen.birth + SPECIMEN_LIFESPAN:
                points += int(coordinate.y/BOARD_HEIGHT)
            else:
                vision = [board.get_color(coordinate+offset)
                          for offset in VISION]
                direction = player.take_turn(specimen, vision)
                new_location = coordinate+direction
                if new_location in board.next_specimens:
                    board.next_specimens[new_location].append(specimen)
                else:
                    board.next_specimens[new_location] = [specimen]
    board.specimens = board.next_specimens
    board.next_specimens = {}
    return points


def breed(board, current_turn):
    total = 0
    for coordinate, specimens in board.specimens.items():
        total += (coordinate.y+1)*len(specimens)
    specimen_positions = [random.randrange(total) for _ in xrange(NUM_PARENTS)]
    selected_specimens = []
    for coordinate, specimens in board.specimens.items():
        specimen_positions = [position-(coordinate.y+1)*len(specimens)
                              for position in specimen_positions]
        to_remove_position = -1
        for position in specimen_positions:
            if position < 0:
                selected_specimens.append(random.choice(specimens))
                to_remove_position = position
        if to_remove_position >= 0:
            del specimen_positions[to_remove_position]
        if len(selected_specimens) == NUM_PARENTS:
            break
    current_parent = random.choice(selected_specimens)
    new_dna = 0
    for position in reversed(xrange(DNA_LENGTH)):
        if random.random() < DNA_CROSSOVER_RATE:
            current_parent = random.choice(selected_specimens)
        bit = current_parent.bit_at(position)
        new_dna = (new_dna+bit) << 2
    board.add_specimen(
        Specimen(new_dna, current_turn),
        coordinates.Coordinate(random.randrange(0, BOARD_WIDTH), 0))


def check_for_life(board):
    for coordinate, specimens in board.specimens.items():
        if len(specimens) != 0:
            return True
    return False


def run():
    player = Player()
    total_points = 0
    reproduction_counter = 0
    for board_number in xrange(NUMBER_OF_BOARDS):
        print("Running board #"+str(board_number+1)+"/"+str(NUMBER_OF_BOARDS))
        board = initialize_board()
        for turn_number in xrange(NUMBER_OF_TURNS):
            # Move
            total_points += take_turn(board, turn_number, player)
            if not check_for_life(board):
                break
            # Reproduce
            reproduction_counter += REPRODUCTION_RATE
            while reproduction_counter >= 1:
                reproduction_counter -= 1
                breed(board, turn_number)
        #Score remaining specimen
        for coordinate, specimen in board.specimens.items():
            total_points += int(coordinate.y/BOARD_HEIGHT)*specimen
    print("Your bot got "+str(total_points)+" points")


if __name__ == "__main__":
    run()
