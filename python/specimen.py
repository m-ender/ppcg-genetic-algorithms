class Specimen(object):
    def __init__(self, dna, turn):
        self.dna = dna
        self.birth = turn
        self.bonus_fitness = 0

    def bit_at(self, position):
        return (self.dna >> position) & 1
