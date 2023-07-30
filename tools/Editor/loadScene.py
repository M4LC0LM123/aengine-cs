from object import Object

def loadScene(filename):
    objects = []
    with open(filename, 'r') as file:
        for line in file:
            line = line.strip()  # Remove leading/trailing whitespace
            if not line:
                continue  # Skip empty lines
            values = line.split()  # Split line into separate values
            obj_values = {}
            obj_values['x'] = float(values[1])
            obj_values['y'] = float(values[3])
            obj_values['z'] = float(values[5])
            obj_values['w'] = float(values[7])
            obj_values['h'] = float(values[9])
            obj_values['d'] = float(values[11])
            obj_values['rx'] = float(values[13])
            obj_values['ry'] = float(values[15])
            obj_values['rz'] = float(values[17])
            obj_values['id'] = int(values[19])
            obj = Object(**obj_values)
            objects.append(obj)
    return objects