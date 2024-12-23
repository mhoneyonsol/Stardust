using System.Collections;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    [Range(0, 9.5f)]
    public float speed = 8.0f;
    [Range(0, 2.0f)]
    public float changeLanesSpeed = 2.0f;
    public float shieldDuration = 5f;

    private Vector2 touchStart;
    private float minSwipeDistance = 50f;
    public TMPro.TextMeshProUGUI shieldTimerText; 
    public Transform parts;
    public GameObject crashedParticles;
    public GameObject shieldVisualEffect; // Add this reference
    
    private int lane = 0;
    private bool changingLanes;
    private float duration;
    private Vector3 startPos, endPos;
    private bool paused;
    public bool isShieldActive = false;
    private float shieldTimer = 0f;

    private Animation anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = this.GetComponent<Animation>();
        audioSource = this.GetComponent<AudioSource>();
        LoadRocket();
        UpdatePosition();
        UpdateObstaclesSpeed(speed);
        if(shieldVisualEffect != null)
            shieldVisualEffect.SetActive(false);
    }

    void Update()
    {
        if(!paused)
        {
            if(transform.position != endPos)
            {
                changingLanes = true;
            }
            else
            {
                changingLanes = false;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }

            HandleSwipeInput();

            if(changingLanes)
            {
                if(changeLanesSpeed != 0)
                {
                    duration += Time.deltaTime/((2-changeLanesSpeed)/10);
                    transform.position = Vector3.Lerp(startPos, endPos, duration);
                }
            }

            // Shield timer update
            if (isShieldActive)
{
    shieldTimer -= Time.deltaTime;
    // Update UI timer (rounded to 1 decimal place)
    if (shieldTimerText != null)
    {
        shieldTimerText.text = $"Shield: {shieldTimer:F1}s";
    }
    if (shieldTimer <= 0)
    {
        DeactivateShield();
    }
}
        }
    }

    public void ActivateShield()
{
    isShieldActive = true;
    shieldTimer = shieldDuration;
    if (shieldVisualEffect != null)
        shieldVisualEffect.SetActive(true);
    if (shieldTimerText != null)
        shieldTimerText.gameObject.SetActive(true);  // Show timer
    Debug.Log("Shield Activated!");
}

    private void DeactivateShield()
{
    isShieldActive = false;
    if (shieldVisualEffect != null)
        shieldVisualEffect.SetActive(false);
    if (shieldTimerText != null)
        shieldTimerText.gameObject.SetActive(false);  // Hide timer
    Debug.Log("Shield Deactivated!");
}

    private void HandleSwipeInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Ended:
                    Vector2 swipeDelta = touch.position - touchStart;
                    if (Mathf.Abs(swipeDelta.x) > minSwipeDistance)
                    {
                        if (swipeDelta.x > 0)
                        {
                            MoveRight();
                        }
                        else
                        {
                            MoveLeft();
                        }
                    }
                    break;
            }
        }
    }

    public void MoveLeft()
    {
        if(lane > -2)
        {
            anim.Play("Move-Left");
            audioSource.Play();
            lane--;
            UpdatePosition();             
        }       
    }

    public void MoveRight()
    {
        if(lane < 2)
        {
            anim.Play("Move-Right");
            audioSource.Play();
            lane++;
            UpdatePosition();   
        }     
    }

    private void LoadRocket()
    {
        foreach(Transform part in parts)
        {
            if(part.name != "Base")
            {
                bool partAdded = PlayerPrefs.GetInt("PartAdded-" + part.name, 0) == 1 ? true : false;
                part.gameObject.SetActive(partAdded);
            }
        }
    }

    private void UpdatePosition()
    {
        duration = 0;
        startPos = transform.position;
        endPos = new Vector3(lane, transform.position.y, transform.position.z);
    }

    private void UpdateObstaclesSpeed(float obstaclesSpeed)
    {
        ObstaclesLine.speed = obstaclesSpeed;
    }

    public void Pause()
    {
        paused = true;
        UpdateObstaclesSpeed(0);
    }

    public void Resume()
    {
        paused = false;
        UpdateObstaclesSpeed(speed);
    }

    public void Crashed()
    {
        Pause();
        crashedParticles.SetActive(true);
        
        #if UNITY_ANDROID || UNITY_IOS
            if(Settings.GetSetting("Vibration"))
            {
                Handheld.Vibrate();
            }
        #endif
    }
}