using System;
using System.Collections.Generic;
using CommandSystem;

internal class FakeCommandable : ICommandable
{
    public string Name { get; }
    public List<ICommand> ReceivedCommands { get; } = new List<ICommand>();

    public FakeCommandable(string name = "Fake Commandable")
    {
        Name = name;
    }

    public void GiveCommand(ICommand command)
    {
        ReceivedCommands.Add(command);
        command.Execute(this);
    }
}

internal class FakeCommandTarget : ICommandTarget
{
    public string Name { get; }

    public FakeCommandTarget(string name = "Fake Target")
    {
        Name = name;
    }
}

internal class FakeCommand : ICommand
{
    public int ExecuteCount { get; private set; }
    public ICommandable LastExecutedOn { get; private set; }

    public void Execute(ICommandable commandable)
    {
        ExecuteCount++;
        LastExecutedOn = commandable;
    }
}

internal class FakeCommandDefinition : ICommandDefinition
{
    private readonly Func<ICommandTarget, ICommand> createCommand;

    public string Name { get; }
    public bool RequiresTarget { get; }
    public ICommandTarget LastCreateCommandTarget { get; private set; }

    public FakeCommandDefinition(string name, bool requiresTarget, Func<ICommandTarget, ICommand> createCommand)
    {
        Name = name;
        RequiresTarget = requiresTarget;
        this.createCommand = createCommand;
    }

    public ICommand CreateCommand(ICommandTarget target)
    {
        LastCreateCommandTarget = target;
        return createCommand(target);
    }
}

internal class FakeCommandPallette : ICommandPallette
{
    public IReadOnlyList<ICommandDefinition> Definitions { get; }

    public FakeCommandPallette(IReadOnlyList<ICommandDefinition> definitions)
    {
        Definitions = definitions;
    }
}
