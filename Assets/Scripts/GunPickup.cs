using UnityEngine;

// GunPickup: when picked up, gives the player rapid-fire for a limited time
public class GunPickup : Pickup
{
    [SerializeField] private float duration = 6f; // Seconds of boosted firing

    protected override void OnPickedUp(Player player)
    {
        player.AddGunPowerUp(duration);
    }
}
