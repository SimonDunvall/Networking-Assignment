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
        if (IsServer)
        {
            BulletPool = FindObjectOfType<BulletPool>();
            if (BulletPool == null) Debug.LogError("BulletPool not found in the scene.");
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsServer)
        {
            var player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                if (OwnerReference.TryGet(out var ownerObject) && ownerObject == player.NetworkObject) return;

                ApplyDamageServerRpc(player.NetworkObjectId);
                RequestDespawnServerRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestDespawnServerRpc()
    {
        BulletPool.Despawn(this);
    }

    [Rpc(SendTo.Server)]
    private void ApplyDamageServerRpc(ulong playerId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out var networkObject))
        {
            var player = networkObject.GetComponent<Player>();
            if (player != null) player.Health.Value -= DamageAmount;
        }
    }

    private IEnumerator Despawn(float time)
    {
        yield return new WaitForSeconds(time);
        if (IsServer) RequestDespawnServerRpc();
    }

    public void SetOwner(NetworkObjectReference owner)
    {
        OwnerReference = owner;
    }
}