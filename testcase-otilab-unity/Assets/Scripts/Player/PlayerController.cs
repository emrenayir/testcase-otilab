using UnityEngine;
using Core.Events;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float laneDistance = 5f;
        [SerializeField] private float laneChangeSpeed = 10f;
        [SerializeField] private float rotationAngle = 15f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float minSwipeDistance = 50f;

        public float LaneDistance => laneDistance;

        private int currentLane = 0;
        private float targetX;
        private float currentRotation;
        private float targetRotation;
        private bool isChangingLane;
        private bool canControl = false;
        private Vector2 touchStartPos;
        private bool isSwiping = false;

        private void OnEnable()
        {
            GameEvents.OnGameStart += HandleGameStart;
            GameEvents.OnGameFail += HandleGameFail;
            GameEvents.OnGamePause += HandleGamePause;
            GameEvents.OnGameResume += HandleGameResume;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= HandleGameStart;
            GameEvents.OnGameFail -= HandleGameFail;
            GameEvents.OnGamePause -= HandleGamePause;
            GameEvents.OnGameResume -= HandleGameResume;
        }

        private void Start()
        {
            ResetController();
        }

        private void Update()
        {
            if (!canControl) return;

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TryChangeLane(-1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                TryChangeLane(1);
            }

            HandleTouchInput();

            float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
            newX = Mathf.Clamp(newX, -laneDistance/2, laneDistance/2);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            float targetRotationClamped = Mathf.Clamp(targetRotation, -rotationAngle, rotationAngle);
            currentRotation = Mathf.LerpAngle(currentRotation, targetRotationClamped, rotationSpeed * Time.deltaTime);
            currentRotation = Mathf.Clamp(currentRotation, -rotationAngle, rotationAngle);
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
            {
                targetRotation = 0f;
                currentRotation = 0f;
                transform.rotation = Quaternion.identity;
                isChangingLane = false;
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Ended:
                        if (isSwiping)
                        {
                            ProcessSwipe(touch.position - touchStartPos);
                            isSwiping = false;
                        }
                        break;

                    case TouchPhase.Canceled:
                        isSwiping = false;
                        break;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    touchStartPos = Input.mousePosition;
                    isSwiping = true;
                }
                else if (Input.GetMouseButtonUp(0) && isSwiping)
                {
                    ProcessSwipe((Vector2)Input.mousePosition - touchStartPos);
                    isSwiping = false;
                }
            }
        }

        private void ProcessSwipe(Vector2 swipeDelta)
        {
            if (!canControl || isChangingLane) return;

            float swipeDistance = swipeDelta.magnitude;
            if (swipeDistance > minSwipeDistance)
            {
                if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                {
                    TryChangeLane(swipeDelta.x > 0 ? 1 : -1);
                }
            }
        }

        private void TryChangeLane(int direction)
        {
            int newLane = currentLane + direction;
            
            if (newLane >= 0 && newLane <= 1 && !isChangingLane)
            {
                ChangeLane(newLane);
            }
        }

        private void ChangeLane(int newLane)
        {
            if (isChangingLane) return;
            
            currentLane = newLane;
            targetX = (currentLane == 0) ? -laneDistance / 2 : laneDistance / 2;
            targetRotation = (currentLane == 0) ? -rotationAngle : rotationAngle;
            isChangingLane = true;
        }

        private void HandleGameStart()
        {
            canControl = false;
        }

        private void HandleGameFail()
        {
            canControl = false;
        }

        private void HandleGamePause()
        {
            canControl = false;
        }

        private void HandleGameResume()
        {
            canControl = true;
        }

        public void EnableControl()
        {
            canControl = true;
        }

        public void ResetController()
        {
            targetX = transform.position.x;
            currentLane = 0;
            targetX = -laneDistance / 2;
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            
            targetRotation = 0f;
            currentRotation = 0f;
            transform.rotation = Quaternion.identity;
            
            isChangingLane = false;
            canControl = false;
            isSwiping = false;
        }
    }
}