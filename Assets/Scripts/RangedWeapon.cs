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

    // Fires a bullet from the given position, in the given direction
    public void Use(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        // Check fire rate cooldown — only fire if enough time has passed
        if (Time.time - lastFireTime < 1f / fireRate)
        {
            return;
        }
        lastFireTime = Time.time;

        // Spawn the bullet
        GameObject newBullet = GameObject.Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        // Tell the bullet how much damage to deal
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDamage(damage);
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
