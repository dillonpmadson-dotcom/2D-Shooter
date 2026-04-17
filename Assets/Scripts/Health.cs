using UnityEngine;

public class Health
{
    public float healthpoints;

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
