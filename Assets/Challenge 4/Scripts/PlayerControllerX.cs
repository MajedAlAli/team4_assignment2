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

    public bool hasSmashPowerup = false;
    public float smashHeight = 5f;
    public float smashForce = 30f;
    public float smashRadius = 10f;
    public GameObject smashpowerupIndicator;
    public ParticleSystem explosionParticle;


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
        smashpowerupIndicator.transform.position = transform.position + new Vector3(0, -0.4f, 0);
        explosionParticle.transform.position = transform.position + new Vector3(0, 0, 0);

        if(Input.GetKeyDown(KeyCode.Space)&&gameIsActive){
            playerRb.AddForce(focalPoint.transform.forward * boost, ForceMode.Impulse);
            FindAnyObjectByType<AudioManager>().Play("Boost");
        }

        if (Input.GetKeyDown(KeyCode.E) && hasSmashPowerup)
        {
            StartCoroutine(SmashAttack());
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
        else if (other.gameObject.CompareTag("PowerupSmash")) // New powerup
        {
            FindAnyObjectByType<AudioManager>().Play("PowerUp");
            Destroy(other.gameObject);
            hasSmashPowerup = true;
            smashpowerupIndicator.SetActive(true);
            StartCoroutine(SmashPowerupCooldown());
        }
    }

    IEnumerator SmashAttack()
    {
        if (hasSmashPowerup)
        {
            hasSmashPowerup = false;
            FindAnyObjectByType<AudioManager>().Stop("PowerUp CountDown");
            FindAnyObjectByType<AudioManager>().Play("Whoosh");
            playerRb.linearVelocity = new Vector3(0, smashHeight, 0); // Jump up
            yield return new WaitForSeconds(0.5f); // Wait before slamming down

            playerRb.linearVelocity = new Vector3(0, -smashHeight * 2, 0); // Slam down

            yield return new WaitUntil(() => playerRb.linearVelocity.y == 0); // Wait until grounded

            smashpowerupIndicator.SetActive(false);
            FindAnyObjectByType<AudioManager>().Play("PowerUp Hit");
            explosionParticle.Play();

            Collider[] enemies = Physics.OverlapSphere(transform.position, smashRadius);
            foreach (Collider enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                    if (enemyRb != null)
                    {
                        float distance = Vector3.Distance(transform.position, enemy.transform.position);
                        float force = Mathf.Lerp(smashForce, 5f, distance / smashRadius); // Stronger force if closer
                        Vector3 direction = (enemy.transform.position - transform.position).normalized;

                        enemyRb.AddForce(direction * force, ForceMode.Impulse);
                    }
                }
            }
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

    IEnumerator SmashPowerupCooldown()
    {
        FindAnyObjectByType<AudioManager>().Play("PowerUp CountDown");
        yield return new WaitForSeconds(powerUpDuration);
        FindAnyObjectByType<AudioManager>().Stop("PowerUp CountDown");
        hasSmashPowerup = false;
        smashpowerupIndicator.SetActive(false);
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
            hasPowerup = false;
            hasSmashPowerup = false;
            powerupIndicator.SetActive(false);
            smashpowerupIndicator.SetActive(false);
        }
    }
}
