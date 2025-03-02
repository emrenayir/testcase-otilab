using UnityEngine;
using System.Collections.Generic;
using Cars;

namespace Road
{
    public class RoadPiece : MonoBehaviour
    {
        private List<ObstacleCar> obstacles = new List<ObstacleCar>();
        private CarFactory carFactory;

        public void AddObstacle(ObstacleCar obstacle)
        {
            obstacles.Add(obstacle);
            obstacle.transform.SetParent(transform);
        }

        public void ClearObstacles()
        {
            if (carFactory == null)
            {
                // Find CarFactory if not assigned
                carFactory = Object.FindAnyObjectByType<CarFactory>();
                if (carFactory == null)
                {
                    Debug.LogError("RoadPiece: Could not find CarFactory");
                    return;
                }
            }
            
            foreach (var obstacle in obstacles)
            {
                if (obstacle != null)
                {
                    carFactory.ReturnObstacleCar(obstacle);
                }
            }
            obstacles.Clear();
        }

        public void SetCarFactory(CarFactory factory)
        {
            carFactory = factory;
        }
    }
} 