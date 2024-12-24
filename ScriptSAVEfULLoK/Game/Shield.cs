﻿using UnityEngine;

public class Shield : MonoBehaviour
{
    private Animation anim;
    private AudioSource audioSource;
    private bool taken;

    void Start()
    {
        anim = this.GetComponent<Animation>();
        audioSource = this.GetComponent<AudioSource>();
        taken = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!taken)
        {
            if(col.name == "Front" || col.name == "Back")
            {
                // Search for RocketController in the parent hierarchy
                Transform current = col.transform;
                RocketController rocket = null;
                
                while (current != null && rocket == null)
                {
                    rocket = current.GetComponent<RocketController>();
                    if (rocket == null)
                        current = current.parent;
                }

                if (rocket != null)
                {
                    Debug.Log("Shield collected - activating shield!");
                    rocket.ActivateShield();
                    
                    if (anim != null)
                        anim.Play("Shield-Collect");
                    
                    if (audioSource != null)
                        audioSource.Play();
                    
                    taken = true;
                    Destroy(gameObject, 0.2f);
                }
            }
        }
    }
}