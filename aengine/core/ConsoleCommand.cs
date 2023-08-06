namespace aengine.core;

public delegate void CommandAction(Console console, string[] args);

public class ConsoleCommand
{
    public string name { get; }
    public string description { get; }
    public CommandAction action { get; }

    public ConsoleCommand(string name, string description, CommandAction action)
    {
        this.name = name;
        this.description = description;
        this.action = action;
    }
}