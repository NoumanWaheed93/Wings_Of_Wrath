using NUnit.Framework;
using CommandSystem;

public class CommandTargetManagerTests
{
    [Test]
    public void AddTarget_AddsToTargetsCollection()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget target = new FakeCommandTarget();

        manager.AddTarget(target);

        CollectionAssert.Contains(manager.Targets, target);
    }

    [Test]
    public void AddTarget_InvokesOnTargetAdded()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget target = new FakeCommandTarget();
        bool eventFired = false;

        manager.OnTargetAdded += added =>
        {
            eventFired = true;
            Assert.AreEqual(target, added);
        };

        manager.AddTarget(target);

        Assert.IsTrue(eventFired, "OnTargetAdded should fire when a target is added");
    }

    [Test]
    public void RemoveTarget_WhenPresent_RemovesFromTargetsCollection()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget target = new FakeCommandTarget();
        manager.AddTarget(target);

        manager.RemoveTarget(target);

        CollectionAssert.DoesNotContain(manager.Targets, target);
    }

    [Test]
    public void RemoveTarget_WhenPresent_InvokesOnTargetRemoved()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget target = new FakeCommandTarget();
        manager.AddTarget(target);
        bool eventFired = false;

        manager.OnTargetRemoved += removed =>
        {
            eventFired = true;
            Assert.AreEqual(target, removed);
        };

        manager.RemoveTarget(target);

        Assert.IsTrue(eventFired, "OnTargetRemoved should fire when a present target is removed");
    }

    [Test]
    public void RemoveTarget_WhenNotPresent_DoesNotInvokeOnTargetRemoved()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget target = new FakeCommandTarget();
        bool eventFired = false;

        manager.OnTargetRemoved += _ => eventFired = true;

        manager.RemoveTarget(target);

        Assert.IsFalse(eventFired, "OnTargetRemoved should not fire for a target that was never added");
    }

    [Test]
    public void Targets_TracksMultipleAddedTargets()
    {
        CommandTargetManager manager = new CommandTargetManager();
        FakeCommandTarget a = new FakeCommandTarget("A");
        FakeCommandTarget b = new FakeCommandTarget("B");
        FakeCommandTarget c = new FakeCommandTarget("C");

        manager.AddTarget(a);
        manager.AddTarget(b);
        manager.AddTarget(c);

        Assert.AreEqual(3, manager.Targets.Count);
    }
}
