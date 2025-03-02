using UnityEngine;
using Core.Events;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float laneDistance = 5f;
        [SerializeField] private float laneChangeSpeed = 10f;
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private float laneChangeRotationAngle = 15f;
        [SerializeField] private float rotationResetSpeed = 20f;
        [SerializeField] private float rotationResetThreshold = 0.8f;

        public float LaneDistance => laneDistance;
        public float ForwardSpeed
        {
            get { return forwardSpeed; }
            set { forwardSpeed = value; }
        }

        private int currentLane = 0;
        private float targetX;
        private float previousX;
        private bool isChangingLane;
        private bool canControl = false;
        private Vector2 touchStartPos;
        private bool isSwiping = false;
        private bool isMoving = false;
        private bool isWaitingForInput = false;

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
            CheckForClickToStart();
            
            if (!canControl) return;
            
            if (isMoving)
            {
                transform.Translate(Vector3.forward * (forwardSpeed * Time.smoothDeltaTime));
            }
            
            HandleTouchInput();

            previousX = transform.position.x;
            
            float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
            newX = Mathf.Clamp(newX, -laneDistance/2, laneDistance/2);
            
            Vector3 worldPosition = transform.position;
            worldPosition.x = newX;
            transform.position = worldPosition;

            float laneChangeProgress = 1f - (Mathf.Abs(transform.position.x - targetX) / (laneDistance));
            laneChangeProgress = Mathf.Clamp01(laneChangeProgress);

            if (isChangingLane)
            {
                float movementDirection = newX - previousX;
                if (Mathf.Abs(movementDirection) > 0.001f)
                {
                    if (laneChangeProgress < rotationResetThreshold)
                    {
                        float targetRotationY = Mathf.Sign(movementDirection) * laneChangeRotationAngle;
                        Quaternion targetRotation = Quaternion.Euler(0, targetRotationY, 0);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * laneChangeSpeed);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationResetSpeed);
                    }
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationResetSpeed);
            }

            if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
            {
                isChangingLane = false;
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
            canControl = true;
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
            isChangingLane = true;
        }

        private void HandleGameStart()
        {
            canControl = false;
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

        private void HandleGameFail()
        {
            canControl = false;
            isMoving = false;
            isWaitingForInput = false;
        }

        private void HandleGamePause()
        {
            canControl = false;
            isMoving = false;
        }

        private void HandleGameResume()
        {
            canControl = true;
            isMoving = true;
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
            
            Vector3 worldPosition = transform.position;
            worldPosition.x = targetX;
            transform.position = worldPosition;
            
            transform.rotation = Quaternion.identity;
            
            isChangingLane = false;
            canControl = false;
            isSwiping = false;
            isMoving = false;
            isWaitingForInput = false;
        }
    }
}