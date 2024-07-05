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
    private int cantOfActives = 0;
    [SerializeField] private int cantToActive = 1;

    [SerializeField] private Sprite enabledDoor, activeDoor;
    private SpriteRenderer myRend;
    [SerializeField] private string sceneName;

    public override void Spawned()
    {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
        _networkRunner = FindObjectOfType<NetworkRunner>();
        myRend = GetComponent<SpriteRenderer>();
    }
    public void ChangeToActive()
    {
        cantOfActives++;
        if(cantOfActives >= cantToActive){
            RPC_ChangeToEnable();
            GetComponent<BoxCollider2D>().enabled = true;
        }
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
        if(myRend != null)
            myRend.sprite = enabledDoor;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ChangeToActive(){
        if(myRend != null)
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


        if (_networkRunner != null && _networkRunner.IsSceneAuthority)
        {
            ChangeScene();
            SpawnManager.Instance.StartNewLevel();
        }
    }
    private void DespawnPlayer()
    {
        /*if (_networkRunner != null)
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
        }*/
    }
    private void ChangeScene()
    {
        var runnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        runnerHandler.ChangeScene(sceneName);
        
    }
}

