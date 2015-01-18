require_relative 'vector2d'

class Player
    def take_turn(genome, vision)
        @genome = genome
        @vision = vision
        turn
    end

    def turn
        Vector2D.new(1,0)
    end

    def bit_at(i)
        (@genome >> i) & 1
    end

    def bit_range(start, stop)
        (@dna >> start) & ((1 << (stop-start)) - 1)
    end

    def bit_chunk(start, length)
        (@dna >> start) & ((1 << length) - 1)
    end

    def vision_at(x, y)
        @vision[x+2][y+2]
    end
end

class RandomPlayer < Player
    def turn
        Vector2D.new(1,rand(3)-1)
    end
end

PLAYER = RandomPlayer