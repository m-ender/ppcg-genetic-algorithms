require_relative 'vector2d'

class Player
    def initialize(rng)
        @rng = rng
    end

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
        (@genome >> start) & ((1 << (stop-start)) - 1)
    end

    def bit_chunk(start, length)
        (@genome >> start) & ((1 << length) - 1)
    end

    def vision_at(x, y)
        @vision[x+2][y+2]
    end
end

class RandomPlayer < Player
    def turn
        Vector2D.new(1,@rng.rand(3)-1)
    end
end

class LemmingPlayer < Player
    def initialize(rng)
        super(rng)
        @coords = [Vector2D.new(-1,-1),
                   Vector2D.new(-1, 0),
                   Vector2D.new(-1, 1),
                   Vector2D.new( 1, 0)]
    end

    def turn
        @coords.sample(random: @rng)
    end
end

class ColorScorePlayer < Player
    def initialize(rng)
        super(rng)
        @coords = [Vector2D.new( 1,-1),
                   Vector2D.new( 1, 0),
                   Vector2D.new( 1, 1)]
    end

    def vision_at(vec2d)
        @vision[vec2d.x+2][vec2d.y+2]
    end

    def turn
        max_score = @coords.map { |c|
            color = vision_at(c)
            color < 0 ? -1 : bit_chunk(6*color, 6)
        }.max

        restricted_coords = @coords.select { |c|
            color = vision_at(c)
            color >= 0 && bit_chunk(6*color, 6) == max_score
        }

        restricted_coords.sample(random: @rng)
    end
end

PLAYER = ColorScorePlayer