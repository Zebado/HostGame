using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;


public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [SerializeField] NetworkRunnerHandler _networkHandler;

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
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void RestartLevel()
    {
        if (HasInputAuthority)
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