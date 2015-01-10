
from random import Random

class Board(object):
    def __init__(self, seed):
        self.random = Random(seed)
        self.specimens = {}
        self.traps = {}
