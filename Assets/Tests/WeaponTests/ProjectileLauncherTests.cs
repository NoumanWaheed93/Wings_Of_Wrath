using WeaponSystem;
using NSubstitute;
using NUnit.Framework;
using Common;
using UnityEngine;

public class ProjectileLauncherTests : WeaponTests
{
    [SetUp]
    public void SetUp()
    {
        weapon = new ProjectileLauncher(Substitute.For<Transform>(), Substitute.For<ITimeProvider>(), 100, 1, Substitute.For<IProjectileFactory>());
    }

    [Test]
    public void ProjectileLauncher_Instance_Can_Be_Created()
    {
        Assert.IsNotNull(weapon);
    }
}
