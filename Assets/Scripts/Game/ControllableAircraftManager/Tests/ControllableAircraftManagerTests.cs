using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Game;
using SelectableEntitySystem;
using NSubstitute;
using AircraftController;
using AircraftController.AircraftAI;
using CameraController;
using ScreenInputControls;
using WeaponSystem;
using Common;
using FormationSystem;

public class ControllableAircraftManagerTests
{
    private SelectableEntityManager selectableEntityManager;
    private AircraftPlayerController playerController;
    private ControllableAircraftManager manager;

    private Formation<AircraftFormationMember> CreateMockFormation(IAircraft leaderAircraft)
    {
        var formation = new Trail<AircraftFormationMember>();
        var leaderMember = new AircraftFormationMember { aircraft = leaderAircraft };
        formation.AddMember(leaderMember);
        return formation;
    }

    [SetUp]
    public void Setup()
    {
        selectableEntityManager = new SelectableEntityManager();
        IAircraftPlayerInputManager thumbDrift = Substitute.For<IAircraftPlayerInputManager>();
        playerController = new AircraftPlayerController(thumbDrift);

        manager = new ControllableAircraftManager(selectableEntityManager, playerController, new ITargetTransformReceiver[0]);
    }

    [Test]
    public void ControllableAircraftManager_Constructor_Initializes_Successfully()
    {
        // Arrange & Act - Already done in Setup

        // Assert - Constructor completes without throwing
        Assert.IsNotNull(manager);
    }

    [Test]
    public void AddControllableFormation_Adds_Formation_To_Manager()
    {
        // Arrange
        var mockAircraft = Substitute.For<IAircraft>();
        var formation = CreateMockFormation(mockAircraft);

        // Act
        manager.AddControllableFormation(formation);

        // Assert
        Assert.AreEqual(1, selectableEntityManager.SelectableEntities.Count);
    }

    [Test]
    public void AddControllableFormation_Creates_SelectableEntity_For_Formation()
    {
        // Arrange
        var mockAircraft = Substitute.For<IAircraft>();
        var formation = CreateMockFormation(mockAircraft);

        // Act
        manager.AddControllableFormation(formation);

        // Assert
        Assert.AreEqual(1, selectableEntityManager.SelectableEntities.Count);
        Assert.AreEqual("formation", selectableEntityManager.SelectableEntities[0].Name);
    }

    [Test]
    public void AddControllableFormation_Multiple_Formations_Are_Added_Correctly()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockAircraft3 = Substitute.For<IAircraft>();

        var formation1 = CreateMockFormation(mockAircraft1);
        var formation2 = CreateMockFormation(mockAircraft2);
        var formation3 = CreateMockFormation(mockAircraft3);

        // Act
        manager.AddControllableFormation(formation1);
        manager.AddControllableFormation(formation2);
        manager.AddControllableFormation(formation3);

