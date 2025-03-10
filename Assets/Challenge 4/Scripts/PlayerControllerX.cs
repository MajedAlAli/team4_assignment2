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
    public GameObject menuPanel;  // Make sure this is public
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
            PowerupX powerupScript = other.gameObject.GetComponent<PowerupX>(); // Get powerup type
            FindAnyObjectByType<AudioManager>().Play("PowerUp");
            if (powerupScript != null)
            {
                ActivatePowerup(powerupScript.powerupType); // Activate correct powerup
            }
            Destroy(other.gameObject);

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
            case PowerupX.PowerupType.Freeze:
                StartCoroutine(FreezeEnemies());
                break;
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
        powerupStrength = normalStrength; // Reset strength after powerup ends
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


    // Powerup Effects

    IEnumerator SpeedBoost()
    {
        speed *= 2; // Double speed
        yield return new WaitForSeconds(powerUpDuration);
        speed /= 2; // Reset speed
    }
IEnumerator FreezeEnemies()
{
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    Dictionary<GameObject, float> enemySpeeds = new Dictionary<GameObject, float>();

    // Freeze enemies but allow physics interactions
    foreach (GameObject enemy in enemies)
    {
        EnemyX enemyScript = enemy.GetComponent<EnemyX>();
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

        if (enemyScript != null && enemyRb != null)
        {
            enemySpeeds[enemy] = enemyScript.speed; // Store original speed

            enemyScript.speed = 0f; // Stop enemy from chasing the goal
            enemyRb.linearVelocity = Vector3.zero;  // Stop current movement
            enemyRb.angularVelocity = Vector3.zero; // Stop rotation 
        }
    }

    yield return new WaitForSeconds(powerUpDuration);

    // Restore original speed
    foreach (GameObject enemy in enemies)
    {
        EnemyX enemyScript = enemy.GetComponent<EnemyX>();

        if (enemyScript != null)
        {
            enemyScript.speed = enemySpeeds[enemy]; // Restore original movement speed
        }
    }
}

    IEnumerator SlowDownEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Slow down enemies
        foreach (GameObject enemy in enemies)
        {
            EnemyX enemyScript = enemy.GetComponent<EnemyX>();
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

            if (enemyScript != null && enemyRb != null)
            {
                enemyScript.speed *= 0.5f; // Reduce speed by 50%
                enemyRb.linearVelocity *= 0.5f;  // Reduce current movement speed
            }
        }

        yield return new WaitForSeconds(powerUpDuration);

        // Restore original enemy speed
        foreach (GameObject enemy in enemies)
        {
            EnemyX enemyScript = enemy.GetComponent<EnemyX>();
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

            if (enemyScript != null && enemyRb != null)
            {
                enemyScript.speed *= 2f; // Restore speed
                enemyRb.linearVelocity *= 2f;  // Restore movement speed
            }
        }
    }
    IEnumerator EnableMagnet()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 directionToGoal = (enemyRb.transform.position - GameObject.Find("Enemy Goal").transform.position).normalized;
                enemyRb.linearVelocity = -directionToGoal * 10f; // Pull toward enemy goal
            }
        }

        yield return new WaitForSeconds(powerUpDuration);

        // Restore enemy movement by clearing velocity
        foreach (GameObject enemy in enemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector3.zero; // Stop movement after magnet ends
            }
        }
    }

    IEnumerator EnableJump()
    {
        float jumpForce = 20f;   // Upward force
        float smashForce = -40f; // Downward force (to smash ground)
        float explosionRadius = 10f; // How far enemies are affected
        float maxImpactForce = 40f; // Max force for nearby enemies

        // Apply upward force to jump
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        FindAnyObjectByType<AudioManager>().Play("Whoosh");

        // Wait in the air for a short time
        yield return new WaitForSeconds(0.5f);

        // Apply downward force to create a smash effect
        playerRb.AddForce(Vector3.up * smashForce, ForceMode.Impulse);
        FindAnyObjectByType<AudioManager>().Play("PowerUp Hit");
        explosionParticle.Play();

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
}
