from dataclasses import *
from drawText import *
import json

BLACK = (0, 0, 0)
WHITE = (255, 255, 255)
GREEN = (0, 255, 0)
RED = (255, 0, 0)
BLUE = (0, 0, 255)
YELLOW = (255, 255, 0)
ORANGE = (255, 127, 80)
PINK = (255, 0, 177)
PURPLE = (107, 0, 179)
CYAN = (102, 255, 255)
DARKPURPLE = (102, 0, 102)

def saveToJson(objects, file_path, SIZE_MULTIPLIER):
    data = [obj.to_dict(SIZE_MULTIPLIER) for obj in objects]
    with open(file_path, "w") as file:
        json.dump(data, file)

def loadFromJson(file_path, SIZE_MULTIPLIER):
    with open(file_path, "r") as file:
        data = json.load(file)
    return [Object.from_dict(obj_data, SIZE_MULTIPLIER) for obj_data in data]

@dataclass
class Object():
    x: float
    y: float
    z: float
    w: float
    h: float
    d: float
    rx: float
    ry: float
    rz: float
    id: int
    selected: bool

    def __init__(self, x, y, z, w, h, d, rx, ry, rz, id):
        self.x = x
        self.y = y
        self.z = z
        self.w = w
        self.h = h
        self.d = d
        self.rx = rx
        self.ry = ry
        self.rz = rz
        self.id = id
        self.selected = False

    def rect(self):
        return pygame.Rect(self.x, self.y, self.w, self.h)
    
    def rect3D(self):
        return pygame.Rect(self.x, self.z, self.w, self.d)
    
    def projRec(self, offset_x, offset_y):
        return pygame.Rect(self.x + offset_x, self.z + offset_y, self.w, self.d)
    
    def render(self, screen, color, gridOriginX, gridOriginY):
        pygame.draw.rect(screen, color, pygame.Rect(self.x + gridOriginX, self.y + gridOriginY, self.w, self.h))
        drawText(screen, self.id, self.x + gridOriginX + 15, self.y + gridOriginY + 15, pygame.font.Font("../fonts/CascadiaMono.ttf", 32), WHITE, None)

        if self.selected:
            pygame.draw.rect(screen, YELLOW, pygame.Rect(self.x + gridOriginX, self.y + gridOriginY, self.w, self.h), 1)

    def to_dict(self, SIZE_MULTIPLIER):
        return {
            "x": (self.x + self.w/2)/SIZE_MULTIPLIER,
            "y": self.y,
            "z": (self.z + self.d/2)/SIZE_MULTIPLIER,
            "w": self.w/SIZE_MULTIPLIER,
            "h": self.h,
            "d": self.d/SIZE_MULTIPLIER,
            "rx": self.rx,
            "ry": self.ry,
            "rz": self.rz,
            "id": self.id,
        }

    @classmethod
    def from_dict(cls, data, SIZE_MULTIPLIER):
        return cls(
            x = (data["x"] - data["w"]/2)*SIZE_MULTIPLIER,
            y = data["y"],
            z = (data["z"] - data["d"]/2)*SIZE_MULTIPLIER,
            w = data["w"] * SIZE_MULTIPLIER,
            h = data["h"],
            d = data["d"] * SIZE_MULTIPLIER,
            rx = data["rx"],
            ry = data["ry"],
            rz = data["rz"],
            id = data["id"]
        )