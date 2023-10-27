using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("UI Points")] 
    [SerializeField] private int pointsToWin = 2000;
    [SerializeField] private Slider sliderPoints;
    [SerializeField] private TextMeshProUGUI textScore;
    
    [Header("UI Time")] 
    [SerializeField] private Image sliderTime;

    [Header("Panels")] 
    [SerializeField] private GameObject panelWin; 
    [SerializeField] private GameObject panelPause; 
    [SerializeField] private GameObject panelGameOver; 

    private void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        var newTime = 1-((float)GameManager.Instance.currentTimeToMatch / (float)GameManager.Instance.timeToMatch);
        sliderTime.fillAmount= newTime >= 0 ? newTime : 0;
    }

    public void UpdatePoints()
    {
        AudioManager.Instance.PlayAudio(0);
        textScore.text = GameManager.Instance.points.ToString();
        StartCoroutine(UpdatePointsCoroutine());
    }

    private IEnumerator UpdatePointsCoroutine()
    {
        var newPoints = (float)GameManager.Instance.points / (float)pointsToWin;
        while (sliderPoints.value < newPoints)
        {
            sliderPoints.value+=0.01f;
            if (sliderPoints.value >= 1) SetPanelWin();
            yield return new WaitForSeconds(0.075f);
        }
        yield return null;
    }

    private void SetPanelWin()
    {
        Time.timeScale = 0;
        panelWin.SetActive(true);
        AudioManager.Instance.PlayAudio(1);
        GameManager.Instance.gameState = GameManager.GameState.Idle;
    }
    
    public void SetPanelPause(bool state)
    {
        Time.timeScale = state ? 0 : 1;
        panelPause.SetActive(state);
        GameManager.Instance.gameState = 
            state ? GameManager.GameState.Idle : GameManager.GameState.InGame;
    }

    public void SetPanelGameOver()
    {
        Time.timeScale = 0;
        panelGameOver.SetActive(true);
        AudioManager.Instance.PlayAudio(2);
        GameManager.Instance.gameState = GameManager.GameState.GameOver;
    }
}