using UnityEngine;
using System.Collections.Generic;
using Core.ObjectPool;
using Core.Events;

namespace Cars
{
    public class CarFactory : MonoBehaviour
    {
        [SerializeField] private GameObject carBasePrefab;
        [SerializeField] private List<CarScriptableObject> carConfigurations;
        private ObjectPool<ObstacleCar> carPool;
        private int currentPlayerValue = 2; // Default starting value

        private void Awake()
        {
            InitializeCarPool();
        }

        private void OnEnable()
        {
            GameEvents.OnCarValueUpdate += HandlePlayerValueUpdate;
        }

        private void OnDisable()
        {
            GameEvents.OnCarValueUpdate -= HandlePlayerValueUpdate;
        }

        private void HandlePlayerValueUpdate(int newValue)
        {
            currentPlayerValue = newValue;
        }

        private void InitializeCarPool()
        {
            carPool = new ObjectPool<ObstacleCar>(carBasePrefab.GetComponent<ObstacleCar>(), 15, transform);
        }

        public ObstacleCar CreateObstacleCar(Vector3 position)
        {
            int value = GetAppropriateObstacleValue();
            var config = GetCarConfigurationByValue(value);
            
            if (config != null)
            {
                var car = carPool.Get();
                car.transform.position = position;
                car.Initialize(config);
                return car;
            }
            
            Debug.LogError($"No car configuration found for value: {value}");
            return null;
        }

        private int GetAppropriateObstacleValue()
        {
            int currentIndex = carConfigurations.FindIndex(config => config.Value == currentPlayerValue);
            if (currentIndex == -1)
            {
                Debug.LogError($"Current player value {currentPlayerValue} not found in configurations!");
                return 2; 
            }

            // Define the range of possible values
            int minIndex = Mathf.Max(0, currentIndex - 2); 
            int maxIndex = Mathf.Min(carConfigurations.Count - 1, currentIndex + 2); 

            // Get a random index within this range
            int randomIndex = Random.Range(minIndex, maxIndex + 1);
            return carConfigurations[randomIndex].Value;
        }

        public void ReturnObstacleCar(ObstacleCar car)
        {
            carPool.Return(car);
        }

        private CarScriptableObject GetCarConfigurationByValue(int value)
        {
            return carConfigurations.Find(config => config.Value == value);
        }
    }
} 