using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkHandler;

    [Header("Panels")]
    [SerializeField] GameObject _initialPanel;
    [SerializeField] GameObject _sessionBrowserPanel;
    [SerializeField] GameObject _hostGamePanel;
    [SerializeField] GameObject _statusPanel;

    [Header("Buttons")]
    [SerializeField] Button _joinLobbyButton;
    [SerializeField] Button _gotToHostPanelButton;
    [SerializeField] Button _hostButton;

    [Header("InputFields")]
    [SerializeField] TMP_InputField _hostsessionName;

    [Header("Texts")]
    [SerializeField] TMP_Text _statusText;
    void Start()
    {
        _joinLobbyButton.onClick.AddListener(Button_JoinLobby);
        _gotToHostPanelButton.onClick.AddListener(Button_ShowHostPanel);
        _hostButton.onClick.AddListener(Button_CreateGameSession);

        _networkHandler.OnJoinedLobby += () =>
        {
            _statusPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };
    }

    void Button_JoinLobby()
    {

    }
    void Button_ShowHostPanel()
    {

    } 
    void Button_CreateGameSession()
    {

    }
}
