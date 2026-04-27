using UnityEngine;

public class RangedWeapon : Weapon
{
    private float fireRate;
    private GameObject bulletPrefab;
    private float lastFireTime;

    // Constructor — sets up the weapon with custom values when created in code
    public RangedWeapon(float newFireRate, float newDamage, GameObject newBulletPrefab)
    {
        fireRate = newFireRate;
        damage = newDamage;
        bulletPrefab = newBulletPrefab;
        lastFireTime = -999f; // Allow firing immediately on game start
    }

    // Fires a bullet from the given position, in the given direction.
    // The "owner" is the Character firing — bullet won't damage its own owner.
    public void Use(Vector3 spawnPosition, Quaternion spawnRotation, Character owner = null)
    {
        // Check fire rate cooldown — only fire if enough time has passed
        if (Time.time - lastFireTime < 1f / fireRate)
        {
            return;
        }
        lastFireTime = Time.time;

        // Spawn the bullet
        GameObject newBullet = GameObject.Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        // Configure the new bullet
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
            bulletScript.SetOwner(owner);
        }
    }

    // Old abstract Use() from Weapon — required override but not used for ranged
    public override void Use()
    {
        // Use the version with position/rotation parameters instead
    }

    public override void StopUse()
    {
        // Nothing to clean up between shots
    }
}
