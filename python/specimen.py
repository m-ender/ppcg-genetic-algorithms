class Specimen(object):
    def __init__(self, genome, turn):
        self.genome = genome
        self.birth = turn
        self.bonus_fitness = 0

    def bit_at(self, position):
        return (self.genome >> position) & 1
