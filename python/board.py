from random import Random


class Board(object):
    def __init__(self, seed):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.unused_colors = []
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
        if coordinates in self.traps:
            return self.traps[coordinates].color
        else:
            return self.random.choice(self.unused_colors)

    def get_changed_cells(self):
        changed = self.changed_cells
        self.changed_cells = set()
        return changed

    def update_specimens(self):
        self.changed_cells.update(self.next_specimens.keys())
        self.changed_cells.update(self.specimens.keys())
        self.specimens = self.next_specimens
        self.next_specimens = {}
