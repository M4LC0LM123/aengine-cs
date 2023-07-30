import pygame
import pygame_gui
from dataclasses import *
from drawText import *
from object import Object

SCREEN_WIDTH = 1280
SCREEN_HEIGHT = 720

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

@dataclass
class ObjectInfo():
    panel: pygame.Rect
    boundObj: Object

    def __init__(self, boundObj, panelRec = pygame.Rect(SCREEN_WIDTH - 250, 10, 120, 350)):
        self.panel = panelRec
        self.boundObj = boundObj

    def set(self, boundObj, panelRec = pygame.Rect(SCREEN_WIDTH - 250, 10, 120, 350)):
        self.panel = panelRec
        self.boundObj = boundObj

    def render(self, screen):
        pygame.draw.rect(screen, "#32628A", self.panel)
        drawText(screen, "x: " + str(self.boundObj.x), self.panel.x + 55, self.panel.y + 10, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "y: " + str(self.boundObj.y), self.panel.x + 55, self.panel.y + 30, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "z: " + str(self.boundObj.z), self.panel.x + 55, self.panel.y + 50, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "w: " + str(self.boundObj.w), self.panel.x + 55, self.panel.y + 70, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "h: " + str(self.boundObj.h), self.panel.x + 55, self.panel.y + 90, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "d: " + str(self.boundObj.d), self.panel.x + 55, self.panel.y + 110, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "rx: " + str(self.boundObj.rx), self.panel.x + 55, self.panel.y + 130, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "ry: " + str(self.boundObj.ry), self.panel.x + 55, self.panel.y + 150, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "rz: " + str(self.boundObj.rz), self.panel.x + 55, self.panel.y + 170, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)
        drawText(screen, "id: " + str(self.boundObj.id), self.panel.x + 55, self.panel.y + 190, pygame.font.Font("../fonts/CascadiaMono.ttf", 14), BLACK, None)

        
