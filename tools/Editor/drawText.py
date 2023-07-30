import pygame

def drawText(screen, text, x, y, font, color, bgcolor):
    text = font.render(str(text), True, color, bgcolor)
    textRec = text.get_rect()
    textRec.center = (x, y)
    screen.blit(text, textRec)
