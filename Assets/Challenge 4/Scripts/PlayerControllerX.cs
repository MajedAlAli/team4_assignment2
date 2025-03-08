﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    
    private float boost = 10;
    public ParticleSystem smokeParticle;
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);  

        if(Input.GetKeyDown(KeyCode.Space)){
            playerRb.AddForce(focalPoint.transform.forward * boost, ForceMode.Impulse);
            smokeParticle.Play();
        }

    }

    // If Player collides with powerup, activate powerup
        private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            PowerupX powerupScript = other.gameObject.GetComponent<PowerupX>(); // Get powerup type

            if (powerupScript != null)
            {
                ActivatePowerup(powerupScript.powerupType); // Activate correct powerup
            }

            Destroy(other.gameObject); // Remove powerup from scene
        }
    }

    // Activate powerup based on its type
    void ActivatePowerup(PowerupX.PowerupType type)
    {
        hasPowerup = true;
        powerupIndicator.SetActive(true);
        StartCoroutine(PowerupCooldown());

        switch (type)
        {
            case PowerupX.PowerupType.Speed:
                StartCoroutine(SpeedBoost());
                break;
            case PowerupX.PowerupType.Strength:
                powerupStrength = 25; // Increase hit force
                break;
            case PowerupX.PowerupType.Slowdown:
                StartCoroutine(SlowDownEnemies());
                break;
            case PowerupX.PowerupType.Magnet:
                StartCoroutine(EnableMagnet());
                break;
            case PowerupX.PowerupType.Jump:
                StartCoroutine(EnableJump());
                break;
        }
    }
// Powerup Effects

    IEnumerator SpeedBoost()
    {
        speed *= 2; // Double speed
        yield return new WaitForSeconds(powerUpDuration);
        speed /= 2; // Reset speed
    }

    IEnumerator SlowDownEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity *= 0.5f; // Slow down enemy movement
            }
        }
        yield return new WaitForSeconds(powerUpDuration);

        // Restore original enemy speed
        foreach (GameObject enemy in enemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity *= 2f; // Reset enemy speed
            }
        }
    }

    IEnumerator EnableMagnet()
    {
        // Example: Apply magnet effect (e.g., attract ball)
        yield return new WaitForSeconds(powerUpDuration);
    }

IEnumerator EnableJump()
{
    float jumpForce = 20f;   // Upward force
    float smashForce = -40f; // Downward force (to smash ground)
    float explosionRadius = 10f; // How far enemies are affected
    float maxImpactForce = 40f; // Max force for nearby enemies

    // Apply upward force to jump
    playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    // Wait in the air for a short time
    yield return new WaitForSeconds(0.5f);

    // Apply downward force to create a smash effect
    playerRb.AddForce(Vector3.up * smashForce, ForceMode.Impulse);

    // Wait a little before applying knockback to enemies
    yield return new WaitForSeconds(0.2f);

    // Get all enemies in range
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
    foreach (Collider hitCollider in hitColliders)
    {
        if (hitCollider.CompareTag("Enemy"))
        {
            Rigidbody enemyRb = hitCollider.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Calculate knockback force based on distance
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                float knockbackForce = Mathf.Lerp(maxImpactForce, 20f, distance / explosionRadius); // Closer = stronger

                // Apply force outward from the player
                Vector3 direction = hitCollider.transform.position - transform.position;
                enemyRb.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}


    // Coroutine to reset powerup effect after duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
        powerupStrength = normalStrength; // Reset strength after powerup ends
    }


    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer =  other.gameObject.transform.position  - transform.position; 
           
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
    }



}
