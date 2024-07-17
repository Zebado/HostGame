using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;
using System;

public class CanvasHandler : MonoBehaviour
{
    public static CanvasHandler Instance { get; private set; }
    [SerializeField] GameObject _menuDefeat;
    List<NewCharacterController> _controllers = new List<NewCharacterController>();
    PlayerHealth _playerHealth;
    NetworkRunner _networkRunner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        _controllers = FindObjectsOfType<NewCharacterController>().ToList();
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
        _networkRunner = FindAnyObjectByType<NetworkRunner>();
    }
    public void RestartPlayers()
    {
        if (_networkRunner.IsServer)
            RPC_RestartPlayersPoint();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RestartPlayersPoint()
    {
        var spawner = FindObjectOfType<Spawner>();
        if (spawner != null)
            spawner.RestartLevel();
        foreach (var player in _controllers)
        {
            player.SetNewLevelSpawn();
        }
    }
    private void OnEnable()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        if (_playerHealth != null)
            _playerHealth.OnDead += ActiveLoseMenu;
    }
    private void OnDisable()
    {
        if(_playerHealth != null)
            _playerHealth.OnDead -= ActiveLoseMenu;
    }
    public void ActiveLoseMenu()
    {
        _menuDefeat.SetActive(true);
    }
}
