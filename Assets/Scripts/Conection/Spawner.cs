using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkPrefabRef _playerPrefab1,_playerPrefab2;
    [SerializeField] private int cantOfPlayers;
    public Dictionary<PlayerRef, NetworkObject> playerObjects = new Dictionary<PlayerRef, NetworkObject>();
   
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Transform spawnPoint = cantOfPlayers % 2 == 0 ? SpawnManager.Instance.GetSpawnPoint1() : SpawnManager.Instance.GetSpawnPoint2();
            NetworkPrefabRef prefab = cantOfPlayers % 2 == 0 ? _playerPrefab1 : _playerPrefab2;
            NetworkObject playerObject = runner.Spawn(prefab, spawnPoint.position, spawnPoint.rotation, player);
            if(!playerObjects.ContainsKey(player))
                playerObjects.Add(player, playerObject);
            cantOfPlayers++;
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

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        cantOfPlayers--;
    }
    
    
    public void OnSceneLoadDone(NetworkRunner runner) {
        Debug.Log("Dictionary");
        foreach (var player in playerObjects.Values)
        {
            Debug.Log("Este es el diccionario del spawner" + player);
            StartCoroutine(Esperar(runner,player));
        }
    }
    IEnumerator Esperar(NetworkRunner runner, NetworkObject player)
    {
        yield return new WaitForSeconds(5);
        var controller = player.GetComponent<NewCharacterController>();
        controller.SetNewLevelSpawn();

    }
    #region Unused Callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
