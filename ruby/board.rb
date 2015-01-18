require_relative 'vector2d'
require_relative 'cell'
require_relative 'constants'

class Board
    attr_accessor :specimens

    def initialize(rng)
        @specimens = Hash.new { |hash, key| hash[key] = [] }

        # Assign available colors to cell types
        remaining_colors = [*0...NUMBER_OF_COLORS]

        empty_colors = remaining_colors.sample(NUMBER_OF_EMPTY_COLORS)
        remaining_colors -= empty_colors

        teleporter_colors = remaining_colors.sample(NUMBER_OF_TELEPORTER_COLORS)
        remaining_colors -= teleporter_colors

        trap_colors = remaining_colors.sample(NUMBER_OF_TRAP_COLORS)
        remaining_colors -= trap_colors

        wall_colors = remaining_colors

        # Create a cell prototype for each color
        cell_prototypes = [nil]*NUMBER_OF_COLORS

        empty_colors.each do |c|
            cell_prototypes[c] = Cell.new(c)
        end

        teleporter_colors.each_slice(2) do |c1, c2|
            dx = dy = 0
            while dx == 0 && dy == 0
                dx = rng.rand(TELEPORT_WIDTH) - TELEPORT_DISTANCE
                dy = rng.rand(TELEPORT_WIDTH) - TELEPORT_DISTANCE
            end
            cell_prototypes[c1] = Teleporter.new(c1, dx, dy)
            cell_prototypes[c2] = Teleporter.new(c2, -dx, -dy)
        end

        trap_colors.each do |c|
            dx = rng.rand(3) - 1
            dy = rng.rand(3) - 1
            cell_prototypes[c] = Trap.new(c, dx, dy)
        end

        wall_colors.each do |c|
            cell_prototypes[c] = Wall.new(c)
        end

        # Create another cell prototype for out of bounds cells.
        # These are empty but lethal.
        @oob_prototype = Cell.new(-1)
        @oob_prototype.lethal = true

        # Fill board with cells
        @cells = Array.new(BOARD_EXTENDED_WIDTH) { |x|
            Array.new(BOARD_HEIGHT) { |y|
                if x >= UNSAFE_BOARD_WIDTH
                    Cell.new(empty_colors.sample)
                else
                    cell_prototypes.sample.clone
                end
            }
        }

        # Activate traps
        @cells.each_with_index do |column, x|
            column.each_with_index do |cell, y|
                if cell.is_trap
                    lethal_cell = Vector2D.new(x,y)+cell.offset
                    get(lethal_cell).lethal = true
                end
            end
        end
    end

    def get(vec2d)
        vec2d.out_of_bounds ? @oob_prototype : @cells[vec2d.x][vec2d.y]
    end

    def to_s
        landscape = @cells.transpose
        landscape.map{ |l|
            l.map(&:color_s).join + ' '*5 + l.map(&:property_s).join
        }.join($/)
    end
end