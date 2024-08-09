using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private NetworkManager NetworkManager;

    private void OnGUI()
    {
        if (GUILayout.Button("Host")) NetworkManager.StartHost();

        if (GUILayout.Button("Join")) NetworkManager.StartClient();
        if (GUILayout.Button("Quit")) Application.Quit();
    }
}