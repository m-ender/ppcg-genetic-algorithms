from coordinates import Coordinate
import random


class Player(object):
    def __init__(self):
        self.vision = None
        self.dna = None

    def take_turn(self, dna, vision):
        self.vision = vision
        self.dna = dna
        return self.turn()

    def bit_at(self, position):
        return (self.dna >> position) & 1

    def bit_range(self, start, stop):
        return (self.dna >> start) & ((1 << (stop-start)) - 1)

    def turn(self):
        return Coordinate(1, 0)

    def vision_at(self, coordinates):
        adjusted_coordinates = Coordinate(2, 2)+coordinates
        return self.vision[adjusted_coordinates.x, adjusted_coordinates.y]


class RandomPlayer(Player):
    def turn(self):
        return Coordinate(1, random.randint(-1, 1))


class LinearCombinationPlayer(Player):
    def __init__(self):
        Player.__init__(self)
        self.coords = [#Coordinate(-1,-1),
                       #Coordinate( 0,-1),
                       Coordinate( 1, 0),
                       Coordinate( 1,-1),
                       #Coordinate(-1, 0),
                       #Coordinate( 0, 0),
                       #Coordinate(-1, 1),
                       #Coordinate( 0, 1),
                       Coordinate( 1, 1)]
        self.n_moves = len(self.coords)

    def turn(self):
        s = 0
        for i in range(25):
            s += self.bit_range(2*i,2*i+2)*self.vision[i]
        return self.coords[s%self.n_moves]

PLAYER_TYPE = RandomPlayer