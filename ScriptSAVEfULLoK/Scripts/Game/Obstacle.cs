using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float speed;
    private bool crashed;
    private AudioSource audioSource;

    void Start()
    {
        speed = Random.Range(-1.0f, 1.0f);
        audioSource = this.GetComponent<AudioSource>();
        crashed = false;
    }

    void Update()
    {
        transform.Rotate(0, 0, speed, Space.Self); 
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!crashed)
        {
            if(col.name == "Front" || col.name == "Back")
            {
                // Search for RocketController in the parent hierarchy
                Transform current = col.transform;
                RocketController rocket = null;
                
                while (current != null && rocket == null)
                {
                    rocket = current.GetComponent<RocketController>();
                    current = current.parent;
                }

                Debug.Log($"Found Rocket: {rocket != null}, Shield Active: {rocket?.isShieldActive}");

                if (rocket != null && rocket.isShieldActive)
                {
                    Debug.Log("Shield active - destroying obstacle");
                    Destroy(gameObject);
                    return;
                }

                audioSource.Play();
                speed = 0;
                GameOver.instance.Crashed();
                crashed = true;
            }
        }
    }
}