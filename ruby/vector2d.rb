class Vector2D
    attr_accessor :x, :y

    def initialize(x, y)
        @x = x
        @y = y
    end

    def self.from_string(string)
        coords = string.split.map(&:to_i)
        Vector2D.new(coords[0], coords[1])
    end

    def +(other)
        return Vector2D.new(@x+other.x, @y+other.y)
    end

    def -(other)
        return Vector2D.new(@x-other.x, @y-other.y)
    end

    def *(other)
        return Vector2D.new(@x*other, @y*other)
    end

    def ==(other)
        return @x == other.x && @y == other.y
    end

    def coerce(other)
        return self, other
    end

    def out_of_bounds
        return @x < 0 || @y < 0 || @y >= BOARD_HEIGHT
    end

    def at_goal
        return !out_of_bounds && @x >= UNSAFE_BOARD_WIDTH
    end

    def to_s
        "(% d,% d)" % [@x, @y]
    end

    def hash
        @x*2*BOARD_HEIGHT + @y
    end

    alias eql? ==
end