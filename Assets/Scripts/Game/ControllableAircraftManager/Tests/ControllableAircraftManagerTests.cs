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

public class ControllableAircraftManagerTests
{
    private SelectableEntityManager selectableEntityManager;
    private AircraftPlayerController playerController;
    private UIControls uiControls;
    private TopDownCamera cameraController;
    private ControllableAircraftManager manager;

    [SetUp]
    public void Setup()
    {
        selectableEntityManager = new SelectableEntityManager();
        IAircraftPlayerInputManager thumbDrift = Substitute.For<IAircraftPlayerInputManager>();
        playerController = new AircraftPlayerController(thumbDrift);
        GameObject uiControlsGO = new GameObject("UIControls");
        uiControls = uiControlsGO.AddComponent<UIControls>();
        GameObject cameraGO = new GameObject("Camera");
        cameraController = cameraGO.AddComponent<TopDownCamera>();

        manager = new ControllableAircraftManager(selectableEntityManager, playerController, uiControls, cameraController);
    }

    [Test]
    public void ControllableAircraftManager_Constructor_Initializes_Successfully()
    {
        // Arrange & Act - Already done in Setup

        // Assert - Constructor completes without throwing
        Assert.IsNotNull(manager);
    }

    [Test]
    public void AddControllableAircraft_Adds_Aircraft_To_Manager()
    {
        // Arrange
        var mockAircraft = Substitute.For<IAircraft>();
        var mockTransform = new GameObject().transform;
        var aiController = new AircraftAIController(mockAircraft, mockTransform);

        // Act
        manager.AddControllableAircraft(aiController);

        // Assert
        Assert.AreEqual(1, selectableEntityManager.SelectableEntities.Count);
    }

    [Test]
    public void AddControllableAircraft_Creates_SelectableEntity_For_Aircraft()
    {
        // Arrange
        var mockAircraft = Substitute.For<IAircraft>();
        var mockTransform = new GameObject().transform;
        var aiController = new AircraftAIController(mockAircraft, mockTransform);

        // Act
        manager.AddControllableAircraft(aiController);

        // Assert
        Assert.AreEqual(1, selectableEntityManager.SelectableEntities.Count);
        Assert.AreEqual("aircraft", selectableEntityManager.SelectableEntities[0].Name);
    }

