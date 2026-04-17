using UnityEngine;
using UnityEngine.Rendering;

public abstract class Character : MonoBehaviour
{
    protected Vector2 movementDirection;
    protected bool isDead;


    [SerializeField] protected float moveSpeed;
    [SerializeField] protected Rigidbody2D RigidbodyModule;

    public Health healthModule;

    public virtual void Start()
    {
        // Auto-grab Rigidbody2D if not manually assigned (saves dragging it in the Inspector)
        if (RigidbodyModule == null)
        {
            RigidbodyModule = GetComponent<Rigidbody2D>();
        }

        healthModule = new Health(100);
    }


    public void Move()
    {
        // Set velocity directly — crisp, responsive, jitter-free movement.
        // movementDirection is already normalized (length 1) so speed = moveSpeed in units/sec.
        RigidbodyModule.linearVelocity = movementDirection * moveSpeed;
    }

    public void Rotate(Vector3 directionToRotate)
    {
        transform.up = directionToRotate - transform.position;
    }

    public virtual void Attack()
    { 

    }
}
