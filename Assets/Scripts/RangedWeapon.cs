using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bulletPrefab;

    // Constructor — sets up the weapon with custom values when created in code
    public RangedWeapon(float newFireRate, float newDamage)
    {
        fireRate = newFireRate;
        damage = newDamage;
    }

    public override void Use()
    {
        Debug.Log("pew pew");
    }

    public override void StopUse()
    {
        // Nothing to do for now — ranged weapon stops automatically between shots
    }
}
