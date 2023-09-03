using System.Numerics;
using Raylib_CsLo;
using Sandbox.aengine.Gui;

namespace aengine.core;

public class Console
{
    public GuiWindow window;
    public GuiTextBox commandInput;
    public bool active;
    public List<ConsoleCommand> commands;
    
    private List<string> consoleLines;
    private string previousInput;
    private int maxLines;

    public Console()
    {
        window = new GuiWindow("Console", 10, 10, 500, 400);
        commandInput = new GuiTextBox();
        active = false;
        commands = new List<ConsoleCommand>();
        consoleLines = new List<string>();
        maxLines = 18;
        
        previousInput = String.Empty;

        registerCommand(new ConsoleCommand("help", "lists every command and their description", ConsoleCommands.listCMDS));
        registerCommand(new ConsoleCommand("print", "prints text", ConsoleCommands.print));
        registerCommand(new ConsoleCommand("w_fps_set", "set fps of the game", ConsoleCommands.setFPS));
        registerCommand(new ConsoleCommand("w_maximize", "maximize window", ConsoleCommands.maximizeWindow));
        registerCommand(new ConsoleCommand("w_minimize", "minimize window", ConsoleCommands.minimizeWindow));
        registerCommand(new ConsoleCommand("clear", "clears the console window", ConsoleCommands.clear));
        registerCommand(new ConsoleCommand("f_add_body", "adds a spherical rigidbody", ConsoleCommands.newBody));
        registerCommand(new ConsoleCommand("w_width", "sets the width of window", ConsoleCommands.setWidth));
        registerCommand(new ConsoleCommand("w_height", "sets the height of window", ConsoleCommands.setHeight));
        registerCommand(new ConsoleCommand("w_close", "closes the window", ConsoleCommands.close));
    }

    public void clear()
    {
        consoleLines.Clear();
    }

    public void print(string str)
    {
        consoleLines.Add(">  " + str);

        if (consoleLines.Count > maxLines)
        {
            consoleLines.RemoveAt(0);
        }
    }

    public void registerCommand(ConsoleCommand command)
    {
        commands.Add(command);
    }

    public void executeCommand(string commandText)
    {
        string[] parts = commandText.Split(' ');
        string commandName = parts[0].ToLower();
        string[] args = parts.Length > 1 ? new Span<string>(parts).Slice(1).ToArray() : new string[0];

        ConsoleCommand command = commands.Find(cmd => cmd.name.ToLower() == commandName);
        if (command != null)
        {
            command.action(this, args);
        }
        else
        {
            print($"Command {aengine.QUOTE + commandText + aengine.QUOTE} is not a valid command!");
            print($"Use {aengine.QUOTE} help {aengine.QUOTE} for more details");
        }
    }

    public void render()
    {
        if (active)
        {
            window.render();
            
            Gui.GuiInverseRec(2.5f + window.rec.x, 2.5f + window.rec.y, window.rec.width - 5f, window.rec.height - 50);

            for (int i = 0; i < consoleLines.Count; i++)
            {
                Gui.GuiTextPro(Gui.font, consoleLines[i], new Vector2(5f, 2.5f + 17.5f * i), 17.5f, Raylib.WHITE, window);
            }
            
            commandInput.render(2.5f, window.rec.height - 42.5f, 400, 40, window);

            if (commandInput.active)
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                    commandInput.text = previousInput;

            if (Gui.GuiButton("enter", 400, window.rec.height - 42.5f, window.rec.width - 400 - Gui.exitScale, 40, window) || (commandInput.active && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER)))
            {
                executeCommand(commandInput.text);
                previousInput = commandInput.text;
                commandInput.text = String.Empty;
            }
        }
    }
    
}