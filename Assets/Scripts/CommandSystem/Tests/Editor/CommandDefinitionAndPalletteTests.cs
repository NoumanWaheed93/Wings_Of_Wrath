using System.Collections.Generic;
using NUnit.Framework;
using CommandSystem;

public class CommandDefinitionAndPalletteTests
{
    [Test]
    public void CreateCommand_PassesTheGivenTargetThrough()
    {
        FakeCommandTarget target = new FakeCommandTarget();
        FakeCommandDefinition definition = new FakeCommandDefinition("Attack", true, t => new FakeCommand());

        definition.CreateCommand(target);

        Assert.AreEqual(target, definition.LastCreateCommandTarget, "CreateCommand should receive the target it was called with");
    }

    [Test]
    public void CreateCommand_WhenCommandDoesNotRequireTarget_CanBeCalledWithNullTarget()
    {
        FakeCommandDefinition definition = new FakeCommandDefinition("Break Formation", false, t => new FakeCommand());

        ICommand command = definition.CreateCommand(null);

        Assert.IsNotNull(command, "A command not requiring a target should still be created when target is null");
    }

    [Test]
    public void CommandPallette_DefinitionsExposesEveryRegisteredDefinition()
    {
        FakeCommandDefinition attack = new FakeCommandDefinition("Attack", true, t => new FakeCommand());
        FakeCommandDefinition breakFormation = new FakeCommandDefinition("Break Formation", false, t => new FakeCommand());
        FakeCommandPallette pallette = new FakeCommandPallette(new List<ICommandDefinition> { attack, breakFormation });

        Assert.AreEqual(2, pallette.Definitions.Count);
        CollectionAssert.Contains(pallette.Definitions, attack);
        CollectionAssert.Contains(pallette.Definitions, breakFormation);
    }

    [Test]
    public void FullFlow_DefinitionCreatesCommand_ManagerDispatchesIt_CommandExecutesOnSelectedCommandable()
    {
        // Arrange: one selected commandable, one target, and a definition whose command
        // captures whatever target it was built with - mirrors the real Attack command flow.
        CommandManager commandManager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable("Aircraft");
        FakeCommandTarget target = new FakeCommandTarget("Enemy Aircraft");
        FakeCommand command = new FakeCommand();
        FakeCommandDefinition attackDefinition = new FakeCommandDefinition("Attack", true, t => command);

        commandManager.AddCommandable(commandable);
        commandManager.ToggleCommandableSelection(commandable);

        // Act: mirrors UI_CommandFlowPresenter's CreateCommand(target) -> GiveCommandToSelected(...) flow.
        ICommand createdCommand = attackDefinition.CreateCommand(target);
        commandManager.GiveCommandToSelected(createdCommand);

        // Assert
        Assert.AreEqual(target, attackDefinition.LastCreateCommandTarget);
        Assert.AreEqual(1, commandable.ReceivedCommands.Count);
        Assert.AreEqual(command, commandable.ReceivedCommands[0]);
        Assert.AreEqual(1, command.ExecuteCount);
        Assert.AreEqual(commandable, command.LastExecutedOn);
    }
}
