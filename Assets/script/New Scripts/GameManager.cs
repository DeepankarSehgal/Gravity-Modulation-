using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Collectibles")]
    [SerializeField] private TMP_Text itemsText;
    [SerializeField] private int totalRequired = 8;

    [Header("Win UI")]
    [SerializeField] private GameObject nextLevelPanel;

    private int _currentCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        UpdateItemUI();
    }

    public void OnCollectiblePicked(GameObject collectible)
    {
        if (collectible != null)
            Destroy(collectible);

        _currentCount++;
        UpdateItemUI();

        if (_currentCount >= totalRequired)
        {
            HandleWin();
        }
    }

    private void UpdateItemUI()
    {
        if (itemsText != null)
            itemsText.text = _currentCount.ToString();
    }

    private void HandleWin()
    {
        if (nextLevelPanel != null)
            nextLevelPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
