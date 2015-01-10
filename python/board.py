
from random import Random

class Board(object):
    def __init__(self, seed):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}

    def add_specimen(self, specimen, coordinates):
        if coordinates in self.specimens:
            self.specimens[coordinates].append(specimen)
        else:
            self.specimens[coordinates] = [specimen]

    def add_trap(self, trap, coordinates):
        self.traps[coordinates] = trap
