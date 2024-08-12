using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet BulletPrefab;
    [SerializeField] private int InitialSize;

    private Queue<Bullet> poolQueue;

    private void Start()
    {
        poolQueue = new Queue<Bullet>();
        for (int i = 0; i < InitialSize; i++)
        {
            var bullet = Instantiate(BulletPrefab);
            bullet.gameObject.SetActive(false);
            poolQueue.Enqueue(bullet);
        }
    }

    internal Bullet Spawn(Vector3 position, Quaternion rotation)
    {
        Bullet bullet;
        if (poolQueue.Count > 0)
        {
            bullet = poolQueue.Dequeue();
            bullet.transform.position = position;
            bullet.transform.rotation = rotation;
            bullet.gameObject.SetActive(true);
        }
        else
        {
            bullet = Instantiate(BulletPrefab, position, rotation);
        }

        if (bullet.TryGetComponent(out NetworkObject ob)) ob.Spawn();

        return bullet;
    }

    internal void Despawn(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        if (bullet.TryGetComponent(out NetworkObject ob)) ob.Despawn(false);
        poolQueue.Enqueue(bullet);
    }
}