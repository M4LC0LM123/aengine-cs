using Raylib_CsLo;

namespace aengine.core;

public class ConsoleCommands
{
    public static void listCMDS(Console console, string[] args)
    {
        foreach (ConsoleCommand command in console.commands)
        {
            console.print($"{command.name} - {command.description}");
        }
    }

    public static void print(Console console, string[] args)
    {
        string str = console.commandInput.text.Remove(0, 6);
        console.print(str);
    }

    public static void setFPS(Console console, string[] args)
    {
        if (Int32.TryParse(console.commandInput.text.Remove(0, 8), out int fps))
        {
            console.print($"set fps to {fps}");
            Raylib.SetTargetFPS(fps);       
        }
        else
        {
            console.print(aengine.QUOTE + console.commandInput.text.Remove(0, 8) + aengine.QUOTE + "is not a number");
            console.print($"example usage of {aengine.QUOTE}set_fps{aengine.QUOTE}: set_fps 60");
        }
    }

    public static void maximizeWindow(Console console, string[] args)
    {
        Raylib.MaximizeWindow();
    }
    
    public static void minimizeWindow(Console console, string[] args)
    {
        Raylib.MinimizeWindow();
    }
    
}