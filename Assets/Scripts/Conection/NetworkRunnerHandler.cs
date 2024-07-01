using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner _runnerPrefab;

    NetworkRunner _currentRunner;

    public event Action OnJoinedLobby = delegate { };

    public event Action<List<SessionInfo>> OnSessionListUpdate = delegate { };


    #region Join / Create Game

    public async void CreateGame(string sessionName, string sceneName)
    {
        await InitializeGame(GameMode.Host, sessionName, SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}"));
    }
    public async void JoinGame(SessionInfo sessionInfo)
    {
        await InitializeGame(GameMode.Client, sessionInfo.Name, SceneManager.GetActiveScene().buildIndex);
    }

    async Task InitializeGame(GameMode gameMode, string sessionName, int sceneIndex)
    {
        _currentRunner.ProvideInput = true;

        var result = await _currentRunner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode,
            Scene = SceneRef.FromIndex(sceneIndex),
            SessionName = sessionName
        });

        if (!result.Ok)
        {
            Debug.LogError("[Custom error] Unable to start game");
        }
        else
        {
            Debug.Log("[Custom Msg] Game started");
        }
    }

    #endregion

    #region Lobby
    public void JoinLobby()
    {
        if (_currentRunner != null)
        {
            Destroy(_currentRunner.gameObject);
        }

        _currentRunner = Instantiate(_runnerPrefab);
        DontDestroyOnLoad(_currentRunner.gameObject);

        _currentRunner.AddCallbacks(this);

        JoinLobbyAsync();
    }
     
    async void JoinLobbyAsync()
    {
        var result = await _currentRunner.JoinSessionLobby(SessionLobby.Custom, "Normal Lobby");

        if (!result.Ok)
        {
            Debug.LogError("[Custom error] UNable to Join lobby");
        }
        else
        {
            Debug.Log("[Custom Msg] Joined Lobby");
            OnJoinedLobby();

            //CreateGame("x", "Game");
        }
    }
    public void ChangeScene(string sceneName)
    {
        Debug.Log("llamaron a change scene");
        if (_currentRunner.IsServer)
        {
            int sceneIndex = SceneUtility.GetBuildIndexByScenePath($"Scenes/{sceneName}");
            if (sceneIndex >= 0)
            {
                Debug.Log("cambiando de escena  ");

                SceneManager.LoadScene(sceneIndex);
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' not found in Build Settings.");
            }
        }
        else
        {
            Debug.LogError("ChangeScene can only be called on the server.");
        }
    }
    #endregion

    #region Used Runner Callbacks
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

        OnSessionListUpdate(sessionList);
    }
    #endregion


    #region Unused Runner Callbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    #endregion
}

