from __future__ import print_function

from board import Board
from random import Random
from specimen import Specimen
import player as Player
import sys
import trap
import itertools
import coordinates
#Pick one of the following 3:
from graphical_display import Display  #Requires pygame
#from text_display import Display
#from no_display import Display


if sys.version_info >= (3,):
    xrange = range

NUMBER_OF_BOARDS = 1
BOARD_WIDTH = coordinates.BOARD_WIDTH
BOARD_HEIGHT = coordinates.BOARD_HEIGHT

TRAP_FREQUENCY = .1
NUMBER_COLORS = 50  # needs to be bigger than 27 to have enough for the board

NUMBER_OF_TURNS = 100000

INITIAL_SPECIMENS = 50
SPECIMEN_LIFESPAN = 500
REPRODUCTION_RATE = 1
NUM_PARENTS = 2

DNA_LENGTH = 50
DNA_CROSSOVER_RATE = .1
DNA_MUTATION_RATE = .01

VISION_WIDTH = 5
VISION_DISTANCE = int(VISION_WIDTH/2)
VISION = [coordinates.Coordinate(x, y)
          for x in xrange(-VISION_DISTANCE, VISION_DISTANCE)
          for y in xrange(VISION_DISTANCE, -VISION_DISTANCE-1, -1)]

RANDOM_SEED = 13722829


random = Random(RANDOM_SEED)


def initialize_board():
    board = Board(random.randrange(0, 100000000))
    #Build each type of trap
    colors = list(range(NUMBER_COLORS))
    random.shuffle(colors)
    #TODO find a better way to generate traps and map them to colors
    all_traps = itertools.product(coordinates.directions, trap.trap_types)
    traps = [trap_type(board, direction, color)
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
            #Kill specimens of old age
            if turn_number == specimen.birth + SPECIMEN_LIFESPAN:
                points += int(coordinate.real_y/BOARD_HEIGHT)
            else:
                #calculate vision
                vision = [board.get_color(coordinate+offset)
                          for offset in VISION]
                #move specimen
                #TODO move the player to a random x position upon reaching top
                direction = player.take_turn(specimen, vision)
                new_location = coordinate+direction
                if new_location in board.next_specimens:
                    board.next_specimens[new_location].append(specimen)
                else:
                    board.next_specimens[new_location] = [specimen]
                #spring movement-based traps
                if new_location in board.traps:
                    board.traps[new_location].moved_to(new_location, coordinate)
    #transfer next_specimens to be the current specimens
    board.update_specimens()
    return points


def breed(board, current_turn):
    #Calculate the total height of all of the specimens
    total = 0
    for coordinate, specimens in board.specimens.items():
        total += (coordinate.y+1)*len(specimens)
    #Pick random heights from the total height to find a parent
    specimen_positions = [random.randrange(total) for _ in xrange(NUM_PARENTS)]
    selected_specimens = []
    for coordinate, specimens in board.specimens.items():
        if not specimens:
            continue
        #subtract a specimen's height from the random height
        specimen_positions = [position-(coordinate.y+1)*len(specimens)
                              for position in specimen_positions]
        to_remove_position = -1
        for position in specimen_positions:
            #once the random height is below 0
            if position < 0:
                #pick that specimen as one of the parents
                selected_specimens.append(random.choice(specimens))
                to_remove_position = position
        #and remove the random height from the random heights
        if to_remove_position >= 0:
            del specimen_positions[to_remove_position]
        if len(selected_specimens) == NUM_PARENTS:
            break
    #choose a random parent
    current_parent = random.choice(selected_specimens)
    new_dna = 0
    for position in reversed(xrange(DNA_LENGTH)):
        #randomly switch parents
        if random.random() < DNA_CROSSOVER_RATE:
            current_parent = random.choice(selected_specimens)
        #copy over dna from the chosen parent
        bit = current_parent.bit_at(position)
        #mutate some of that data
        if random.random() < DNA_MUTATION_RATE:
            bit = -bit+1
        new_dna = (new_dna+bit) << 2
    #create specimen with new dna
    board.add_specimen(
        Specimen(new_dna, current_turn),
        coordinates.Coordinate(random.randrange(0, BOARD_WIDTH), 0))


def check_for_life(board):
    for coordinate, specimens in board.specimens.items():
        if len(specimens) != 0:
            return True
    return False


def run():
    player = Player.PLAYER_TYPE()
    total_points = 0
    reproduction_counter = 0
    display = Display(BOARD_HEIGHT, BOARD_WIDTH)
    for board_number in xrange(NUMBER_OF_BOARDS):
        print("Running board #"+str(board_number+1)+"/"+str(NUMBER_OF_BOARDS))
        board = initialize_board()
        for turn_number in xrange(NUMBER_OF_TURNS):
            # Move
            total_points += take_turn(board, turn_number, player)
            # Kill
            for coordinate, trap in board.traps.items():
                trap.turn(coordinate)
            if not check_for_life(board):
                break
            # Reproduce
            reproduction_counter += REPRODUCTION_RATE
            while reproduction_counter >= 1:
                reproduction_counter -= 1
                breed(board, turn_number)
            #Draw tiles
            for coordinate in board.get_changed_cells():
                display.draw_cell(coordinate, board)
            display.update()
        #Score remaining specimen
        for coordinate, specimen in board.specimens.items():
            total_points += int(coordinate.real_y/BOARD_HEIGHT)*specimen
    print("Your bot got "+str(total_points)+" points")


if __name__ == "__main__":
    run()
