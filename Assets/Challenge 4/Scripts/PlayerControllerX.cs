using System.Collections;
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
    private bool isRolling = false;

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
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.4f, 0);  

        if(Input.GetKeyDown(KeyCode.Space)){
            FindAnyObjectByType<AudioManager>().Play("Boost");
            playerRb.AddForce(focalPoint.transform.forward * boost, ForceMode.Impulse);
            smokeParticle.Play();
        }
        HandleRollingSound();
    }

    void HandleRollingSound()
    {
        if (playerRb.linearVelocity.magnitude > 0.1f)
        {
            if (!isRolling)
            {
                FindAnyObjectByType<AudioManager>().Play("Rolling");
                isRolling = true;
            }
        }
        else
        {
            if (isRolling)
            {
                FindAnyObjectByType<AudioManager>().Stop("Rolling");
                isRolling = false;
            }
        }
    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            FindAnyObjectByType<AudioManager>().Play("CollisionBall");
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


        }else if (other.gameObject.CompareTag("Wall")||other.gameObject.CompareTag("Goal"))
        {
            FindAnyObjectByType<AudioManager>().Play("CollisionWall");
        }
    }



}
