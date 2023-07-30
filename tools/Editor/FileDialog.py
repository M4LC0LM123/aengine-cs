import tkinter as tk
from tkinter import filedialog

def fileDialog():
    root = tk.Tk()
    root.withdraw()  # Hide the main window

    file_path = filedialog.askopenfilename()
    if file_path:
        return file_path