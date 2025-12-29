using NUnit.Framework;
using WeaponSystem;
using NSubstitute;
using Common;
using UnityEngine;

public class RaycastGunTests : WeaponTests
{
    [SetUp]
    public override void SetUp()
    {
        base.SetUp();

        weapon = new GunRaycastBased(barrelTransform, Substitute.For<ITimeProvider>(), 100, 1, 10);
    }

    [Test]
    public void Gun_Instance_Can_Be_Created()
    {
        Assert.IsNotNull(weapon);
    }
}
