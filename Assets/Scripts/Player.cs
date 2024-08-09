using Input;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private InputReader InputReader;

    [SerializeField] private float speedMultiplier;

    private readonly NetworkVariable<Vector2> MoveInput = new();

    private void Start()
    {
        if (InputReader != null && IsLocalPlayer) InputReader.MoveEvent += OnMove;
    }

    private void FixedUpdate()
    {
        if (IsServer) transform.position += (Vector3)MoveInput.Value * speedMultiplier;
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