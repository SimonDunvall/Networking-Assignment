using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float FollowThreshold;
    [SerializeField] private float SmoothSpeed;

    private Player Player;
    private Vector3 Velocity;
    internal static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (Player)
            transform.position =
                Vector3.SmoothDamp(transform.position,
                    new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z),
                    ref Velocity, SmoothSpeed);
    }

    internal void SetPlayer(Player player)
    {
        Player = player;
    }
}