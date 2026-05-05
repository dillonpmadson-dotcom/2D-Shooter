using UnityEngine;

public class Health
{
    public float healthpoints;

    // True once HP drops to 0 or below — used by Character.Die()
    public bool IsDead => healthpoints <= 0f;

    public void IncreaseHealth(float ToIncrease)
    {
        healthpoints += ToIncrease;
    }

    public void DecreaseHealth(float toDecrease)
    {
        healthpoints -= toDecrease;
    }

    public Health(float initialHealth)
    {
        healthpoints = initialHealth;
    }
}
