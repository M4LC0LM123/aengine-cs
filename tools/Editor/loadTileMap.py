
def loadTileMap(filename):
    array = []
    with open(filename, 'r') as file:
        for line in file:
            row = [int(element) for element in line.strip().split(',')]
            array.append(row)
    return array