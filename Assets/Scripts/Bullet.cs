using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float DespawnTime;
    [SerializeField] private float DamageAmount;

    private BulletPool BulletPool;
    private NetworkObjectReference OwnerReference;
    internal Vector2 Direction { private get; set; }


    private void Start()
    {
        if (IsServer) BulletPool = FindObjectOfType<BulletPool>();
    }

    private void Update()
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (IsServer)
        {
            var player = other.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (OwnerReference.TryGet(out var ownerObject) && ownerObject == player.NetworkObject) return;

                HitServerRpc(player.NetworkObjectId);
                DespawnServerRpc();
            }
        }
    }

    [ServerRpc]
    private void HitServerRpc(ulong playerId)
    {
        var player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId]?.GetComponent<Player>();
        if (player != null) player.Health.Value -= DamageAmount;
    }

    private IEnumerator Despawn(float time)
    {
        yield return new WaitForSeconds(time);
        if (IsServer) DespawnServerRpc();
    }

    [ServerRpc]
    private void DespawnServerRpc()
    {
        BulletPool.Despawn(this);
    }

    public void SetOwner(NetworkObjectReference owner)
    {
        OwnerReference = owner;
    }
}