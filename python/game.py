from constants import *

random = Random()

TotalFitness = 0
MaxFitness = 0
AllTimeMaxFitness = 0

def sanitized(board):
    safe_squares = []
    for start_y in xrange(BOARD_HEIGHT):
        start_coordinate = coordinates.Coordinate(0, start_y)
        next_squares = {start_coordinate}
        visited_squares = set(next_squares)
        for __ in xrange(SPECIMEN_LIFESPAN):
            neighbors = [direction+coordinate
                         for direction in coordinates.directions
                         for coordinate in next_squares]
            teleported = [board.get_square(neighbor).teleport + neighbor
                          for neighbor in neighbors]
            alive = [square for square in teleported
                     if not board.get_square(square).killer]
            if any([square.at_finish() or square in safe_squares
                    for square in alive]):
                safe_squares.append(start_coordinate)
                break
            unvisited = [square for square in alive
                         if square not in visited_squares]
            if len(unvisited) == 0:
                break
            visited_squares.update(unvisited)
            next_squares = set(unvisited)
    return safe_squares


def initialize_board():
    colors = list(range(NUMBER_OF_COLORS))
    random.shuffle(colors)
    while True:
        board = Board(random.randrange(0, 10000000), colors)
        safe_squares = sanitized(board)
        if len(safe_squares) >= 10:
            board.starting_squares = safe_squares
            break
        print("Bad board, retrying...")

    #add specimens
    for __ in xrange(INITIAL_SPECIMENS):
        board.add_specimen(
            Specimen(random.getrandbits(GENOME_LENGTH), 0),
            random.choice(safe_squares))

    return board


def take_turn(board, turn_number, player):
    points = 0
    for coordinate, specimens in board.specimens.items():
        for specimen in specimens:
            #Send winners back to start
            if coordinate.at_finish():
                points += 1
                new_start_coords = random.choice(board.starting_squares)
                specimen.birth = turn_number
                specimen.bonus_fitness += coordinates.UNSAFE_BOARD_WIDTH
                if new_start_coords in board.next_specimens:
                    board.next_specimens[new_start_coords].append(specimen)
                else:
                    board.next_specimens[new_start_coords] = [specimen]
                continue
            #Kill specimens of old age
            if turn_number == specimen.birth + SPECIMEN_LIFESPAN:
                continue
            #calculate vision
            vision = [[board.get_color(coordinate+offset)
                      for offset in line] for line in VISION]
            #move specimen
            direction = player.take_turn(specimen.genome, vision)
            new_location = coordinate+direction
            new_square = board.get_square(new_location)
            if new_square.wall:
                new_square = board.get_square(coordinate)
                new_location = coordinate
            teleported = new_square.teleport+new_location
            if board.get_square(teleported).killer and not coordinate.at_finish():
                continue
            if teleported in board.next_specimens:
                board.next_specimens[teleported].append(specimen)
            else:
                board.next_specimens[teleported] = [specimen]

    #transfer next_specimens to be the current specimens
    board.update_specimens()
    return points


def score_specimen(coordinate, specimen):
    return coordinate.x + specimen.bonus_fitness + 1


def breed(board, current_turn, number_of_offspring):
    global TotalFitness, MaxFitness, AllTimeMaxFitness
    #Calculate the total height of all of the specimens
    total = 0
    MaxFitness = 0
    for coordinate, specimens in board.specimens.items():
        for specimen in specimens:
            fitness = score_specimen(coordinate, specimen)
            if fitness > MaxFitness:
                MaxFitness = fitness
                if fitness > AllTimeMaxFitness:
                    AllTimeMaxFitness = fitness
            total += fitness
    TotalFitness = total
    #Pick random heights from the total height to find a parent
    parent_groups = []
    for __ in xrange(number_of_offspring):
        selected_specimens = []
        remaining_total = total
        for __ in xrange(NUM_PARENTS):
            count_down = random.randrange(remaining_total)
            for coordinate, specimens in board.specimens.items():
                for specimen in specimens:
                    if specimen in selected_specimens:
                        continue
                    count_down -= score_specimen(coordinate, specimen)
                    if count_down < 0:
                        selected_specimens.append(specimen)
                        remaining_total -= score_specimen(coordinate, specimen)
                        break
                else:
                    continue
                break
        parent_groups.append(selected_specimens)
        
    for i in xrange(number_of_offspring):
        selected_specimens = parent_groups[i]
        current_parent = random.choice(selected_specimens)
        new_genome = 0
        for position in reversed(xrange(GENOME_LENGTH)):
            #randomly switch parents
            if random.random() < GENOME_CROSSOVER_RATE:
                current_parent = random.choice(selected_specimens)
            #copy over genome from the chosen parent
            bit = current_parent.bit_at(position)
            #mutate some of that data
            if random.random() < GENOME_MUTATION_RATE:
                bit = -bit+1
            new_genome = (new_genome << 1) + bit
        assert new_genome <= GENOME_MAX_VALUE
        #create specimen with new genome
        board.add_specimen(
            Specimen(new_genome, current_turn),
            random.choice(board.starting_squares)
            )


def check_for_life(board):
    if len(board.specimens) >= NUM_PARENTS:
        return True
    population = 0
    for cell in board.specimens:
        population += len(cell)
        if population >= NUM_PARENTS:
            return True


def run():
    player = Player.PLAYER_TYPE()
    game_records = []
    display = Display(BOARD_HEIGHT, BOARD_EXTENDED_WIDTH)
    for board_number in xrange(NUMBER_OF_BOARDS):
        global TotalFitness, MaxFitness, AllTimeMaxFitness
        AllTimeMaxFitness = 0
        total_points = 0
        print("Running board #"+str(board_number+1)+"/"+str(NUMBER_OF_BOARDS))
        board = initialize_board()
        start = time.time()
        for turn_number in xrange(NUMBER_OF_TURNS):
            # Move
            total_points += take_turn(board, turn_number, player)
            if not check_for_life(board):
                break
            # Reproduce
            breed(board, turn_number, REPRODUCTION_RATE)
            #Draw tiles
            for coordinate in board.get_changed_cells():
                display.draw_cell(coordinate, board)
            display.update()
            if not turn_number % int(NUMBER_OF_TURNS/100):
                population = 0
                for c, specimens in board.specimens.items():
                    population += len(specimens)
                print('{:3.0%} '.format(turn_number/NUMBER_OF_TURNS) +
                      '{:5.4}s '.format(time.time() - start) +
                      '{: 10} pts '.format(total_points) +
                      'Pop {: 5} '.format(population) +
                      'Fit ' +
                      'Avg {:7.3} '.format(TotalFitness/float(population)) +
                      'Max {: 5} '.format(MaxFitness) +
                      'AllTimeMax {: 5}'.format(AllTimeMaxFitness)
                      )
        #Score remaining specimen
        for coordinate, specimen in board.specimens.items():
            if coordinate.at_finish():
                total_points += len(specimen)
        print("Your bot got "+str(total_points)+" points")
        game_records.append(total_points)
    if NUMBER_OF_BOARDS > 1:
        print("=========================================")
        print("Individual scores: "+str(game_records))
        print("On average, your bot got "+str(sum(game_records)/float(NUMBER_OF_BOARDS))+" points")


if __name__ == "__main__":
    run()
