using System.Collections;
using System.Collections.Generic;
using TargetingSystem;
using UnityEngine;
using Zenject;

namespace Game
{
    //ZW stands for zenject wrapper. These kind of classes are an adapter for zenject and target class.
    public class RadarMonobehaviourZW : MonoBehaviour
    {
        [SerializeField]
        private RadarMonobehaviour radar;

        [Inject]
        private void Install(TargetTracker tracker)
        {
            radar.Init(tracker);
        }
    }
}
