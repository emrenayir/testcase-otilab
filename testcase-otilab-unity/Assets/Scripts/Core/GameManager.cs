using UnityEngine;
using Core.Events;
using Player;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerSave playerSave;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform spawnPoint;

        private GameObject currentPlayer;

        public enum GameState
        {
            WaitingToStart,
            Playing,
            Paused,
            Failed
        }

        private GameState currentState;

        private void Awake()
        {
            Application.targetFrameRate = 61; // when we set this to 61 everything works fine on lower end devices  
            currentState = GameState.WaitingToStart;
            Debug.Log("GameManager: Initialized in WaitingToStart state");
        }

        private void Start()
        {
            // Initial setup when game loads
            SetupGame();
        }

        private void SetupGame()
        {
            Debug.Log("GameManager: Setting up initial game state");
            
            // Only spawn player if no current player exists
            if (currentPlayer == null)
            {
                Debug.Log("GameManager: No player exists, spawning new player");
                SpawnPlayer();
            }
            else
            {
                Debug.Log("GameManager: Player already exists, not spawning a new one");
            }
            
            // Kısa bir gecikme ile GameStart eventini tetikleyelim
            // Bu, tüm sistemlerin hazır olması için zaman tanır
            Debug.Log("GameManager: About to trigger GameStart event");
            GameEvents.TriggerGameStart(); // This will trigger road spawning and set player to wait for input
        }

        private void OnEnable()
        {
            Debug.Log("GameManager: OnEnable called, subscribing to events");
            
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGamePause += HandleGamePause;
            GameEvents.OnGameResume += HandleGameResume;
            GameEvents.OnGameFail += HandleGameFail;
            
            Debug.Log("GameManager: Successfully subscribed to events");
        }

        private void OnDisable()
        {
            Debug.Log("GameManager: OnDisable called, unsubscribing from events");
            
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGamePause -= HandleGamePause;
            GameEvents.OnGameResume -= HandleGameResume;
            GameEvents.OnGameFail -= HandleGameFail;
            
            Debug.Log("GameManager: Successfully unsubscribed from events");
        }

        private void Update()
        {
            if (currentState == GameState.WaitingToStart)
            {
                // We no longer need this since player handles its own movement start
                // Keep the state management though
                return;
            }
        }

        private void HandleGameStart()
        {
            Debug.Log("GameManager: HandleGameStart called");
            currentState = GameState.Playing;
        }

        private void HandleGamePause()
        {
            if (currentState == GameState.Playing)
            {
                Debug.Log("GameManager: Game Paused");
                currentState = GameState.Paused;
            }
        }

        private void HandleGameResume()
        {
            if (currentState == GameState.Paused)
            {
                Debug.Log("GameManager: Game Resumed");
                currentState = GameState.Playing;
            }
        }

        private void HandleGameFail()
        {
            Debug.Log("GameManager: Game Failed");
            currentState = GameState.Failed;
        }

        public bool IsGamePlaying()
        {
            return currentState == GameState.Playing;
        }

        public bool IsGamePaused()
        {
            return currentState == GameState.Paused;
        }
        
        private void SpawnPlayer()
        {
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }
            
            if (playerPrefab == null)
            {
                Debug.LogError("GameManager: Player Prefab is not assigned!");
                return;
            }

            if (spawnPoint == null)
            {
                Debug.LogError("GameManager: Spawn Point is not assigned!");
                return;
            }
            
            Debug.Log($"GameManager: Spawning player at position {spawnPoint.position}");
            currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            if (currentPlayer != null)
            {
                Debug.Log("GameManager: Player spawned successfully");
            }
        }
        
        public void RestartGame()
        {
            if (currentState == GameState.Failed)
            {
                Debug.Log("GameManager: Restarting game");
                
                if (currentPlayer != null)
                {
                    Debug.Log("GameManager: Resetting player position and rotation");
                    // Reset player position to spawn point
                    currentPlayer.transform.position = spawnPoint.position;
                    currentPlayer.transform.rotation = spawnPoint.rotation;
                    
                    Player.Player playerComponent = currentPlayer.GetComponent<Player.Player>();
                    if (playerComponent != null)
                    {
                        Debug.Log("GameManager: Resetting player state");
                        playerComponent.ResetPlayerState();
                    }
                    
                    PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        Debug.Log("GameManager: Resetting player controller");
                        playerController.ResetController();
                    }
                }
                else
                {
                    Debug.Log("GameManager: Player is null, spawning new player");
                    SpawnPlayer();
                }
                
                currentState = GameState.WaitingToStart;
                
                Debug.Log("GameManager: Triggering game restart event");
                GameEvents.TriggerGameRestart();    
                
                Debug.Log("GameManager: Triggering game start event");
                GameEvents.TriggerGameStart();
            }
        }
    }
}