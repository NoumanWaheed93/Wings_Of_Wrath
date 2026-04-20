using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SelectableEntitySystem;

public class SelectableEntityTests
{
    #region SelectableEntity Tests

    [Test]
    public void SelectableEntity_OnSelectInvokesSelectEvent()
    {
        SelectableEntity entity = new SelectableEntity("Test Entity");
        bool eventFired = false;

        entity.OnSelect += (selectedEntity) =>
        {
            eventFired = true;
            Assert.AreEqual(entity, selectedEntity);
        };

        entity.Select();

        Assert.IsTrue(eventFired, "OnSelect event should fire when Select() is called");
    }

    [Test]
    public void SelectableEntity_OnDeselectInvokesDeselectEvent()
    {
        SelectableEntity entity = new SelectableEntity("Test Entity");
        bool eventFired = false;

        entity.OnDeselect += (deselectedEntity) =>
        {
            eventFired = true;
            Assert.AreEqual(entity, deselectedEntity);
        };

        entity.Deselect();

        Assert.IsTrue(eventFired, "OnDeselect event should fire when Deselect() is called");
    }

    [Test]
    public void SelectableEntity_NamePropertyIsSet()
    {
        string entityName = "Aircraft Alpha";
        SelectableEntity entity = new SelectableEntity(entityName);

        Assert.AreEqual(entityName, entity.Name, "Entity name should match constructor parameter");
    }

    [Test]
    public void SelectableEntity_MultipleSelectCallsInvokeEventMultipleTimes()
    {
        SelectableEntity entity = new SelectableEntity("Test Entity");
        int eventCount = 0;

        entity.OnSelect += (selectedEntity) =>
        {
            eventCount++;
        };

        entity.Select();
        entity.Select();
        entity.Select();

        Assert.AreEqual(3, eventCount, "OnSelect event should fire each time Select() is called");
    }

    #endregion

    #region SelectableEntityManager Tests

    [Test]
    public void SelectableEntityManager_AddSelectableEntityInvokesAddedEvent()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        bool eventFired = false;

        manager.OnSelectableEntityAdded += (addedEntity) =>
        {
            eventFired = true;
            Assert.AreEqual(entity, addedEntity);
        };

        manager.AddSelectableEntity(entity);

        Assert.IsTrue(eventFired, "OnSelectableEntityAdded event should fire when entity is added");
    }

    [Test]
    public void SelectableEntityManager_RemoveSelectableEntityInvokesRemovedEvent()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        manager.AddSelectableEntity(entity);
        bool eventFired = false;

        manager.OnSelectableEntityRemoved += (removedEntity) =>
        {
            eventFired = true;
            Assert.AreEqual(entity, removedEntity);
        };

        manager.RemoveSelectableEntity(entity);

        Assert.IsTrue(eventFired, "OnSelectableEntityRemoved event should fire when entity is removed");
    }

    [Test]
    public void SelectableEntityManager_SelectEntityInvokesSelectionEvent()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        manager.AddSelectableEntity(entity);
        bool eventFired = false;

        manager.OnEntitySelected += (selectedEntity) =>
        {
            eventFired = true;
            Assert.AreEqual(entity, selectedEntity);
        };

        manager.SelectEntity(entity);

        Assert.IsTrue(eventFired, "OnEntitySelected event should fire when entity is selected through manager");
    }

    [Test]
    public void SelectableEntityManager_SelectEntityNotInManagerDoesNotInvokeSelectionEvent()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        bool eventFired = false;

        manager.OnEntitySelected += (selectedEntity) =>
        {
            eventFired = true;
        };

        manager.SelectEntity(entity);

        Assert.IsFalse(eventFired, "OnEntitySelected event should NOT fire for entity not in manager");
    }

    [Test]
    public void SelectableEntityManager_SelectingNewEntityDeselectsOldEntity()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity1 = new SelectableEntity("Entity 1");
        SelectableEntity entity2 = new SelectableEntity("Entity 2");
        manager.AddSelectableEntity(entity1);
        manager.AddSelectableEntity(entity2);

        bool entity1DeselectFired = false;
        entity1.OnDeselect += (deselectedEntity) =>
        {
            entity1DeselectFired = true;
        };

        manager.SelectEntity(entity1);
        manager.SelectEntity(entity2);

        Assert.IsTrue(entity1DeselectFired, "Previous entity should be deselected when new entity is selected");
    }

    [Test]
    public void SelectableEntityManager_SelectingEntityInvokesEntitySelectEvent()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        manager.AddSelectableEntity(entity);
        bool entitySelectEventFired = false;

        entity.OnSelect += (selectedEntity) =>
        {
            entitySelectEventFired = true;
        };

        manager.SelectEntity(entity);

        Assert.IsTrue(entitySelectEventFired, "Entity's OnSelect event should fire when selected through manager");
    }

    [Test]
    public void SelectableEntityManager_AddMultipleEntities()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity1 = new SelectableEntity("Entity 1");
        SelectableEntity entity2 = new SelectableEntity("Entity 2");
        SelectableEntity entity3 = new SelectableEntity("Entity 3");

        int addedCount = 0;
        manager.OnSelectableEntityAdded += (addedEntity) =>
        {
            addedCount++;
        };

        manager.AddSelectableEntity(entity1);
        manager.AddSelectableEntity(entity2);
        manager.AddSelectableEntity(entity3);

        Assert.AreEqual(3, addedCount, "Should track all added entities");
    }

    [Test]
    public void SelectableEntityManager_SelectFirstThenSecondEntity()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity1 = new SelectableEntity("Entity 1");
        SelectableEntity entity2 = new SelectableEntity("Entity 2");
        manager.AddSelectableEntity(entity1);
        manager.AddSelectableEntity(entity2);

        int selectionCount = 0;
        manager.OnEntitySelected += (selectedEntity) =>
        {
            selectionCount++;
        };

        manager.SelectEntity(entity1);
        manager.SelectEntity(entity2);

        Assert.AreEqual(2, selectionCount, "Should fire selection event for each valid selection");
    }

    [Test]
    public void SelectableEntityManager_SelectSameEntityTwice()
    {
        SelectableEntityManager manager = new SelectableEntityManager();
        SelectableEntity entity = new SelectableEntity("Test Entity");
        manager.AddSelectableEntity(entity);

        int selectionCount = 0;
        manager.OnEntitySelected += (selectedEntity) =>
        {
            selectionCount++;
        };

        manager.SelectEntity(entity);
        manager.SelectEntity(entity);

        Assert.AreEqual(2, selectionCount, "Should allow re-selecting the same entity");
    }

    #endregion

}
