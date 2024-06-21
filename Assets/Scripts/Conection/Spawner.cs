using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkPrefabRef _playerPrefab1,_playerPrefab2;
    [SerializeField] private int cantOfPlayers;
    [SerializeField] Vector3 _spawnPlayerHost;
    [SerializeField] Vector3 _spawnPlayerClient;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if(cantOfPlayers % 2 == 0){
                runner.Spawn(_playerPrefab1, _spawnPlayerHost, null, player);
                cantOfPlayers++;
            }
            else{
                runner.Spawn(_playerPrefab2, _spawnPlayerClient, null, player);
                cantOfPlayers++;
            }
        }
    }   
    CharacterInputHandler _characterInputHandler;
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;
        _characterInputHandler ??= NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        //aca mandamos la "caja" con los inputs a travez de la red hacia el host.
        input.Set(_characterInputHandler.GetLocalInputs());
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) 
    {

    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        cantOfPlayers--;
    }

    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
