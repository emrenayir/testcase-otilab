using UnityEngine;
using System.Collections.Generic;
using Core.ObjectPool;
using Cars;
using Core.Events;
using Player;

namespace Road
{
    public class RoadSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject roadPiecePrefab;
        [SerializeField] private CarFactory carFactory;
        [SerializeField] private int initialRoadCount = 8;
        [SerializeField] private float roadLength = 10f;
        [SerializeField] private float clipValue = 3.0f;

        private float spawnTriggerDistance;
        private float despawnDistance;
        
        private Transform playerTransform;
        private PlayerController playerController;
        private ObjectPool<RoadPiece> roadPool;
        private List<RoadPiece> activeRoads;
        private float lastSpawnZ;
        private bool isInitialized = false;

        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameFail += HandleGameFail;
            GameEvents.OnGameRestart += HandleGameRestart;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameFail -= HandleGameFail;
            GameEvents.OnGameRestart -= HandleGameRestart;
        }

        private void Start()
        {
            if (roadPiecePrefab == null)
            {
                Debug.LogError("RoadSpawner: Road Piece Prefab is not assigned!");
                return;
            }

            if (carFactory == null)
            {
                Debug.LogError("RoadSpawner: Car Factory is not assigned!");
                return;
            }

            CalculateDistances();
            
            roadPool = new ObjectPool<RoadPiece>(roadPiecePrefab.GetComponent<RoadPiece>(), initialRoadCount, transform);
            activeRoads = new List<RoadPiece>();
            
            foreach (var roadPiece in roadPool.GetActiveObjects())
            {
                roadPiece.SetCarFactory(carFactory);
            }
        }

        private void CalculateDistances()
        {
            spawnTriggerDistance = roadLength * clipValue;
            despawnDistance = roadLength * 1.2f;
        }
        
        private void HandleGameStart()
        {
            if (!isInitialized)
            {
                InitializeRoadSystem();
            }
        }
        
        private System.Collections.IEnumerator FindPlayerDelayed()
        {
            yield return null;
            
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerController = player.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    Debug.LogError("RoadSpawner: PlayerController component not found on player!");
                }
            }
            else
            {
                Debug.LogError("RoadSpawner: Could not find player with tag 'Player'");
            }
        }
        
        private void HandleGameFail()
        {
            isInitialized = false;
        }
        
        private void HandleGameRestart()
        {
            isInitialized = false;

            foreach (var roadPiece in activeRoads)
            {
                if (roadPiece != null)
                {
                    roadPiece.ClearObstacles();
                    roadPool.Return(roadPiece);
                }
            }

            activeRoads.Clear();
            lastSpawnZ = 0f;
        }
        
        private void InitializeRoadSystem()
        {
            isInitialized = false;
            lastSpawnZ = 0f;
            
            StartCoroutine(InitializeRoadSystemDelayed());
        }
        
        private System.Collections.IEnumerator InitializeRoadSystemDelayed()
        {
            yield return StartCoroutine(FindPlayerDelayed());
            
            if (playerTransform == null || playerController == null)
            {
                Debug.LogError("RoadSpawner: Failed to find player or its components. Cannot initialize road system.");
                yield break;
            }
            
            for (int i = 0; i < initialRoadCount; i++)
            {
                var roadPiece = roadPool.Get();
                if (roadPiece != null)
                {
                    Vector3 spawnPosition = new Vector3(0, 0, lastSpawnZ);
                    roadPiece.transform.position = spawnPosition;
                    roadPiece.gameObject.SetActive(true);
                    activeRoads.Add(roadPiece);
                    SpawnObstacles(roadPiece);
                    lastSpawnZ += roadLength;
                }
            }
            
            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized || playerTransform == null) return;

            if (playerTransform.position.z + spawnTriggerDistance > lastSpawnZ - roadLength)
            {
                SpawnRoadPiece();
            }

            for (int i = activeRoads.Count - 1; i >= 0; i--)
            {
                if (playerTransform.position.z - activeRoads[i].transform.position.z > despawnDistance)
                {
                    DespawnRoadPiece(activeRoads[i]);
                }
            }
        }

        private void SpawnRoadPiece()
        {
            var roadPiece = roadPool.Get();
            if (roadPiece == null)
            {
                Debug.LogError("RoadSpawner: Failed to get road piece from pool");
                return;
            }

            Vector3 spawnPosition = new Vector3(0, 0, lastSpawnZ);
            roadPiece.transform.position = spawnPosition;
            
            if (!roadPiece.gameObject.activeSelf)
            {
                roadPiece.gameObject.SetActive(true);
            }
            
            activeRoads.Add(roadPiece);
            SpawnObstacles(roadPiece);
            lastSpawnZ += roadLength;
        }

        private void DespawnRoadPiece(RoadPiece roadPiece)
        {
            activeRoads.Remove(roadPiece);
            roadPiece.ClearObstacles();
            roadPool.Return(roadPiece);
        }

        private void SpawnObstacles(RoadPiece roadPiece)
        {
            if (playerController == null)
            {
                Debug.LogError("RoadSpawner: Cannot spawn obstacles, PlayerController not found!");
                return;
            }

            int obstacleCount = Random.Range(1, 4);
            float laneOffset = playerController.LaneDistance / 2;
            
            for (int i = 0; i < obstacleCount; i++)
            {
                int lane = Random.Range(0, 2);
                float xPos = lane == 0 ? -laneOffset : laneOffset;

                float minZ = (roadLength * i) / obstacleCount + 2f;
                float maxZ = (roadLength * (i + 1)) / obstacleCount - 2f;
                
                Vector3 obstaclePosition = roadPiece.transform.position + new Vector3(xPos, 0f, Random.Range(minZ, maxZ));
                var obstacle = carFactory.CreateObstacleCar(obstaclePosition);
                
                if (obstacle != null)
                {
                    roadPiece.AddObstacle(obstacle);
                }
            }
        }
    }
}