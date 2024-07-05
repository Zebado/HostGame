using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;


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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RestartLevel()
    {
        SpawnManager.Instance.StartNewLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartLevel()
    {
        if (_runnerPrefab != null && _runnerPrefab.IsSceneAuthority)
        {
            RPC_RestartLevel();
        }
    }

    public void QuitGame()
    {
        // Lógica para salir del juego
        Application.Quit();
    }
}