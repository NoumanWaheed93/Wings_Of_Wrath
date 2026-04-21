using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Game;
using SelectableEntitySystem;
using NSubstitute;
using AircraftController;
using CameraController;
public class ControllableAircraftManagerTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void ControllableAircraftManagerTestsSimplePasses()
    {
        // Use the Assert class to test conditions
        SelectableEntityManager selectableEntitySystem = new SelectableEntityManager();
        ControllableAircraftManager manager = new ControllableAircraftManager(selectableEntitySystem, 
            Substitute.For<AircraftPlayerController>(), 
            Substitute.For<UIControls>(), 
            Substitute.For<TopDownCamera>());
    }

}
