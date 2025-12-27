using Zenject;
using HealthSystem;
using UnityEngine;

//ZW stands for zenject wrapper. These kind of classes are an adapter for zenject and target class.
public class DamageablePartZW : MonoBehaviour
{
    [SerializeField]
    private DamageablePart damageablePart;

    [Inject]
    private void Install(Health health)
    {
        damageablePart.Init(health);
    }
}
