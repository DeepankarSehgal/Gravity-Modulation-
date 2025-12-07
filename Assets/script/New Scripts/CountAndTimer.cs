using UnityEngine;
using TMPro;

public class CountAndTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float durationInSeconds = 120f; // 2 minutes
    [SerializeField] private GameObject pauseMenu;

    private float _remainingTime;
    private bool _isRunning = true;

    public bool IsTimeUp => !_isRunning && _remainingTime <= 0f;

    private void Start()
    {
        _remainingTime = durationInSeconds;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!_isRunning) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            _isRunning = false;
            OnTimeUp();
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(_remainingTime / 60f);
        int seconds = Mathf.FloorToInt(_remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnTimeUp()
    {
        if (timerText != null)
            timerText.color = Color.red;

        Time.timeScale = 0f;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
    }

    // Optional: if later you want to restart timer from other scripts:
    public void ResetTimer()
    {
        _remainingTime = durationInSeconds;
        _isRunning = true;
        if (timerText != null)
            timerText.color = Color.white;
        UpdateTimerUI();
    }
}
