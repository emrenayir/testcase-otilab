using UnityEngine;
using Core.Events;
using Cars;
using System.Collections.Generic;
using TMPro;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int initialHealth = 3;
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private MeshRenderer carRenderer;
        [SerializeField] private List<CarScriptableObject> carConfigurations;
        [SerializeField] private TextMeshPro valueText;
        
        private int currentHealth;
        private int currentValue = 2;
        private bool isMoving = false;
        private bool isWaitingForInput = false;

        private void Start()
        {
            ResetPlayerState();
            HandleGameStart();
        }

        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGamePause += HandleGamePause;
            GameEvents.OnGameResume += HandleGameResume;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGamePause -= HandleGamePause;
            GameEvents.OnGameResume -= HandleGameResume;
        }

        private void Update()
        {
            CheckForClickToStart();
            if (isMoving)
            {
                transform.Translate(Vector3.forward * forwardSpeed * Time.smoothDeltaTime);
            }
        }

        private void CheckForClickToStart()
        {
            if (!isWaitingForInput) return;
            
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (UnityEngine.EventSystems.EventSystem.current != null)
                {
                    bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
                    if (!isOverUI)
                    {
                        StartMoving();
                    }
                }
                else
                {
                    StartMoving();
                }
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                bool isOverUI = false;
                if (UnityEngine.EventSystems.EventSystem.current != null)
                {
                    isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                }
                else
                {
                    StartMoving();
                    return;
                }
                
                if (!isOverUI)
                {
                    StartMoving();
                }
            }
        }

        private void StartMoving()
        {
            isWaitingForInput = false;
            isMoving = true;
            var controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.EnableControl();
            }
        }

        public void HandleCollision(ObstacleCar obstacle)
        {
            if (obstacle.Value == currentValue)
            {
                currentValue *= 2;
                GameEvents.TriggerCarValueUpdate(currentValue);
                GameEvents.TriggerScoreUpdate(currentValue);
                UpdateCarAppearance();
                
                if (valueText != null)
                {
                    valueText.text = Helper.NumberFormatter.FormatNumber(currentValue);
                }
            }
            else
            {
                TakeDamage(1);
            }
        }

        private void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            GameEvents.TriggerHealthUpdate(currentHealth);

            if (currentHealth <= 0)
            {
                GameFail();
            }
        }

        private void GameFail()
        {
            isMoving = false;
            isWaitingForInput = false;
            GameEvents.TriggerGameFail();
        }

        private void HandleGameStart()
        {
            isWaitingForInput = true;
            isMoving = false;
            Invoke("CheckInputFlags", 1.0f);
        }

        private void CheckInputFlags()
        {
            if (!isWaitingForInput && !isMoving)
            {
                isWaitingForInput = true;
            }
        }

        private void HandleGamePause()
        {
            isMoving = false;
        }

        private void HandleGameResume()
        {
            isMoving = true;
        }

        private void UpdateCarAppearance()
        {
            var carConfig = GetCarConfigurationByValue(currentValue);
            if (carConfig != null && carRenderer != null)
            {
                Material[] materials = carRenderer.materials;
                
                if (materials.Length > 0)
                {
                    materials[0] = carConfig.CarMaterial;
                    carRenderer.materials = materials;
                }
                
                forwardSpeed = carConfig.Speed *5;
            }
        }

        private CarScriptableObject GetCarConfigurationByValue(int value)
        {
            return carConfigurations.Find(config => config.Value == value);
        }

        public void ResetPlayerState()
        {
            currentHealth = initialHealth;
            currentValue = 2;
            isMoving = false;
            isWaitingForInput = false;
            
            UpdateCarAppearance();
            GameEvents.TriggerHealthUpdate(currentHealth);
            GameEvents.TriggerCarValueUpdate(currentValue);
            GameEvents.TriggerScoreUpdate(currentValue);
            
            if (valueText != null)
            {
                valueText.text = Helper.NumberFormatter.FormatNumber(currentValue);
            }
        }
    }
}