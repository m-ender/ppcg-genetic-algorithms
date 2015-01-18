class Specimen
    attr_accessor :genome, :birth, :bonus_fitness

    def initialize(genome, turn = 0)
        @genome = genome
        @birth = turn
        @bonus_fitness = 0
    end

    def bit_at(i)
        (@genome >> position) & 1
    end
end