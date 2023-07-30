import pygame
import pygame_gui
from drawText import *
import tkinter as tk
from tkinter import filedialog
from saveTileMap import *
from loadTileMap import *
from object import Object
from saveScene import *
from loadScene import *
from objectInfo import ObjectInfo
from Mode import Mode
from FileDialog import *

pygame.init()

SCREEN_WIDTH = 1280
SCREEN_HEIGHT = 720

screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT), pygame.RESIZABLE)
pygame.display.set_caption("aengine-editor")
clock = pygame.time.Clock()

#colors
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

SIZE_MULTIPLIER = 10

objects = []
currentId = 0

mouseX, mouseY = pygame.mouse.get_pos()

panelX = SCREEN_WIDTH - 125
panelY = 10
panel = pygame.Rect(panelX, panelY, 120, 500)
fontSize = 12

manager = pygame_gui.UIManager((SCREEN_WIDTH, SCREEN_HEIGHT), "../themes/theme.json")
save = pygame_gui.elements.UIButton(relative_rect = pygame.Rect((panelX, panelY), (120, 50)), text = "save", manager = manager)
load = pygame_gui.elements.UIButton(relative_rect = pygame.Rect((panelX, panelY + 50), (120, 50)), text = "load", manager = manager)
currYText = pygame_gui.elements.UILabel(relative_rect = pygame.Rect((panelX - 15, panelY + 75 + 12), (120, 50)), text = "current y", manager = manager)
currYInput = pygame_gui.elements.UITextEntryLine(relative_rect = pygame.Rect((panelX, panelY + 125), (120, 25)), manager = manager)
currHeightText = pygame_gui.elements.UILabel(relative_rect = pygame.Rect((panelX - 12.5, panelY + 125 + 12), (120, 50)), text = "current height", manager = manager)
currHeightInput = pygame_gui.elements.UITextEntryLine(relative_rect = pygame.Rect((panelX, panelY + 175), (120, 25)), manager = manager)
clearBtn = pygame_gui.elements.UIButton(relative_rect = pygame.Rect((panelX, panelY + 200), (120, 50)), text = "clear", manager = manager)
tileValueText = pygame_gui.elements.UILabel(relative_rect = pygame.Rect((panelX - 10, panelY + 225 + 12), (120, 50)), text = "object index", manager = manager)
tileValueInput = pygame_gui.elements.UITextEntryLine(relative_rect = pygame.Rect((panelX, panelY + 275), (120, 25)), manager = manager)

font = pygame.font.Font("../fonts/CascadiaMono.ttf", fontSize)

objInfo = ObjectInfo(manager, None)

currY = 0
currHeight = 2

mode = Mode.ROAM

lmd = False # left mouse button
rmd = False # right mouse button

offset_x = SCREEN_WIDTH/2
offset_y = SCREEN_HEIGHT/2

running = True
while running:
    time_delta = clock.tick(60)/1000.0
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        keys = pygame.key.get_pressed()

        if event.type == pygame.VIDEORESIZE:
            screen = pygame.display.set_mode((event.w, event.h), pygame.RESIZABLE)
            
        if event.type == pygame.KEYDOWN:
            if keys[pygame.K_F1]:
                mode = Mode.ROAM
            if keys[pygame.K_F2]:
                mode = Mode.PLACE
            if keys[pygame.K_F3]:
                mode = Mode.MOVE
            if keys[pygame.K_F4]:
                mode = Mode.SCALE
            if keys[pygame.K_F5]:
                mode = Mode.SELECT
                
            if keys[pygame.K_LEFT]:
                offset_x -= 10
            if keys[pygame.K_RIGHT]:
                offset_x += 10
            if keys[pygame.K_UP]:
                offset_y -= 10
            if keys[pygame.K_DOWN]:
                offset_y += 10

        if event.type == pygame.MOUSEBUTTONDOWN:
            if pygame.mouse.get_pressed()[0]:
                lmd = True
                if mode == Mode.PLACE:
                    objects.append(Object(mouseX - offset_x - 10, currY, mouseY - offset_y - 10, 20, currHeight, 20, 0, 0, 0, currentId))
                    
            if pygame.mouse.get_pressed()[2]:
                rmd = True
                if mode == Mode.MOVE:
                    for obj in objects:
                        if obj.projRec(offset_x, offset_y).collidepoint(mouseX, mouseY):
                            objects.remove(obj)

        if event.type == pygame.MOUSEBUTTONUP:
            if not pygame.mouse.get_pressed()[0]:
                lmd = False
            if not pygame.mouse.get_pressed()[2]:
                rmd = False
                
        if lmd:
            if mode == Mode.SCALE:
                for obj in objects:
                    if obj.projRec(offset_x, offset_y).collidepoint(mouseX, mouseY) and obj.y == currY:
                        obj.w = ((mouseX - offset_x) - obj.x)*2
                        obj.d = ((mouseY - offset_y) - obj.z)*2
            if mode == Mode.MOVE:
                for obj in objects:
                    if obj.projRec(offset_x, offset_y).collidepoint(mouseX, mouseY) and obj.y == currY:
                        obj.x = (mouseX - offset_x) - obj.w / 2
                        obj.z = (mouseY - offset_y) - obj.d / 2
                
        if event.type == pygame_gui.UI_TEXT_ENTRY_FINISHED:
            if event.ui_element == currYInput:
                currY = int(currYInput.text)
            if event.ui_element == currHeightInput:
                currHeight = int(currHeightInput.text)
            if event.ui_element == tileValueInput:
                currentId = int(tileValueInput.text)
                
        if event.type == pygame_gui.UI_BUTTON_PRESSED:
            if event.ui_element == save:
                saveToJson(objects, fileDialog(), SIZE_MULTIPLIER)
            if event.ui_element == clearBtn:
                objects.clear()
            if event.ui_element == load:
                objects = loadFromJson(fileDialog(), SIZE_MULTIPLIER)

        mouseX, mouseY = pygame.mouse.get_pos()

        manager.process_events(event)

    pygame.display.update()
    clock.tick(60)

    screen.fill("#282C34")

    for obj in objects:
        pygame.draw.rect(screen, RED, pygame.Rect(obj.x + offset_x, obj.z + offset_y, obj.w, obj.d))
        pygame.draw.rect(screen, BLACK, pygame.Rect(obj.x + offset_x, obj.z + offset_y, obj.w, obj.d), 1)
        drawText(screen, obj.id, obj.x + obj.w/2 + offset_x, obj.z + obj.d/2  + offset_y, font, WHITE, None)
        
    pygame.draw.line(screen, WHITE, (offset_x - 10, offset_y), (offset_x + 10, offset_y), 1)
    pygame.draw.line(screen, WHITE, (offset_x, offset_y - 10), (offset_x, offset_y + 10), 1)

    pygame.draw.rect(screen, "#32628A", panel)
    drawText(screen, mode, 10, 10, font, WHITE, None)
    drawText(screen, len(objects), 10, 35, font, WHITE, None)

    manager.draw_ui(screen)
    manager.update(time_delta)
    
pygame.quit()
quit()
