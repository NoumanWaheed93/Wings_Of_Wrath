using UnityEngine;

namespace AircraftController.AircraftAI
{
    /// <summary>
    /// The AI's output port for weapons, mirroring how the AI drives the airframe through
    /// <see cref="IAircraft"/>. Concrete weapon behaviours (which live in higher-level
    /// assemblies) implement this so the AI can fire without depending on the weapon system.
    /// </summary>
    public interface IWeaponController
    {
        /// <summary>
        /// Attempts to fire a shot at the given target.
        /// </summary>
        /// <returns>True if a shot was actually launched (had ammo and the fire rate allowed it).</returns>
        bool FireAt(Transform target);
    }
}
