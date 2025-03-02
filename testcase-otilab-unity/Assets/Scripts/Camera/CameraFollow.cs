using UnityEngine;
using Core.Events;

namespace Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, 3, -7); // Default position behind and slightly above the car
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private Vector3 rotation = new Vector3(15, 0, 0); // Slight downward angle to see the road better
        
        private Transform playerTransform;
        private bool isFollowing = false;
        private float initialX; 

        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameFail += HandleGameFail;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameFail -= HandleGameFail;
        }

        private void Start()
        {
            initialX = transform.position.x;
        }

        private void HandleGameStart()
        {
            StartCoroutine(FindPlayerDelayed());
        }

        private void HandleGameFail()
        {
            isFollowing = false;
        }

        private System.Collections.IEnumerator FindPlayerDelayed()
        {
            yield return null;
            
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                isFollowing = true;
                
                transform.rotation = Quaternion.Euler(rotation);
                Vector3 startPos = new Vector3(initialX, playerTransform.position.y + offset.y, playerTransform.position.z + offset.z);
                transform.position = startPos;
                
                Debug.Log("CameraFollow: Found player, starting to follow");
            }
            else
            {
                Debug.LogError("CameraFollow: Could not find player with tag 'Player'");
                isFollowing = false;
            }
        }

        private void LateUpdate()
        {
            if (!isFollowing || playerTransform == null) return;

            Vector3 targetPosition = new Vector3(
                initialX, 
                playerTransform.position.y + offset.y,
                playerTransform.position.z + offset.z
            );
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }

        private void OnValidate()
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
} 