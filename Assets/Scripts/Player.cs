using Input;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private InputReader InputReader;
    [SerializeField] private float SpeedMultiplier;
    [SerializeField] private float StartHealth;

    internal readonly NetworkVariable<float> Health = new(writePerm:
        NetworkVariableWritePermission.Server
    );

    private readonly NetworkVariable<Vector2> MoveInput = new(writePerm:
        NetworkVariableWritePermission.Server
    );

    private float AutoFirringTimer;
    private BulletPool BulletPool;
    private bool IsMoving;

    private void Start()
    {
        if (IsServer) Health.Value = StartHealth;

        BulletPool = FindObjectOfType<BulletPool>();

        if (IsLocalPlayer)
        {
            CameraManager.Instance.SetPlayer(this);

            if (InputReader != null)
            {
                InputReader.MoveEvent += OnMove;
                InputReader.ShotEvent += OnShot;
            }
        }
    }

    private void Update()
    {
        if (IsServer && Health.Value <= 0) Respawn();
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            var moveVector = (Vector3)MoveInput.Value * (SpeedMultiplier * Time.deltaTime);
            transform.position += moveVector;
            IsMoving = MoveInput.Value != Vector2.zero;
        }
    }

    private void Respawn()
    {
        if (IsServer)
        {
            Health.Value = StartHealth;
            transform.position = Vector3.zero;
        }
    }

    private void OnShot(Vector2 input)
    {
        ShotRPC(input);
    }

    [Rpc(SendTo.Server)]
    private void ShotRPC(Vector2 data)
    {
        if (!IsMoving)
        {
            var bullet = BulletPool.Spawn(transform.position, transform.rotation,
                new NetworkObjectReference(NetworkObject));
            if (bullet)
            {
                var bulletDirection = data - (Vector2)transform.position;
                bulletDirection.Normalize();

                bullet.Direction = bulletDirection;
            }
        }
    }

    private void OnMove(Vector2 input)
    {
        if (MoveInput.Value != input) MoveRPC(input);
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        MoveInput.Value = data;
    }
}