        // Assert
        Assert.AreEqual(3, selectableEntityManager.SelectableEntities.Count);
    }

    [Test]
    public void OnEntitySelected_Transfers_Control_From_AI_To_Player()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();

        var aiController1 = Substitute.For<IAircraftController>();
        var aiController2 = Substitute.For<IAircraftController>();

        manager.AddControllableAircraft(mockAircraft1, aiController1, null, null, null);
        manager.AddControllableAircraft(mockAircraft2, aiController2, null, null, null);

        var formation1 = CreateMockFormation(mockAircraft1);
        var formation2 = CreateMockFormation(mockAircraft2);
        manager.AddControllableFormation(formation1);
        manager.AddControllableFormation(formation2);

        // Set first aircraft as player controlled
        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        // Get the second formation's selectable entity
        var entities = selectableEntityManager.SelectableEntities;
        var secondFormationEntity = entities[1];

        // Act - Select the second formation
        selectableEntityManager.SelectEntity(secondFormationEntity);

        // Assert
        Assert.AreEqual(mockAircraft2, playerController.Aircraft);
        Assert.IsTrue(aiController1.IsActive, "First aircraft AI should be active");
        Assert.IsFalse(aiController2.IsActive, "Second aircraft AI should be inactive");
    }

    [Test]
    public void OnEntitySelected_Returns_Previous_Aircraft_To_AI_Control()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();

        var aiController1 = Substitute.For<IAircraftController>();
        var aiController2 = Substitute.For<IAircraftController>();

        manager.AddControllableAircraft(mockAircraft1, aiController1, null, null, null);
        manager.AddControllableAircraft(mockAircraft2, aiController2, null, null, null);

        var formation1 = CreateMockFormation(mockAircraft1);
        var formation2 = CreateMockFormation(mockAircraft2);
        manager.AddControllableFormation(formation1);
        manager.AddControllableFormation(formation2);

        // Set first aircraft as player controlled
        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        var entities = selectableEntityManager.SelectableEntities;
        var secondFormationEntity = entities[1];

        // Act - Select second formation
        selectableEntityManager.SelectEntity(secondFormationEntity);

        // Assert - First aircraft should now be controlled by AI
        Assert.IsTrue(aiController1.IsActive, "Previous aircraft AI should be reactivated");
    }

    [Test]
    public void OnEntitySelected_Handles_Switching_Between_Multiple_Formations()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockAircraft3 = Substitute.For<IAircraft>();

        var aiController1 = Substitute.For<IAircraftController>();
        var aiController2 = Substitute.For<IAircraftController>();
        var aiController3 = Substitute.For<IAircraftController>();

        manager.AddControllableAircraft(mockAircraft1, aiController1, null, null, null);
        manager.AddControllableAircraft(mockAircraft2, aiController2, null, null, null);
        manager.AddControllableAircraft(mockAircraft3, aiController3, null, null, null);

        var formation1 = CreateMockFormation(mockAircraft1);
        var formation2 = CreateMockFormation(mockAircraft2);
        var formation3 = CreateMockFormation(mockAircraft3);
        manager.AddControllableFormation(formation1);
        manager.AddControllableFormation(formation2);
        manager.AddControllableFormation(formation3);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Switch to formation 2
        selectableEntityManager.SelectEntity(entities[1]);
        Assert.AreEqual(mockAircraft2, playerController.Aircraft);
        Assert.IsTrue(aiController1.IsActive);
        Assert.IsFalse(aiController2.IsActive);

        // Act - Switch to formation 3
        selectableEntityManager.SelectEntity(entities[2]);

        // Assert
        Assert.AreEqual(mockAircraft3, playerController.Aircraft);
        Assert.IsTrue(aiController1.IsActive);
        Assert.IsTrue(aiController2.IsActive);
        Assert.IsFalse(aiController3.IsActive);
    }

    [Test]
    public void OnEntitySelected_Clears_Player_Aircraft_When_Returning_To_AI()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();

        var aiController1 = Substitute.For<IAircraftController>();
        var aiController2 = Substitute.For<IAircraftController>();

        manager.AddControllableAircraft(mockAircraft1, aiController1, null, null, null);
        manager.AddControllableAircraft(mockAircraft2, aiController2, null, null, null);

        var formation1 = CreateMockFormation(mockAircraft1);
        var formation2 = CreateMockFormation(mockAircraft2);
        manager.AddControllableFormation(formation1);
        manager.AddControllableFormation(formation2);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Switch from formation 1 to formation 2
        selectableEntityManager.SelectEntity(entities[1]);

        // Assert - Aircraft 1 should be cleared from player and returned to AI
        Assert.IsTrue(aiController1.IsActive);
    }

    [Test]
    public void OnEntitySelected_With_No_Previous_Player_Aircraft_Assigns_Correctly()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var aiController1 = Substitute.For<IAircraftController>();

        manager.AddControllableAircraft(mockAircraft1, aiController1, null, null, null);

        var formation1 = CreateMockFormation(mockAircraft1);
        manager.AddControllableFormation(formation1);

        // Start with no player aircraft
        playerController.Aircraft = null;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Select first formation
        selectableEntityManager.SelectEntity(entities[0]);

        // Assert - Aircraft should be assigned to player and AI should be inactive
        Assert.AreEqual(mockAircraft1, playerController.Aircraft);
        Assert.IsFalse(aiController1.IsActive);
    }

    [Test]
    public void Manager_Tracks_All_Added_Aircraft_Correctly()
    {
        // Arrange
        var aircraftCount = 5;
        var mockAircrafts = new List<IAircraft>();
        var aiControllers = new List<IAircraftController>();

        for (int i = 0; i < aircraftCount; i++)
        {
            mockAircrafts.Add(Substitute.For<IAircraft>());
            var controller = Substitute.For<IAircraftController>();
            aiControllers.Add(controller);
            manager.AddControllableAircraft(mockAircrafts[i], aiControllers[i], null, null, null);
        }

        var formations = new List<Formation<AircraftFormationMember>>();

        // Act
        for (int i = 0; i < aircraftCount; i++)
        {
            var formation = CreateMockFormation(mockAircrafts[i]);
            formations.Add(formation);
            manager.AddControllableFormation(formation);
        }

        // Assert
        Assert.AreEqual(aircraftCount, selectableEntityManager.SelectableEntities.Count);
    }

}
