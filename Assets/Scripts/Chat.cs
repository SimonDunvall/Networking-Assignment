using System.Collections.Generic;
using Input;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Chat : NetworkBehaviour
{
    [SerializeField] private InputReader InputReader;
    [SerializeField] private TextMeshProUGUI TextFieldPrefab;
    [SerializeField] private GameObject TextFieldArea;
    [SerializeField] private TMP_InputField MessageInputField;
    [SerializeField] private int MaxMessages;
    private readonly Queue<TextMeshProUGUI> TextFieldsQueue = new();

    private void Start()
    {
        if (InputReader != null) InputReader.SendEvent += OnSend;
    }

    private void OnSend()
    {
        if (string.IsNullOrWhiteSpace(MessageInputField.text)) return;
        var message = new FixedString128Bytes(MessageInputField.text);
        SubmitMessageRPC(message);
        MessageInputField.text = string.Empty;
    }

    [Rpc(SendTo.Server)]
    private void SubmitMessageRPC(FixedString128Bytes message)
    {
        UpdateMessageRPC(message);
    }

    [Rpc(SendTo.Everyone)]
    private void UpdateMessageRPC(FixedString128Bytes message)
    {
        if (TextFieldsQueue.Count >= MaxMessages) Destroy(TextFieldsQueue.Dequeue());

        var textField = Instantiate(TextFieldPrefab, TextFieldArea.transform);
        textField.text = message.ToString();
        TextFieldsQueue.Enqueue(textField);
    }
}