using NUnit.Framework;
using WeaponSystem;
using NSubstitute;
using Common;
using UnityEngine;

public class GuidedProjectileLauncherTests : WeaponTests
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        IProjectileFactory projectileFactory = Substitute.For<IProjectileFactory>();
        IHomingProjectile projectile = Substitute.For<IHomingProjectile>();
        projectile.Transform.Returns(projectileTransform);
        projectileFactory.GetHomingProjectile().Returns(projectile);
        weapon = new GuidedProjectileLauncher(barrelTransform, Substitute.For<ITimeProvider>(), 100, 1, projectileFactory);
    }

    [Test]
    public void Gun_Instance_Can_Be_Created()
    {
        Assert.IsNotNull(weapon);
    }

}
