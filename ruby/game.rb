require_relative 'constants'
require_relative 'vector2d'
require_relative 'cell'
require_relative 'random'
require_relative 'board'
require_relative 'specimen'
require_relative 'player'

def take_turn(board, turn, player)
    points = 0
    specimens = board.specimens
    board.specimens = {}
    specimens.each do |coord, specimens|
        vision = board.get_vision(coord)
        specimens.each do |specimen|
            next if turn == specimen.birth + SPECIMEN_LIFESPAN
            move = player.take_turn(specimen.genome, vision)
            if move.x.abs > 1 || move.y.abs > 1
                abort("Invalid move (#{move.x}, #{move.y}). Exiting...")
            end
            new_coord = coord + move

            new_cell = board.get new_coord
            if new_cell.is_wall
                new_coord = coord
                new_cell = board.get new_coord
            end
            if new_cell.is_teleporter
                new_coord += new_cell.offset
                new_cell = board.get new_coord
            end
            next if new_cell.lethal

            if new_coord.at_goal
                points += 1
                specimen.birth = turn
                specimen.bonus_fitness += BOARD_WIDTH
                board.add_specimen(specimen)
                next
            end

            if board.specimens[new_coord]
                board.specimens[new_coord].push specimen
            else
                board.specimens[new_coord] = [specimen]
            end
        end
    end

    return points
end

def breed(board, turn)

end

player = PLAYER.new

game_records = []

NUMBER_OF_BOARDS.times do |board_number|
    puts "Running board ##{board_number+1}/#{NUMBER_OF_BOARDS}"
    board = Board.new(RNG)

    INITIAL_SPECIMENS.times do
        board.add_specimen(Specimen.new(RNG.rand(GENOME_MAX_VALUE)))
    end

    total_points = 0

    start = Time.now

    NUMBER_OF_TURNS.times do |turn|
        # Move
        total_points += take_turn(board, turn, player)

        # Reproduce
        population = board.population
        break if population < NUMBER_OF_PARENTS
        breed(board, turn)

        if turn % (NUMBER_OF_TURNS/100) == 0
            puts "%2d%% %4.3fs %10d pts Pop %5d" % [
                turn*100/NUMBER_OF_TURNS,
                Time.now - start,
                total_points,
                population
            ]
        end
    end

    puts "Your bot got #{total_points} points."
    game_records.push total_points
end

if NUMBER_OF_BOARDS > 1
    puts "========================================="
    puts "Individual scores: #{game_records}"
    puts "On average, your bot got #{game_records.reduce(:+)/NUMBER_OF_BOARDS.to_f} points."
end