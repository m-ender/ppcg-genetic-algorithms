require_relative 'vector2d'
require_relative 'constants'

class Cell
    attr_accessor :type, :color, :offset, :lethal

    def initialize(color, x=0, y=0)
        @type = :empty
        @color = color
        @offset = Vector2D.new(x,y)
        @lethal = false
    end

    def is_empty
        @type == :empty
    end

    def is_wall
        @type == :wall
    end

    def is_trap
        @type == :trap
    end

    def is_teleporter
        @type == :teleporter
    end

    def color_s
        @color.to_s(36)
    end

    def property_s
        is_wall ? '#' : @lethal ? 'X' : is_teleporter ? '@' : ' '
    end
end

class Wall < Cell
    def initialize(color, x=0, y=0)
        super(color, x, y)
        @type = :wall
        @lethal = true
    end
end

class Trap < Cell
    def initialize(color, x=0, y=0)
        super(color, x, y)
        @type = :trap
    end
end

class Teleporter < Cell
    def initialize(color, x=0, y=0)
        super(color, x, y)
        @type = :teleporter
    end
end