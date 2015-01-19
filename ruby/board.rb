require_relative 'vector2d'
require_relative 'cell'
require_relative 'constants'

class Board
    attr_accessor :specimens

    def initialize(rng)
        @rng = rng
        # Hash from coordinate to list of specimens
        @specimens = {}

        # Create a cell prototype for out of bounds cells.
        # These are empty but lethal.
        @oob_prototype = Cell.new(-1)
        @oob_prototype.lethal = true

        # Keep generating boards until they are valid
        loop do
            # Assign available colors to cell types
            remaining_colors = [*0...NUMBER_OF_COLORS]

            empty_colors = remaining_colors.sample(NUMBER_OF_EMPTY_COLORS, random: @rng)
            remaining_colors -= empty_colors

            teleporter_colors = remaining_colors.sample(NUMBER_OF_TELEPORTER_COLORS, random: @rng)
            remaining_colors -= teleporter_colors

            trap_colors = remaining_colors.sample(NUMBER_OF_TRAP_COLORS, random: @rng)
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
                    dx = @rng.rand(TELEPORT_WIDTH) - TELEPORT_DISTANCE
                    dy = @rng.rand(TELEPORT_WIDTH) - TELEPORT_DISTANCE
                end
                cell_prototypes[c1] = Teleporter.new(c1, dx, dy)
                cell_prototypes[c2] = Teleporter.new(c2, -dx, -dy)
            end

            trap_colors.each do |c|
                dx = @rng.rand(3) - 1
                dy = @rng.rand(3) - 1
                cell_prototypes[c] = Trap.new(c, dx, dy)
            end

            wall_colors.each do |c|
                cell_prototypes[c] = Wall.new(c)
            end

            # Fill board with cells
            @cells = Array.new(BOARD_EXTENDED_WIDTH) { |x|
                Array.new(BOARD_HEIGHT) { |y|
                    if x >= UNSAFE_BOARD_WIDTH
                        Cell.new(empty_colors.sample(random: @rng))
                    else
                        cell_prototypes.sample(random: @rng).clone
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

            # Validate
            determine_starting_cells
            break if @starting_cells.length >= 10
            puts "Bad board, retrying..."
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

    def determine_starting_cells
        @starting_cells = []
        (0...BOARD_HEIGHT).each do |y|
            safe = false
            checked = Hash.new false
            yet_to_check = [[Vector2D.new(0,y), 0]]
            while yet_to_check.size > 0
                coord, distance = *yet_to_check.shift
                break if distance >= SPECIMEN_LIFESPAN
                MOVES.each do |move|
                    new_coord = coord + move
                    if get(new_coord).is_teleporter
                        new_coord += get(new_coord).offset
                    end

                    next if checked[new_coord] ||
                            yet_to_check.include?([new_coord, distance+1]) ||
                            get(new_coord).lethal

                    if new_coord.at_goal
                        safe = true
                        break
                    end

                    yet_to_check.push [new_coord, distance+1]
                end

                if safe
                    @starting_cells.push Vector2D.new(0, y)
                    break
                end

                checked[coord] = true
            end
        end
    end

    def add_specimen(specimen)
        coord = @starting_cells.sample(random: @rng)
        if @specimens[coord]
            @specimens[coord].push specimen
        else
            @specimens[coord] = [specimen]
        end
    end

    def population
        total = 0
        @specimens.each { |c,l| total += l.size }
        return total
    end

    def get_vision(vec2d)
        VISION.map{|c| get(vec2d+c).color}.each_slice(5).to_a
    end
end