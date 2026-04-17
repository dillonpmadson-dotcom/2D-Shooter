using UnityEngine;

public class Enemy : Character
{
    private Transform playerTargetTransform;

    public override void Start()
    {
        base.Start();
        playerTargetTransform = FindAnyObjectByType<Player>().transform;
    }

    public virtual void Update()
    {
        movementDirection = (playerTargetTransform.position - transform.position);
        movementDirection = movementDirection.normalized;

        Rotate(playerTargetTransform.position);

        Move();
        
    }

}
   
