using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Points")]
    public int points;
    public UnityEvent onPointsUpdated;

    [Header("Time")]
    public float timeToMatch=10f;
    public float currentTimeToMatch;
    
    [Header("Game State")]
    public GameState gameState;
    public enum GameState { Idle, InGame, GameOver}

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (gameState != GameState.InGame) return;
        
        // Add Time to Timer
        currentTimeToMatch += Time.deltaTime;
        // Finish Timer
        if(currentTimeToMatch >= timeToMatch) UIManager.Instance.SetPanelGameOver();
    }

    public void AddPoints(int matches, int value)
    {
        var  newPoints = matches * value;
        points += newPoints;
        onPointsUpdated?.Invoke();
        currentTimeToMatch = 0;
    }
    
    public void StartGame(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    
}