    [Test]
    public void AddControllableAircraft_Multiple_Aircraft_Are_Added_Correctly()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockAircraft3 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;
        var mockTransform3 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);
        var aiController3 = new AircraftAIController(mockAircraft3, mockTransform3);

        // Act
        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);
        manager.AddControllableAircraft(aiController3);

        // Assert
        Assert.AreEqual(3, selectableEntityManager.SelectableEntities.Count);
    }

    [Test]
    public void OnEntitySelected_Transfers_Control_From_AI_To_Player()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);

        // Set first aircraft as player controlled
        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        // Get the second aircraft's selectable entity
        var entities = selectableEntityManager.SelectableEntities;
        var secondAircraftEntity = entities[1]; // Second entity is for second aircraft

        // Act - Select the second aircraft
        selectableEntityManager.SelectEntity(secondAircraftEntity);

        // Assert
        // After selection, first aircraft should be returned to AI control
        Assert.AreEqual(mockAircraft2, playerController.Aircraft);
        Assert.IsTrue(aiController1.IsActive, "First aircraft AI should be active");
        Assert.IsFalse(aiController2.IsActive, "Second aircraft AI should be inactive");

        // Verify camera target was updated
        cameraController.Received(1).Target = mockTransform2;
    }

    [Test]
    public void OnEntitySelected_Returns_Previous_Aircraft_To_AI_Control()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);

        // Set first aircraft as player controlled
        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        var entities = selectableEntityManager.SelectableEntities;
        var secondAircraftEntity = entities[1];

        // Act - Select second aircraft
        selectableEntityManager.SelectEntity(secondAircraftEntity);

        // Assert - First aircraft should now be controlled by AI
        Assert.IsTrue(aiController1.IsActive, "Previous aircraft AI should be reactivated");
    }

    [Test]
    public void OnEntitySelected_Sets_Camera_Target_To_Selected_Aircraft()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);

        // Setup aircraft mocks to return transforms
        mockAircraft1.Transform.Returns(mockTransform1);
        mockAircraft2.Transform.Returns(mockTransform2);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        var entities = selectableEntityManager.SelectableEntities;
        var secondAircraftEntity = entities[1];

        // Act
        selectableEntityManager.SelectEntity(secondAircraftEntity);

        // Assert
        cameraController.Received(1).Target = mockTransform2;
    }

    [Test]
    public void OnEntitySelected_Updates_UI_With_New_Aircraft_Transform()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;
        var mockMeshRenderer = new GameObject().AddComponent<MeshRenderer>();
        var mockMissileLauncher = new GameObject().AddComponent<GuidedProjectileLauncherMonobehaviour>();
        var mockGun = new GameObject().AddComponent<RaycastGunMonobehaviourDemo>();

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);

        mockAircraft1.Transform.Returns(mockTransform1);
        mockAircraft2.Transform.Returns(mockTransform2);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        cameraController.Target.GetComponentInChildren<MeshRenderer>().Returns(mockMeshRenderer);
        cameraController.Target.GetComponentInChildren<GuidedProjectileLauncherMonobehaviour>().Returns(mockMissileLauncher);
        cameraController.Target.GetComponentInChildren<RaycastGunMonobehaviourDemo>().Returns(mockGun);

        var entities = selectableEntityManager.SelectableEntities;
        var secondAircraftEntity = entities[1];

        // Act
        selectableEntityManager.SelectEntity(secondAircraftEntity);

        // Assert
        uiControls.Received(1).SetPlayer(mockTransform2);
    }

    [Test]
    public void OnEntitySelected_Handles_Switching_Between_Multiple_Aircraft()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockAircraft3 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;
        var mockTransform3 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);
        var aiController3 = new AircraftAIController(mockAircraft3, mockTransform3);

        mockAircraft1.Transform.Returns(mockTransform1);
        mockAircraft2.Transform.Returns(mockTransform2);
        mockAircraft3.Transform.Returns(mockTransform3);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);
        manager.AddControllableAircraft(aiController3);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Switch to aircraft 2
        selectableEntityManager.SelectEntity(entities[1]);
        Assert.AreEqual(mockAircraft2, playerController.Aircraft);
        Assert.IsTrue(aiController1.IsActive);
        Assert.IsFalse(aiController2.IsActive);

        // Act - Switch to aircraft 3
        selectableEntityManager.SelectEntity(entities[2]);

        // Assert
        Assert.AreEqual(mockAircraft3, playerController.Aircraft);
        Assert.IsFalse(aiController1.IsActive);
        Assert.IsTrue(aiController2.IsActive);
        Assert.IsFalse(aiController3.IsActive);
    }

    [Test]
    public void OnEntitySelected_Clears_Player_Aircraft_When_Returning_To_AI()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockAircraft2 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;
        var mockTransform2 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);
        var aiController2 = new AircraftAIController(mockAircraft2, mockTransform2);

        mockAircraft1.Transform.Returns(mockTransform1);
        mockAircraft2.Transform.Returns(mockTransform2);

        manager.AddControllableAircraft(aiController1);
        manager.AddControllableAircraft(aiController2);

        playerController.Aircraft = mockAircraft1;
        aiController1.IsActive = false;
        aiController2.IsActive = true;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Switch from aircraft 1 to aircraft 2
        selectableEntityManager.SelectEntity(entities[1]);

        // Assert - Aircraft 1 should be cleared from player and returned to AI
        // (verified by checking that first aircraft AI is active again)
        Assert.IsTrue(aiController1.IsActive);
    }

    [Test]
    public void OnEntitySelected_With_No_Previous_Player_Aircraft_Assigns_Correctly()
    {
        // Arrange
        var mockAircraft1 = Substitute.For<IAircraft>();
        var mockTransform1 = new GameObject().transform;

        var aiController1 = new AircraftAIController(mockAircraft1, mockTransform1);

        mockAircraft1.Transform.Returns(mockTransform1);

        manager.AddControllableAircraft(aiController1);

        // Start with no player aircraft
        playerController.Aircraft = null;

        var entities = selectableEntityManager.SelectableEntities;

        // Act - Select first aircraft
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
        var mockTransforms = new List<Transform>();
        var aiControllers = new List<AircraftAIController>();

        for (int i = 0; i < aircraftCount; i++)
        {
            mockAircrafts.Add(Substitute.For<IAircraft>());
            mockTransforms.Add(new GameObject().transform);
            var controller = new AircraftAIController(mockAircrafts[i], mockTransforms[i]);
            aiControllers.Add(controller);
            mockAircrafts[i].Transform.Returns(mockTransforms[i]);
        }

        // Act
        foreach (var controller in aiControllers)
        {
            manager.AddControllableAircraft(controller);
        }

        // Assert
        Assert.AreEqual(aircraftCount, selectableEntityManager.SelectableEntities.Count);
    }

}
