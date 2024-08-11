using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float Speed;
    private Vector2 Direction;

    private void FixedUpdate()
    {
        transform.position += (Vector3)Direction * (Speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 direction)
    {
        Direction = direction;
    }
}