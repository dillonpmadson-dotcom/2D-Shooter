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

    public override void Start()
    {
        base.Start(); // Sets up healthModule from Character.Start()

        // Build the player's starting weapon
        currentWeapon = new RangedWeapon(weaponFireRate, weaponDamage, bulletPrefab);
    }

    void Update()
    {
        // Read movement input (WASD / arrow keys)
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");

        // Convert mouse screen position into world coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Rotate player to face the mouse
        Rotate(mousePosition);

        // Apply movement
        Move();

        // Dash on Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dash();
        }

        // Fire on left mouse button (held = continuous fire, fire rate enforces cooldown)
        if (Input.GetMouseButton(0))
        {
            currentWeapon.Use(firePoint.position, firePoint.rotation);
        }
    }

    public void Dash()
    {
        RigidbodyModule.AddForce(movementDirection * moveSpeed * 3f);
    }
}
