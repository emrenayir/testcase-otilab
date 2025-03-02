using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core.Events;
using Player;
using Core;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI topScoreText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;
        [SerializeField] private PlayerSave playerSave;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TextMeshProUGUI gameOverScoreText;


        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Sprite playSprite;

        private bool isPaused;
        private int currentScore = 0;

        private void OnEnable()
        {
            GameEvents.OnHealthUpdate += UpdateHealth;
            GameEvents.OnScoreUpdate += UpdateScore;
            GameEvents.OnGameFail += ShowGameOver;
            
            pauseButton.onClick.AddListener(TogglePause);
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnHealthUpdate -= UpdateHealth;
            GameEvents.OnScoreUpdate -= UpdateScore;
            GameEvents.OnGameFail -= ShowGameOver;
            
            pauseButton.onClick.RemoveListener(TogglePause);
            if (restartButton != null)
            {
                restartButton.onClick.RemoveListener(RestartGame);
            }
        }
        
        private void Start()
        {
            // Hide game over panel
            gameOverPanel.SetActive(false);
            
            // Update top score from saved data
            UpdateTopScore(playerSave.TopScore);
        }

        private void UpdateHealth(int health)
        {
            if (healthText != null)
            {
                healthText.text = $"Health: {health}";
            }
        }

        private void CheckTopScore(int score)
        {
            if (score > playerSave.TopScore)
            {
                playerSave.TopScore = score;
                UpdateTopScore(score);
            }
        }

        private void UpdateScore(int score)
        {
            currentScore = score;
            
            CheckTopScore(score);
        }

        private void UpdateTopScore(int score)
        {
            if (topScoreText != null)
            {
                topScoreText.text = $"TOP SCORE: {Helper.NumberFormatter.FormatNumber(score)}";
            }
        }

        private void TogglePause()
        {
            isPaused = !isPaused;
            
            Debug.Log("UIManager: TogglePause called, isPaused = " + isPaused);
            
            if (isPaused)
            {
                Debug.Log("UIManager: Triggering game pause");  
                GameEvents.TriggerGamePause();
                pauseButton.image.sprite = playSprite;
            }
            else
            {
                Debug.Log("UIManager: Triggering game resume");
                GameEvents.TriggerGameResume();
                pauseButton.image.sprite = pauseSprite;
            }
        }

        private void ShowGameOver()
        {
            gameOverPanel.SetActive(true);
            pauseButton.gameObject.SetActive(false);
            
            if (gameOverScoreText != null)
            {
                gameOverScoreText.text = $"{Helper.NumberFormatter.FormatNumber(currentScore)}";
            }
        }

        private void RestartGame()
        {
            gameOverPanel.SetActive(false);
            pauseButton.gameObject.SetActive(true);
            gameManager.RestartGame();
        }
    }
} 