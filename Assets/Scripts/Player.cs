using Input;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private InputReader InputReader;

    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float FireRate;

    private readonly NetworkVariable<Vector2> MoveInput = new();
    private float AutoFirringTimer;
    private BulletPool BulletPool;

    private void Start()
    {
        BulletPool = FindObjectOfType<BulletPool>();

        if (InputReader == null || !IsLocalPlayer) return;
        InputReader.MoveEvent += OnMove;
        InputReader.ShotEvent += OnShot;
    }

    private void Update()
    {
        if (IsLocalPlayer && InputReader.IsShooting && Time.time >= AutoFirringTimer) AutoFirringLogic();
    }

    private void FixedUpdate()
    {
        if (IsServer) transform.position += (Vector3)MoveInput.Value * SpeedMultiplier;
    }

    private void AutoFirringLogic()
    {
        AutoFirringTimer = Time.time + FireRate;
        OnShot(InputReader.MousePosition);
    }

    private void OnShot(Vector2 input)
    {
        ShotRPC(input);
    }

    [Rpc(SendTo.Server)]
    private void ShotRPC(Vector2 data)
    {
        var bullet = BulletPool.Spawn(transform.position, transform.rotation);
        var bulletDirection = data - (Vector2)transform.position;
        bulletDirection.Normalize();

        bullet.Direction = bulletDirection;
    }

    private void OnMove(Vector2 input)
    {
        MoveRPC(input);
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        MoveInput.Value = data;
    }
}