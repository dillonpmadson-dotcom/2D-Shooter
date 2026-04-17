using UnityEngine;

// NukePickup: when picked up, gives the player +1 nuke charge
// Player can then right-click to detonate (handled by Player.cs)
public class NukePickup : Pickup
{
    protected override void OnPickedUp(Player player)
    {
        // Tell the player they got a nuke
        player.AddNuke();
    }
}
