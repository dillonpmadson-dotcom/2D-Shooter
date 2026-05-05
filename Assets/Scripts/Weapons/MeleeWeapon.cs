using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] private float range;

    public override void Use()
    {
        Debug.Log("Slash");
    }

    public override void StopUse()
    {
        // Melee swings stop immediately; nothing to clean up
    }
}
