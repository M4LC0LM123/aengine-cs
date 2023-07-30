from object import *

def saveScene(objects, filename):
    with open(filename, 'w') as file:
        for obj in objects:
            line = f'x: {obj.x} y: {obj.y} z: {obj.z} w: {obj.w} h: {obj.h} d: {obj.d} rx: {obj.rx} ry: {obj.ry} rz: {obj.rz} id: {obj.id}\n'
            file.write(line)