using UnityEngine;

public class Player : Character, IDash
{
    [SerializeField] private Vector2 mousePosition;

    // Bullet & weapon settings (assigned in Inspector)
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float weaponFireRate = 5f;
    [SerializeField] private float weaponDamage = 10f;

    // The actual weapon instance the player uses
    private RangedWeapon currentWeapon;
    private RangedWeapon boostedWeapon;

    // Power-up state — nuke charges the player has stockpiled
    private int nukeCount = 0;
    public int NukeCount => nukeCount;

    // Gun power-up state
    [Header("Gun Power-up")]
    [SerializeField] private float boostedFireRate = 15f;   // 15 shots/sec while powered up
    private float gunPowerUpTimeLeft = 0f;
    public float GunPowerUpTimeLeft => gunPowerUpTimeLeft;
    public bool HasGunPowerUp => gunPowerUpTimeLeft > 0f;

    public override void Start()
    {
        base.Start(); // Sets up healthModule from Character.Start()

        // Build the player's starting weapon (normal + boosted variants)
        currentWeapon = new RangedWeapon(weaponFireRate, weaponDamage, bulletPrefab);
        boostedWeapon = new RangedWeapon(boostedFireRate, weaponDamage, bulletPrefab);
    }

    void Update()
    {
        // Stop responding to input once dead (Game Over UI takes over)
        if (isDead) return;

        // Read movement input (WASD / arrow keys) — input MUST be read in Update
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");

        // Convert mouse screen position into world coordinates + rotate to face mouse
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Rotate(mousePosition);

        // Dash on Space (input event — must catch in Update)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }

        // Tick the gun power-up timer down
        if (gunPowerUpTimeLeft > 0f)
        {
            gunPowerUpTimeLeft -= Time.deltaTime;
        }

        // Fire on left mouse button (held = continuous fire, fire rate enforces cooldown)
        // Use the boosted weapon while powered up. Pass `this` so the bullet doesn't shoot us.
        if (Input.GetMouseButton(0))
        {
            RangedWeapon weaponToUse = HasGunPowerUp ? boostedWeapon : currentWeapon;
            weaponToUse.Use(firePoint.position, firePoint.rotation, this);
        }

        // Right mouse button = detonate a nuke (if we have any)
        if (Input.GetMouseButtonDown(1))
        {
            UseNuke();
        }
    }

    // Physics goes in FixedUpdate — runs in lock-step with the physics engine.
    // This is what eliminates jitter when interpolation is on.
    void FixedUpdate()
    {
        Move();
    }

    public void Dash()
    {
        RigidbodyModule.AddForce(movementDirection * moveSpeed * 3f);
    }

    // Static event fires when the player dies — GameOverManager listens for this
    public static System.Action OnPlayerDied;

    [Header("Damage Flash")]
    [SerializeField] private UnityEngine.UI.Image damageFlashImage; // drag a full-screen red Image here

    // Override TakeDamage to also flash the screen red
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount); // inherited: HP decrease + body flash + die-check

        if (damageFlashImage != null && !isDead)
        {
            StartCoroutine(ScreenDamageFlash());
        }
    }

    private System.Collections.IEnumerator ScreenDamageFlash()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Color flashColor = new Color(1f, 0f, 0f, 0.35f);
        Color clearColor = new Color(1f, 0f, 0f, 0f);

        damageFlashImage.color = flashColor;
        while (elapsed < duration)
        {
            damageFlashImage.color = Color.Lerp(flashColor, clearColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        damageFlashImage.color = clearColor;
    }

    // Override base Die() — fire the event, freeze the player, but don't destroy the GameObject
    // (we leave the player visible so the corpse stays on screen during Game Over)
    public override void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player died!");

        // Stop the player from moving + shooting
        movementDirection = Vector2.zero;
        if (RigidbodyModule != null) RigidbodyModule.linearVelocity = Vector2.zero;

        // Notify the GameOverManager (or anyone else listening)
        OnPlayerDied?.Invoke();
    }

    // Called by NukePickup when the player walks over one
    public void AddNuke()
    {
        nukeCount++;
        Debug.Log("Nuke picked up! Now have " + nukeCount);
    }

    // Called by GunPickup — refreshes the rapid-fire timer
    public void AddGunPowerUp(float duration)
    {
        // If already active, extend the timer rather than overwrite
        gunPowerUpTimeLeft = Mathf.Max(gunPowerUpTimeLeft, duration);
        Debug.Log("Gun power-up! Time left: " + gunPowerUpTimeLeft);
    }

    // Detonates one nuke — wipes all enemies + pickups in the scene
    private void UseNuke()
    {
        if (nukeCount <= 0) return;
        nukeCount--;

        // Kill every enemy (pickups are spared so the player can still grab them)
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            enemy.Die();
        }

        Debug.Log("BOOM! Nuke detonated. Nukes left: " + nukeCount);
    }
}
