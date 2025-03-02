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
        [SerializeField] private MeshRenderer carRenderer;
        [SerializeField] private List<CarScriptableObject> carConfigurations;
        [SerializeField] private TextMeshPro valueText;
        
        private int currentHealth;
        private int currentValue = 2;
        private PlayerController playerController;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
            ResetPlayerState();
            GameEvents.TriggerGameStart();
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
            GameEvents.TriggerGameFail();
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
                
                if (playerController != null)
                {
                    playerController.ForwardSpeed = carConfig.Speed * 3;
                }
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
            
            if (playerController != null)
            {
                playerController.ResetController();
            }
            
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