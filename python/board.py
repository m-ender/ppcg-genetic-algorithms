from random import Random
from coordinates import BOARD_HEIGHT, BOARD_WIDTH

OUT_OF_BOUNDS_COLOR = -1


class Board(object):
    def __init__(self, seed, blank_colors):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.blank_cell = [BlankSquare(blank_colors, self.random)]
        self.changed_cells = set()

    def add_specimen(self, specimen, coordinates):
        if coordinates in self.specimens:
            self.specimens[coordinates].append(specimen)
        else:
            self.specimens[coordinates] = [specimen]
        self.changed_cells.add(coordinates)

    def add_trap(self, trap, coordinates):
        self.traps[coordinates] = trap
        self.changed_cells.add(coordinates)

    def get_color(self, coordinates):
        if coordinates.x < 0 or coordinates.y < 0 or \
                coordinates.x > BOARD_WIDTH or coordinates.y > BOARD_HEIGHT:
            return OUT_OF_BOUNDS_COLOR
        self.traps.get(coordinates, self.blank_cell)

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
    def __init__(self, colors, random):
        self.colors = colors
        self.random = random

    def color(self):
        return self.random.choice(self.colors)