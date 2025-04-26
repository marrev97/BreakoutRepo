using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject spacebarText;

    [HideInInspector] public int currentLevel = 1;
    public int CurrentLevel => currentLevel;

    private int score = 0;

    void Start()
    {
        levelText.text = $"Level: {currentLevel}";
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacebarText.SetActive(false);
        }
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {score}";
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    public void UpdateLevelText()
    {
        levelText.text = $"Level: {currentLevel}";
    }
}