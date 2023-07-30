
def saveTileMap(array, filename):
    with open(filename, 'w') as file:
        for row in array:
            row_str = ','.join(str(element) for element in row)
            file.write(row_str + '\n')