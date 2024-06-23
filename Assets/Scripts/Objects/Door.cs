using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using TMPro;

public class Door : MonoBehaviour, IActivable
{
    [SerializeField] Sprite doorActive;
    private NetworkMecanimAnimator _mecanim;
    public bool _active = false;
    private NetworkRunner _networkRunner;
    [SerializeField] TextMeshProUGUI _contadorUI;

    private void Start()
    {
        _mecanim = GetComponent<NetworkMecanimAnimator>();
        _networkRunner = FindObjectOfType<NetworkRunner>();
        if (_contadorUI != null)
        {
            _contadorUI.gameObject.SetActive(false);
        }
    }
    public void ChangeToActive()
    {
        _mecanim.Animator.SetBool("Active", true);
        GetComponent<BoxCollider>().enabled = true;
    }
    public void Activate()
    {
        _mecanim.Animator.SetBool("Open", true);

        StartCoroutine(HandlePlayerDespawnAndSceneChange());

        Debug.Log("Object Activated!");
    }
    private IEnumerator HandlePlayerDespawnAndSceneChange()
    {
        if (_contadorUI != null)
        {
            _contadorUI.gameObject.SetActive(true);
        }

        int countdown = 3;
        while (countdown >= 0)
        {
            _contadorUI.text = countdown.ToString();
            yield return new WaitForSeconds(1.0f);
            countdown--;
        }

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

