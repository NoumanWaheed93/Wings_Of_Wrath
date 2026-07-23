using NUnit.Framework;
using UnityEngine;
using AircraftController;
using NSubstitute;
using Locomotion;
using Assert = UnityEngine.Assertions.Assert;
using Common;

public class AircraftControllerTests
{

    private AircraftController.Aircraft aircraftController;
    private GameObject aircraftGameObject;

    [SetUp]
    public void SetUp()
    {
        aircraftGameObject = new GameObject();
        GameObject aircraftModelGO = new GameObject();
        aircraftModelGO.transform.parent = aircraftGameObject.transform;
        Rigidbody rigidbody = aircraftGameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        
        aircraftController =
            new AircraftController.Aircraft(ScriptableObject.CreateInstance<AircraftMovementData>(), aircraftGameObject.transform, rigidbody);
        aircraftController.Spawn();
    }

    [Test]
    public void AircraftStateTestsSimplePasses()
    {
        AircraftStateMachine stateMachine = new AircraftStateMachine();
        AircraftState stateA = Substitute.For<AircraftState>(stateMachine, null);
        AircraftState stateB = Substitute.For<AircraftState>(stateMachine, null);
        stateMachine.ChangeState(stateA);
        Assert.AreEqual(stateA, stateMachine.currentState, "Initial state is incorrect.");
        stateMachine.ChangeState(stateB);
        Assert.AreEqual(stateB, stateMachine.currentState, "State does not change correctly.");
    }

    [Test]
    public void State_Changes_To_TakeOff_After_Speed_Goes_Above_TakeOffSpeed()
    {
        aircraftController.MovementHandler.SetThrottle(1);

        float startSpeed = aircraftController.MovementHandler.CurrSpeed;
        int tickCounter = 0;
        while (aircraftController.MovementHandler.CurrSpeed <= aircraftController.MovementHandler.AerodynamicMovementData.takeOffSpeed)
        {
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            tickCounter++;
            if(tickCounter > 20 && startSpeed == aircraftController.MovementHandler.CurrSpeed)
            {
                Assert.IsTrue(false, "Aircraft speed did not increase over time.");
                return;
            }
        }

        aircraftController.Update(1);

        Assert.AreEqual(aircraftController.StateTakeOff, aircraftController.StateMachine.currentState);
    }

    [Test]
    public void State_Changes_To_InAir_After_Climbing_Above_AirborneAltitude()
    {
        aircraftController.StateMachine.ChangeState(aircraftController.StateTakeOff);

        aircraftGameObject.transform.position = new Vector3(0, GlobalAircraftControllerSettings.airborneAltitude, 0);
        aircraftController.Update(1);
        
        Assert.AreEqual(aircraftController.StateInAir, aircraftController.StateMachine.currentState);
    }

