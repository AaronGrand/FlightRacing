using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using TMPro;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField nameInputField;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        connectingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void StartHost()
    {
        if(nameInputField.text == "") return;
        GameState.Instance.userName = nameInputField.text;
        HostManager.Instance.StartHost();
    }

    public void StartClient()
    {
        if (joinCodeInputField.text == "" || nameInputField.text == "") return;
        Debug.Log($"Starting Client: {joinCodeInputField.text}");
        ClientManager.Instance.StartClient(joinCodeInputField.text);
        GameState.Instance.userName = nameInputField.text;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
