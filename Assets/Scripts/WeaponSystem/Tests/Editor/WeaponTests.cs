using NUnit.Framework;
using UnityEngine;
using WeaponSystem;

public abstract class WeaponTests
{
    protected Transform projectileTransform;
    protected Transform barrelTransform;

    //The clock the weapon under test is built with. Tests move it forward.
    protected FakeTimeProvider time;

    protected Weapon weapon;

    [SetUp]
    public virtual void SetUp()
    {
        projectileTransform = (new GameObject("Test-Projectile-Transform")).transform;
        barrelTransform = (new GameObject("Test-barrel-Transform")).transform;
        time = new FakeTimeProvider();
    }

    [Test]
    public void Can_Fire_Weapon()
    {
        Assert.IsTrue(weapon.Fire());
    }

    [Test]
    public void Firing_Weapon_Lessens_Bullets()
    {
        weapon.Fire();
        Assert.AreEqual(99, weapon.RemainingAmmo);
    }

    [Test]
    public void Cannot_Fire_Between_Interval()
    {
        Assert.IsTrue(weapon.Fire(), "Could not First Fire");
        Assert.IsTrue(weapon.ShotInterval > 0, "Shot Interval should be greater than zero");
        time.Advance(weapon.ShotInterval / 2f);
        Assert.IsFalse(weapon.Fire(), "Could fire before interval");
    }

    [Test]
    public void Can_Fire_After_Interval()
    {
        Assert.IsTrue(weapon.Fire(), "Could not Fire First time");
        Assert.IsTrue(weapon.ShotInterval > 0, "Shot interval should be greater than zero");
        time.Advance(weapon.ShotInterval);
        Assert.IsTrue(weapon.Fire(), "Could not fire exactly after interval");
        time.Advance(weapon.ShotInterval + 1f);
        Assert.IsTrue(weapon.Fire(), "Could not fire 1 second after interval");
    }


    [TearDown]
    public virtual void TearDown()
    {
        GameObject.DestroyImmediate(barrelTransform.gameObject);
        GameObject.DestroyImmediate(projectileTransform.gameObject);
    }
}
