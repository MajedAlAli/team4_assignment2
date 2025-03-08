using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10;
    private float powerupStrength = 25;

    // ✅ Adjusted Magnet Boost Variables
    public bool hasMagnetBoost = false;
    public float magnetStrength = 8f;  // 🔹 Lowered pull strength significantly
    public float magnetDuration = 500f;
    public float pullDistanceThreshold = 1000f;  // 🔹 Enemies get pulled within 12 units
    public float maxPullSpeed = 10000f;  // 🔹 Limit max speed to prevent flying

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        Debug.Log("🧲 Magnet Duration at Start: " + magnetDuration);
    }


    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * Time.deltaTime);

        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        // ✅ Space Key Boost
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space Pressed - Boost Activated!");
            playerRb.AddForce(focalPoint.transform.forward * 10, ForceMode.Impulse);
        }

        // ✅ Apply Magnet Boost Effect
        if (hasMagnetBoost)
        {
            PullEnemiesToPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            if (other.gameObject.name.Contains("MagnetBoost"))
            {
                hasMagnetBoost = true;
                powerupIndicator.SetActive(true);
                Destroy(other.gameObject);
                StartCoroutine(MagnetBoostCooldown());
                Debug.Log("🧲 Magnet Boost Activated!");
            }
            else
            {
                hasPowerup = true;
                powerupIndicator.SetActive(true);
                Destroy(other.gameObject);
                StartCoroutine(PowerupCooldown());
                Debug.Log("⚡ Normal Powerup Activated!");
            }
        }
    }

    // ✅ Magnet Boost Lasts 20 Seconds
    IEnumerator MagnetBoostCooldown()
    {
        Debug.Log("🧲 Magnet Boost Started! Expected duration: " + magnetDuration + " seconds");

        float elapsedTime = 0;
        while (elapsedTime < magnetDuration)
        {
            yield return new WaitForSeconds(1); // Wait for 1 second each loop
            elapsedTime += 1;
            Debug.Log("⏳ Magnet Boost Active... " + elapsedTime + "/" + magnetDuration + " seconds passed.");
        }

        hasMagnetBoost = false;
        powerupIndicator.SetActive(false);
        Debug.Log("🧲 Magnet Boost Expired after " + magnetDuration + " seconds.");
    }


    // ✅ **Balanced Magnet Pull Effect**
    void PullEnemiesToPlayer()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                Vector3 direction = (transform.position - enemy.transform.position).normalized;
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance <= pullDistanceThreshold) // 🔹 Only pull enemies within range
                {
                    float forceAmount = Mathf.Lerp(0, magnetStrength, 1 - (distance / pullDistanceThreshold)); // 🔹 Smooth force scaling
                    enemyRb.AddForce(direction * forceAmount, ForceMode.Acceleration);  // 🔹 Use Acceleration for smoother pull

                    // 🔹 Limit max speed to prevent enemies flying uncontrollably
                    if (enemyRb.linearVelocity.magnitude > maxPullSpeed)
                    {
                        enemyRb.linearVelocity = enemyRb.linearVelocity.normalized * maxPullSpeed;
                    }

                    Debug.Log("🧲 Softly Pulling " + enemy.name + " towards player!");
                }
            }
        }
    }

    // ✅ Normal Powerup Cooldown
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
        Debug.Log("⚡ Powerup Expired!");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

            if (hasPowerup)
            {
                enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else
            {
                enemyRb.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }
        }
    }
}

