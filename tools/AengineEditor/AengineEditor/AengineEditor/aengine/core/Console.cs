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
    private string currentInput;
    private int maxLines;

    public Console()
    {
        window = new GuiWindow("Console", 10, 10, 500, 400);
        commandInput = new GuiTextBox();
        active = false;
        commands = new List<ConsoleCommand>();
        consoleLines = new List<string>();
        maxLines = 18;

        registerCommand(new ConsoleCommand("help", "lists every command and their description", ConsoleCommands.listCMDS));
        registerCommand(new ConsoleCommand("print", "prints text", ConsoleCommands.print));
        registerCommand(new ConsoleCommand("set_fps", "set fps of the game", ConsoleCommands.setFPS));
        registerCommand(new ConsoleCommand("window_maximize", "maximize window", ConsoleCommands.maximizeWindow));
        registerCommand(new ConsoleCommand("window_minimize", "minimize window", ConsoleCommands.minimizeWindow));
        registerCommand(new ConsoleCommand("clear", "clears the console window", ConsoleCommands.clear));
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

            if (Gui.GuiButton("enter", 400, window.rec.height - 42.5f, window.rec.width - 400 - Gui.exitScale, 40, window) || (commandInput.active && Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER)))
            {
                executeCommand(commandInput.text);
                commandInput.text = String.Empty;
            }
        }
    }
    
}