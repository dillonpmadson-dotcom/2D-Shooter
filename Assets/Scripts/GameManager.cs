using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Holds the two weapon options the player could use
    private Weapon weaponOption1;
    private Weapon weaponOption2;

    // Unity automatically runs Start() once when the scene begins
    void Start()
    {
        // Build a ranged weapon with fire rate 5.5 and damage 10
        // (Bullet prefab passed as null here — Player builds its own weapon with a real prefab)
        weaponOption1 = new RangedWeapon(newFireRate: 5.5f, newDamage: 10, newBulletPrefab: null);

        // Build a melee weapon (uses default values from MeleeWeapon)
        weaponOption2 = new MeleeWeapon();

        // Print weapon damage to the console so we can confirm it works
        Debug.Log("Weapon 1 damage: " + weaponOption1.GetDamageValue());
    }

    // Public getter so other scripts can grab weaponOption1
    public Weapon GetWeaponOption1()
    {
        return weaponOption1;
    }

    public Weapon GetWeaponOption2()
    {
        return weaponOption2;
    }
}
