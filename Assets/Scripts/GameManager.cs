using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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

    public void RestartGame()
    {
        // Lógica para reiniciar el juego
        // Por ejemplo, cargar la escena nuevamente
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Lógica para salir del juego
        Application.Quit();
    }
}