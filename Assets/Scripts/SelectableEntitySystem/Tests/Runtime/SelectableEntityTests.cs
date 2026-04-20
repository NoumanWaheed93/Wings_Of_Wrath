using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SelectableEntitySystem;

public class SelectableEntityTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void IsEntitySelectable()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        manager.AddSelectableEntity(entity);
        manager.OnEntitySelected += (selectedEntity) =>
        {
            Assert.AreEqual(entity, selectedEntity);
        };
        entity.Select();
    }

}