    [Test]
    public void Deviation_From_A_Line_Can_Be_Measured()
    {
        aircraftGameObject.transform.position = Vector3.one * 1.5f;
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, Vector3.one, 1));


        aircraftGameObject.transform.position = Vector3.one * 1.6f;
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, Vector3.one, 1));

        aircraftGameObject.transform.position = new Vector3(0f, 1f, 0f);
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, Vector3.one, 1));

        aircraftGameObject.transform.position = new Vector3(-0.2f, 1.2f, 0);
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, Vector3.one, 1));

        aircraftGameObject.transform.position = new Vector3(0.4f, 0.9f, 0);
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(1, 0, 0), 1));

        aircraftGameObject.transform.position = new Vector3(0.4f, 1.1f, 0);
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(1, 0, 0), 1));
        
        aircraftGameObject.transform.position = new Vector3(0.4f, -0.9f, 0);
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(1, 0, 0), 1));

        aircraftGameObject.transform.position = new Vector3(0.4f, -1.1f, 0);
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(1, 0, 0), 1));

        ////
        aircraftGameObject.transform.position = new Vector3(0.9f, 0.2f, 0);
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(0, 1, 0), 1));

        aircraftGameObject.transform.position = new Vector3(1.1f, 0.8f, 0);
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(0, 1, 0), 1));

        aircraftGameObject.transform.position = new Vector3(-0.9f, 0.6f, 0);
        Assert.IsFalse(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(0, 1, 0), 1));

        aircraftGameObject.transform.position = new Vector3(-1.1f, 0.2f, 0);
        Assert.IsTrue(aircraftController.HasDeviatedFromLine(Vector3.zero, new Vector3(0, 1, 0), 1));

    }

    [Test]
    public void State_Changes_To_Landed_After_Completing_Landing_Sequence()
    {
        // Create runway approach points
        GameObject initialApproachGO = new GameObject();
        GameObject finalApproachGO = new GameObject();
        GameObject touchDownPointGO = new GameObject();

        GameObject runwayGO = new GameObject();

        initialApproachGO.transform.position = new Vector3(0, 0, 300);
        finalApproachGO.transform.position = new Vector3(0, 0, 200);
        touchDownPointGO.transform.position = new Vector3(0, 0, 100);

        Runway runway = runwayGO.AddComponent<Runway>();
        runway.Init(Team.Blue, initialApproachGO.transform, finalApproachGO.transform, touchDownPointGO.transform);

        // Spawn aircraft in air
        aircraftController.Spawn(isInAir: true);
        aircraftGameObject.transform.position = Vector3.zero;

        // Set runway to trigger landing
        aircraftController.RunwayInUse = runway;

        // Move toward InitialApproach to satisfy proximity check, then transition to FinalApproach
        int ticks = 0;
        aircraftGameObject.transform.position = initialApproachGO.transform.position;
        aircraftController.Update(1);
        aircraftController.FixedUpdate(1);

        Assert.AreEqual(aircraftController.StateFinalApproach, aircraftController.StateMachine.currentState, "Should transition to FinalApproach.");

        // Move aircraft toward FinalApproach point and update
        ticks = 0;
        while (aircraftController.StateMachine.currentState != aircraftController.StateTouchDown)
        {
            aircraftGameObject.transform.position = Vector3.MoveTowards(
                aircraftGameObject.transform.position,
                finalApproachGO.transform.position,
                10f);
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            ticks++;
            if (ticks > 100)
            {
                Assert.IsTrue(false, "Did not reach TouchDown state.");
                return;
            }
        }

        Assert.AreEqual(aircraftController.StateTouchDown, aircraftController.StateMachine.currentState, "Should transition to TouchDown.");

        // Move aircraft toward TouchDown point and update
        ticks = 0;
        while (aircraftController.StateMachine.currentState != aircraftController.StateLanded)
        {
            aircraftGameObject.transform.position = Vector3.MoveTowards(
                aircraftGameObject.transform.position,
                touchDownPointGO.transform.position,
                10f);
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            ticks++;
            if (ticks > 100)
            {
                Assert.IsTrue(false, "Did not reach Landed state.");
                return;
            }
        }

        Assert.AreEqual(aircraftController.StateLanded, aircraftController.StateMachine.currentState, "Should transition to Landed.");
    }

    [Test]
    public void Aircraft_Speed_Reaches_Zero_After_Landing()
    {
        // Create runway approach points
        GameObject initialApproachGO = new GameObject();
        GameObject finalApproachGO = new GameObject();
        GameObject touchDownPointGO = new GameObject();

        GameObject runwayGO = new GameObject();

        initialApproachGO.transform.position = new Vector3(0, 0, 300);
        finalApproachGO.transform.position = new Vector3(0, 0, 200);
        touchDownPointGO.transform.position = new Vector3(0, 0, 100);

        Runway runway = runwayGO.AddComponent<Runway>();
        runway.Init(Team.Blue, initialApproachGO.transform, finalApproachGO.transform, touchDownPointGO.transform);

        // Spawn aircraft in air
        aircraftController.Spawn(isInAir: true, 100, 80);
        aircraftGameObject.transform.position = new Vector3(0, 100, 0);

        // Set runway to trigger landing
        aircraftController.RunwayInUse = runway;

        // Move toward InitialApproach to satisfy proximity check, then transition to FinalApproach
        int ticks = 0;
        aircraftGameObject.transform.position = initialApproachGO.transform.position;
        aircraftController.Update(1);
        aircraftController.FixedUpdate(1);

        Assert.AreEqual(aircraftController.StateFinalApproach, aircraftController.StateMachine.currentState, "Should transition to FinalApproach.");

        // Move aircraft toward FinalApproach point and update
        ticks = 0;
        while (aircraftController.StateMachine.currentState != aircraftController.StateTouchDown)
        {
            aircraftGameObject.transform.position = Vector3.MoveTowards(
                aircraftGameObject.transform.position,
                finalApproachGO.transform.position,
                10f);
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            ticks++;
            if (ticks > 100)
            {
                Assert.IsTrue(false, "Did not reach TouchDown state.");
                return;
            }
        }

        Assert.AreEqual(aircraftController.StateTouchDown, aircraftController.StateMachine.currentState, "Should transition to TouchDown.");

        // Move aircraft toward TouchDown point and update
        ticks = 0;
        while (aircraftController.StateMachine.currentState != aircraftController.StateLanded)
        {
            aircraftGameObject.transform.position = Vector3.MoveTowards(
                aircraftGameObject.transform.position,
                touchDownPointGO.transform.position,
                10f);
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            ticks++;
            if (ticks > 100)
            {
                Assert.IsTrue(false, "Did not reach Landed state.");
                return;
            }
        }

        Assert.AreEqual(aircraftController.StateLanded, aircraftController.StateMachine.currentState, "Should transition to Landed.");

        // Continue updating until speed reaches zero
        ticks = 0;
        while (aircraftController.MovementHandler.CurrSpeed > 0.1f)
        {
            aircraftController.Update(1);
            aircraftController.FixedUpdate(1);
            ticks++;
            if (ticks > 200)
            {
                Assert.IsTrue(false, "Speed did not reduce to zero after landing.");
                return;
            }
        }

        Assert.AreApproximatelyEqual(0f, aircraftController.MovementHandler.CurrSpeed, "Speed should be zero after landing.");
    }

    [Test]
    public void Aircraft_Can_Reach_Seek_Speed()
    {
        TestSeekSpeed(10, true);
        TestSeekSpeed(30, true);
        TestSeekSpeed(20, false);
        TestSeekSpeed(50, true);
        TestSeekSpeed(0, false);
    }

    private void TestSeekSpeed(float targetSpeed, bool speedIncreasing)
    {
        aircraftController.SeekSpeed(targetSpeed);
        float prevSpeed = aircraftController.MovementHandler.CurrSpeed;
        aircraftController.MovementHandler.Update(1);

        while (speedIncreasing ? prevSpeed < aircraftController.MovementHandler.CurrSpeed : prevSpeed > aircraftController.MovementHandler.CurrSpeed)
        {
            prevSpeed = aircraftController.MovementHandler.CurrSpeed;
            aircraftController.SeekSpeed(targetSpeed);
            aircraftController.MovementHandler.Update(1);
        }

        Assert.AreApproximatelyEqual(targetSpeed, aircraftController.MovementHandler.CurrSpeed, "Speed not achieved.");
    }
}
