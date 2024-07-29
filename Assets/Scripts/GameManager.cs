using UnityEngine;
using Fusion;
using Fusion.Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] NetworkRunnerHandler _networkHandler;
    [SerializeField] NetworkRunner _runnerPrefab;


    public GameObject defeatMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _networkHandler = FindObjectOfType<NetworkRunnerHandler>();
        _runnerPrefab = FindAnyObjectByType<NetworkRunner>();
    }

    public void ShowDefeatMenu()
    {
        defeatMenu.SetActive(true);
        Time.timeScale = 0f; // Pausa el juego
    }

    public void HideDefeatMenu()
    {
        defeatMenu.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RestartLevel()
    {
        StartCoroutine(LeaveRoomAndDisconnect());
    }
    private IEnumerator LeaveRoomAndDisconnect()
    {
        if(_runnerPrefab != null)
        {
            // Detener el NetworkRunner
            _runnerPrefab.Shutdown();
            Destroy(_runnerPrefab);
            SceneManager.LoadScene("Main Menu");

            // Esperar un frame para asegurarse de que el shutdown se complete
            yield return null;
        }
    }

    public void RestartLevel()
    {
        RPC_RestartLevel();
    }

    public void QuitGame()
    {
        // Lógica para salir del juego
        Application.Quit();
    }
}