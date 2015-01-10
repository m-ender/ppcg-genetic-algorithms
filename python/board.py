import trap as t
from random import Random

class Board(object):
    def __init__(self, seed):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
        self.next_specimens = {}
        self.unused_colors = []

    def add_specimen(self, specimen, coordinates):
        if coordinates in self.specimens:
            self.specimens[coordinates].append(specimen)
        else:
            self.specimens[coordinates] = [specimen]

    def add_trap(self, trap, coordinates):
        self.traps[coordinates] = trap

    def get_color(self, coordinates):
        if coordinates in self.traps:
            return self.traps[coordinates].color
        else:
            return self.random.choice(self.unused_colors)
