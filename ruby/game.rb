require_relative 'constants'
require_relative 'vector2d'
require_relative 'cell'
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

def score_specimen(coord, specimen)
    coord.x + specimen.bonus_fitness + 1
end

def breed(board, turn)
    $TotalFitness = 0
    $MaxFitness = 0
    board.specimens.each do |coord, specimens|
        specimens.each do |specimen|
            fitness = score_specimen(coord, specimen)
            $TotalFitness += fitness
            $MaxFitness = fitness if fitness > $MaxFitness
        end
    end
    $AllTimeMaxFitness = $MaxFitness if $MaxFitness > $AllTimeMaxFitness
    parent_groups = []
    REPRODUCTION_RATE.times do
        selected_parents = []
        remaining_total = $TotalFitness
        NUMBER_OF_PARENTS.times do
            count_down = $RNG.rand(remaining_total)
            selected = false
            board.specimens.each do |coord, specimens|
                specimens.each do |specimen|
                    next if selected_parents.include? specimen
                    fitness = score_specimen(coord, specimen)
                    count_down -= fitness
                    if count_down < 0
                        selected_parents.push specimen
                        remaining_total -= fitness
                        selected = true
                        break
                    end
                end
                break if selected
            end
        end
        parent_groups.push selected_parents
    end

    parent_groups.each do |parents|
        parent = parents.sample(random: $RNG)
        new_genome = 0
        (GENOME_LENGTH-1).downto(0).each do |position|
            parent = parents.sample(random: $RNG) if $RNG.rand < GENOME_CROSSOVER_RATE
            bit = parent.bit_at(position)
            b = 1-bit if $RNG.rand < GENOME_MUTATION_RATE
            new_genome = (new_genome << 1) + bit
        end
        raise "Genome too long: #{new_genome}" if new_genome > GENOME_MAX_VALUE
        board.add_specimen(Specimen.new(new_genome, turn))
    end
end

player = nil

game_records = []

NUMBER_OF_BOARDS.times do |board_number|
    puts "Running board ##{board_number+1}/#{NUMBER_OF_BOARDS}"
    $RNG = Random.new(rand(2**64))
    puts "Initialized new RNG with seed #{$RNG.seed}"
    puts
    board = Board.new($RNG)
    player = PLAYER.new($RNG)

    INITIAL_SPECIMENS.times do
        board.add_specimen(Specimen.new($RNG.rand(GENOME_MAX_VALUE)))
    end

    $TotalFitness = $MaxFitness = $AllTimeMaxFitness = 0
    total_points = 1

    start = Time.now

    NUMBER_OF_TURNS.times do |turn|
        # Move
        total_points += take_turn(board, turn, player)

        # Reproduce
        population = board.population
        break if population < NUMBER_OF_PARENTS
        breed(board, turn)

        if turn % (NUMBER_OF_TURNS/100) == 0
            puts "%2d%% %4.1fs %10d pts Pop %5d Fit Avg %7.3f Max %5d AllTimeMax %5d" % [
                turn*100/NUMBER_OF_TURNS,
                Time.now - start,
                total_points,
                population,
                $TotalFitness/population.to_f,
                $MaxFitness,
                $AllTimeMaxFitness
            ]
        end
    end

    puts "Your bot got #{total_points} points."
    game_records.push total_points
end

if NUMBER_OF_BOARDS > 1
    puts "========================================="
    puts "Individual scores: #{game_records}"
    puts "Your overall score is #{game_records.reduce(:*)**(1/NUMBER_OF_BOARDS.to_f)}."
end