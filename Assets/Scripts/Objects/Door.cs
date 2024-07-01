using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class Door : NetworkBehaviour, IActivable
{

    private NetworkMecanimAnimator _mecanim;
    private NetworkRunner _networkRunner;
    private int playerCount = 0;

    [SerializeField] private Sprite enabledDoor, activeDoor;
    private SpriteRenderer myRend;

    public override void Spawned()
    {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
        _networkRunner = FindObjectOfType<NetworkRunner>();
        myRend = GetComponent<SpriteRenderer>();
    }

    public void ChangeToActive()
    {
        RPC_ChangeToEnable();
        GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            if (playerCount >= 2)
                RPC_ChangeToActive();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            if (playerCount < 2)
                RPC_ChangeToEnable();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeToEnable()
    {
        myRend.sprite = enabledDoor;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeToActive()
    {
        myRend.sprite = activeDoor;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OpenDoor()
    {
        var anim = GetComponent<Animator>().enabled = true;
        _mecanim.Animator.SetBool("Open", true);

        StartCoroutine(ChangeSceneForAll());
    }

    public void Activate()
    {
        if (playerCount >= 2)
        {
            RPC_OpenDoor();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ChangeSceneForAll()
    {
        yield return new WaitForSeconds(1.0f);

        if (_networkRunner != null && _networkRunner.IsServer)
        {
            RPC_ChangeScene("Level2");
        }
    }

    private void DespawnPlayers()
    {
        if (_networkRunner != null)
        {
            foreach (var player in _networkRunner.ActivePlayers)
            {
                var networkObject = _networkRunner.GetPlayerObject(player);
                if (networkObject != null)
                {
                    _networkRunner.Despawn(networkObject);
                }
            }
        }
    }
}

