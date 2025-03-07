using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    public float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup
    
    private float boost = 10f;
    public bool gameIsActive;

    void Start()
    {
        gameIsActive = true;
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

        if(Input.GetKeyDown(KeyCode.Space)&&gameIsActive){
            playerRb.AddForce(focalPoint.transform.forward * boost, ForceMode.Impulse);
            FindAnyObjectByType<AudioManager>().Play("Boost");
        }

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            FindAnyObjectByType<AudioManager>().Play("PowerUp");
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        FindAnyObjectByType<AudioManager>().Play("PowerUp CountDown");
        yield return new WaitForSeconds(powerUpDuration);
        FindAnyObjectByType<AudioManager>().Stop("PowerUp CountDown");
        hasPowerup = false;
        powerupIndicator.SetActive(false);
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
                FindAnyObjectByType<AudioManager>().Play("PowerUp Hit");
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                FindAnyObjectByType<AudioManager>().Play("CollisionBall");
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }else if (other.gameObject.CompareTag("Wall")||other.gameObject.CompareTag("Goal"))
        {
            FindAnyObjectByType<AudioManager>().Play("CollisionWall");
        }
    }

    public void DeactivatePowerUp()
    {
        if(!gameIsActive)
        {
            FindAnyObjectByType<AudioManager>().Stop("PowerUp CountDown");
            powerupIndicator.SetActive(false);
            hasPowerup = false;
        }
    }
}
