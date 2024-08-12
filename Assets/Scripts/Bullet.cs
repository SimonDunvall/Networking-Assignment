using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float DespawnTime;
    private BulletPool BulletPool;
    internal Vector2 Direction { private get; set; }

    private void Start()
    {
        if (IsServer) BulletPool = FindObjectOfType<BulletPool>();
    }

    private void FixedUpdate()
    {
        if (IsServer) transform.position += (Vector3)Direction * (Speed * Time.deltaTime);
    }

    private void OnEnable()
    {
        StartCoroutine(Despawn(DespawnTime));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Despawn(float time)
    {
        yield return new WaitForSeconds(time);
        if (IsServer) DespawnRPC();
    }

    [Rpc(SendTo.Server)]
    private void DespawnRPC()
    {
        BulletPool.Despawn(this);
    }
}