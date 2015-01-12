from random import Random


class Board(object):
    def __init__(self, seed, blank_colors):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.blank_cell = ColoredSquare(self.random.choice(blank_colors))
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
        return self.traps.get(coordinates, self.blank_cell)

    def get_changed_cells(self):
        changed = self.changed_cells
        self.changed_cells = set()
        return changed

    def update_specimens(self):
        self.changed_cells.update(self.next_specimens.keys())
        self.changed_cells.update(self.specimens.keys())
        self.specimens = self.next_specimens
        self.next_specimens = {}


class ColoredSquare(object):
    def __init__(self, color):
        self.color = color