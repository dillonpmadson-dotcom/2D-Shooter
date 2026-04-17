using UnityEngine;

public abstract class Weapon
{
    // Protected so child classes (RangedWeapon, MeleeWeapon) can set this
    [SerializeField] protected float damage;

    // Each child weapon decides what "Use" means (shoot, slash, etc.)
    public abstract void Use();

    public abstract void StopUse();

    // Public getter so other scripts can READ damage safely
    public float GetDamageValue()
    {
        return damage;
    }
}
