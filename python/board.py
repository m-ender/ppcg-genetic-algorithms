from random import Random
from coordinates import BOARD_HEIGHT, BOARD_WIDTH
from trap import trap_types, Trap

OUT_OF_BOUNDS_COLOR = -1

class Board(object):
    def __init__(self, seed, colors):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.changed_cells = set()
        self.colors = [[self.random.choice(colors)
                        for __ in xrange(BOARD_WIDTH)]
                       for __ in xrange(BOARD_HEIGHT)]
        self.traps = [Trap(self, None)]*len(colors)
        for trap_type in trap_types:
            used_traps = self.random.sample(trap_type.possible_directions,
                                            trap_type.max_traps)
            coloring = zip(used_traps, colors)
            colors = colors[len(used_traps):]
            for direction, color in coloring:
                self.traps[color] = trap_type(self, direction)


    def get_trap(self, coordinate):
        return self.traps[self.get_color(coordinate)]

    def add_specimen(self, specimen, coordinates):
        if coordinates in self.specimens:
            self.specimens[coordinates].append(specimen)
        else:
            self.specimens[coordinates] = [specimen]
        self.changed_cells.add(coordinates)

    def add_square(self, color, coordinates):
        self.colors[coordinates.y][coordinates.x] = color

    def get_color(self, coordinates):
        if coordinates.x < 0 or coordinates.x >= BOARD_WIDTH or \
                coordinates.y < 0 or coordinates.y >= BOARD_HEIGHT:
            return OUT_OF_BOUNDS_COLOR
        return self.colors[coordinates.y][coordinates.x]

    def get_changed_cells(self):
        changed = self.changed_cells
        self.changed_cells = set()
        return changed

    def update_specimens(self):
        self.changed_cells.update(self.next_specimens.keys())
        self.changed_cells.update(self.specimens.keys())
        self.specimens = self.next_specimens
        self.next_specimens = {}


class BlankSquare(object):
    def __init__(self, color):
        self.color = color

    def color(self):
        return self.color