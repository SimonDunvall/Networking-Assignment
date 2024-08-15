using Input;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private InputReader InputReader;

    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float FireRate;
    [SerializeField] private float StartHealth;
    internal readonly NetworkVariable<float> Health = new();

    private readonly NetworkVariable<Vector2> MoveInput = new();
    private float AutoFirringTimer;
    private BulletPool BulletPool;
    private bool IsMoving;

    private void Start()
    {
        if (IsServer) Health.Value = StartHealth;

        BulletPool = FindObjectOfType<BulletPool>();

        if (!IsLocalPlayer) return;
        CameraManager.Instance.SetPlayer(this);

        if (InputReader == null) return;
        InputReader.MoveEvent += OnMove;
        InputReader.ShotEvent += OnShot;
    }

    private void Update()
    {
        if (IsLocalPlayer && InputReader.IsShooting && Time.time >= AutoFirringTimer) AutoFirringLogic();

        if (IsServer)
            if (Health.Value <= 0)
                Respawn();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        transform.position += (Vector3)MoveInput.Value * (SpeedMultiplier * Time.deltaTime);
        IsMoving = MoveInput.Value != Vector2.zero;
    }

    private void Respawn()
    {
        if (IsServer)
        {
            Health.Value = StartHealth;
            transform.position = Vector3.zero;
        }
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
        if (IsMoving) return;
        var bullet = BulletPool.Spawn(transform.position, transform.rotation,
            new NetworkObjectReference(NetworkObject));
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