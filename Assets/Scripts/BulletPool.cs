using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet BulletPrefab;

    [SerializeField] private int InitialSize;

    private Queue<Bullet> PoolQueue;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        PoolQueue = new Queue<Bullet>();
        for (int i = 0; i < InitialSize; i++)
        {
            var bullet = Instantiate(BulletPrefab);
            bullet.gameObject.SetActive(false);
            PoolQueue.Enqueue(bullet);
        }
    }

    internal Bullet Spawn(Vector3 position, Quaternion rotation, NetworkObjectReference owner)
    {
        Bullet bullet;

        if (PoolQueue.Count > 0)
        {
            bullet = PoolQueue.Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.gameObject.SetActive(true);
        }
        else
        {
            bullet = Instantiate(BulletPrefab, position, rotation);
        }

        bullet.SetOwner(owner);

        if (bullet.TryGetComponent(out NetworkObject networkObject)) networkObject.Spawn();

        return bullet;
    }

    internal void Despawn(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);

        if (bullet.TryGetComponent(out NetworkObject networkObject)) networkObject.Despawn(false);

        PoolQueue.Enqueue(bullet);
    }
}