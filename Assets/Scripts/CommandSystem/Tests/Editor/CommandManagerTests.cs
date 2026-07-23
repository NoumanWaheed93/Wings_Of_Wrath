using NUnit.Framework;
using CommandSystem;

public class CommandManagerTests
{
    #region AddCommandable / RemoveCommandable

    [Test]
    public void AddCommandable_InvokesOnCommandableAdded()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        bool eventFired = false;

        manager.OnCommandableAdded += added =>
        {
            eventFired = true;
            Assert.AreEqual(commandable, added);
        };

        manager.AddCommandable(commandable);

        Assert.IsTrue(eventFired, "OnCommandableAdded should fire when a commandable is added");
    }

    [Test]
    public void AddCommandable_AddsToCommandablesCollection()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();

        manager.AddCommandable(commandable);

        CollectionAssert.Contains(manager.Commandables, commandable);
    }

    [Test]
    public void RemoveCommandable_InvokesOnCommandableRemoved()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        bool eventFired = false;

        manager.OnCommandableRemoved += removed =>
        {
            eventFired = true;
            Assert.AreEqual(commandable, removed);
        };

        manager.RemoveCommandable(commandable);

        Assert.IsTrue(eventFired, "OnCommandableRemoved should fire when a commandable is removed");
    }

    [Test]
    public void RemoveCommandable_RemovesFromCommandablesCollection()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);

        manager.RemoveCommandable(commandable);

        CollectionAssert.DoesNotContain(manager.Commandables, commandable);
    }

    [Test]
    public void RemoveCommandable_WhenSelected_AlsoInvokesOnCommandableDeselected()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        manager.ToggleCommandableSelection(commandable);
        bool deselectedFired = false;

        manager.OnCommandableDeselected += _ => deselectedFired = true;

        manager.RemoveCommandable(commandable);

        Assert.IsTrue(deselectedFired, "Removing a selected commandable should also deselect it");
    }

    [Test]
    public void RemoveCommandable_WhenNotSelected_DoesNotInvokeOnCommandableDeselected()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        bool deselectedFired = false;

        manager.OnCommandableDeselected += _ => deselectedFired = true;

        manager.RemoveCommandable(commandable);

        Assert.IsFalse(deselectedFired, "Removing a commandable that was never selected should not fire deselect");
    }

    #endregion

    #region ToggleCommandableSelection

    [Test]
    public void ToggleCommandableSelection_FirstToggle_SelectsAndReturnsTrue()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);

        bool result = manager.ToggleCommandableSelection(commandable);

        Assert.IsTrue(result, "First toggle should select the commandable");
        CollectionAssert.Contains(manager.SelectedCommandables, commandable);
    }

    [Test]
    public void ToggleCommandableSelection_FirstToggle_InvokesOnCommandableSelected()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        bool eventFired = false;

        manager.OnCommandableSelected += _ => eventFired = true;

        manager.ToggleCommandableSelection(commandable);

        Assert.IsTrue(eventFired, "OnCommandableSelected should fire on first toggle");
    }

    [Test]
    public void ToggleCommandableSelection_SecondToggle_DeselectsAndReturnsFalse()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        manager.ToggleCommandableSelection(commandable);

        bool result = manager.ToggleCommandableSelection(commandable);

        Assert.IsFalse(result, "Second toggle should deselect the commandable");
        CollectionAssert.DoesNotContain(manager.SelectedCommandables, commandable);
    }

    [Test]
    public void ToggleCommandableSelection_SecondToggle_InvokesOnCommandableDeselected()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable commandable = new FakeCommandable();
        manager.AddCommandable(commandable);
        manager.ToggleCommandableSelection(commandable);
        bool eventFired = false;

        manager.OnCommandableDeselected += _ => eventFired = true;

        manager.ToggleCommandableSelection(commandable);

        Assert.IsTrue(eventFired, "OnCommandableDeselected should fire on second toggle");
    }

    #endregion

    #region ToggleAllCommandableSelection

    [Test]
    public void ToggleAllCommandableSelection_WhenNoneSelected_SelectsAllAndReturnsTrue()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable a = new FakeCommandable("A");
        FakeCommandable b = new FakeCommandable("B");
        manager.AddCommandable(a);
        manager.AddCommandable(b);

        bool result = manager.ToggleAllCommandableSelection();

        Assert.IsTrue(result, "Should select all when none are selected");
        Assert.AreEqual(2, manager.SelectedCommandables.Count);
    }

    [Test]
    public void ToggleAllCommandableSelection_WhenAllSelected_DeselectsAllAndReturnsFalse()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable a = new FakeCommandable("A");
        FakeCommandable b = new FakeCommandable("B");
        manager.AddCommandable(a);
        manager.AddCommandable(b);
        manager.ToggleAllCommandableSelection();

        bool result = manager.ToggleAllCommandableSelection();

        Assert.IsFalse(result, "Should deselect all when all are already selected");
        Assert.AreEqual(0, manager.SelectedCommandables.Count);
    }

    [Test]
    public void ToggleAllCommandableSelection_WhenPartiallySelected_SelectsAllAndReturnsTrue()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable a = new FakeCommandable("A");
        FakeCommandable b = new FakeCommandable("B");
        manager.AddCommandable(a);
        manager.AddCommandable(b);
        manager.ToggleCommandableSelection(a);

        bool result = manager.ToggleAllCommandableSelection();

        Assert.IsTrue(result, "Partial selection should select the rest rather than clearing");
        Assert.AreEqual(2, manager.SelectedCommandables.Count);
    }

    #endregion

    #region GiveCommandToSelected

    [Test]
    public void GiveCommandToSelected_OnlySelectedCommandablesReceiveTheCommand()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable selected = new FakeCommandable("Selected");
        FakeCommandable unselected = new FakeCommandable("Unselected");
        manager.AddCommandable(selected);
        manager.AddCommandable(unselected);
        manager.ToggleCommandableSelection(selected);
        FakeCommand command = new FakeCommand();

        manager.GiveCommandToSelected(command);

        Assert.AreEqual(1, selected.ReceivedCommands.Count, "Selected commandable should receive the command");
        Assert.AreEqual(0, unselected.ReceivedCommands.Count, "Unselected commandable should not receive the command");
    }

    [Test]
    public void GiveCommandToSelected_ExecutesCommandOnEachSelectedCommandable()
    {
        CommandManager manager = new CommandManager();
        FakeCommandable a = new FakeCommandable("A");
        FakeCommandable b = new FakeCommandable("B");
        manager.AddCommandable(a);
        manager.AddCommandable(b);
        manager.ToggleAllCommandableSelection();
        FakeCommand command = new FakeCommand();

        manager.GiveCommandToSelected(command);

        Assert.AreEqual(2, command.ExecuteCount, "Command should be executed once per selected commandable");
    }

    #endregion
}
