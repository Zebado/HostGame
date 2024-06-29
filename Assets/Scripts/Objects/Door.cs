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
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            playerCount++;
            if(playerCount >= 2)
                RPC_ChangeToActive();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")){
            playerCount--;
            if(playerCount < 2)
                RPC_ChangeToEnable();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeToEnable(){
        myRend.sprite = enabledDoor;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeToActive(){
        myRend.sprite = activeDoor;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OpenDoor(){
        var anim = GetComponent<Animator>().enabled = true;
        _mecanim.Animator.SetBool("Open", true);

        StartCoroutine(HandlePlayerDespawnAndSceneChange());
    }
    public void Activate()
    {
        if(playerCount >= 2){
            RPC_OpenDoor();
        }

    }
    private IEnumerator HandlePlayerDespawnAndSceneChange()
    {
        yield return new WaitForSeconds(1.0f);

        DespawnPlayer();

        ChangeScene();
    }
    private void DespawnPlayer()
    {
        if (_networkRunner != null)
        {
            NetworkPlayer localPlayer = NetworkPlayer.Local;
            if (localPlayer != null)
            {
                NetworkObject localNetworkObject = localPlayer.GetComponent<NetworkObject>();
                if (localNetworkObject != null)
                {
                    _networkRunner.Despawn(localNetworkObject);
                }
            }
        }
    }
    private void ChangeScene()
    {
        SceneManager.LoadScene("Level2");
    }
}

