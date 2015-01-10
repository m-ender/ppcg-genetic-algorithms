class Specimen(object):
    def __init__(self, player, dna):
        self.player = player
        self.dna = dna

    def __getitem__(self, item):
        return self.bit_at(item)

    def bit_at(self, position):
        return (self.dna >> position) & 1

    def bit_range(self, start, stop):
        return (self.dna >> start) & ((1 << (stop-start)) - 1)