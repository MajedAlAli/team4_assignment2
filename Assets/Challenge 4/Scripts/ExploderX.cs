using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExploderX : MonoBehaviour
{
    public float speed;
    private Rigidbody enemyRb;
    private GameObject playerGoal;
    private SpawnManagerX smScript;
    private float waveCount;
    public ParticleSystem explosionParticle;
    public AudioSource ball;
    public AudioSource wall;

    // Start is called before the first frame update
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");
        smScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManagerX>();
        waveCount = smScript.waveCount;
        speed = speed + 10 * waveCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Set enemy direction towards player goal and move there
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);

    }

    private void OnCollisionEnter(Collision other)
    {
        // If enemy collides with either goal, destroy it
        if (other.gameObject.name == "Enemy Goal")
        {
            FindAnyObjectByType<AudioManager>().Play("Explode");;
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            Destroy(gameObject);
            ScoreManager.instance.AddPointHome();
            ScoreManager.instance.DisplayMessageHome();
            FindAnyObjectByType<AudioManager>().Play("Goal");
            FindAnyObjectByType<AudioManager>().Play("CrowdCheer");
        } 
        else if (other.gameObject.name == "Player Goal")
        {
            FindAnyObjectByType<AudioManager>().Play("Explode");
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            Destroy(gameObject);
            ScoreManager.instance.AddPointAway();
            ScoreManager.instance.DisplayMessageAway();
            FindAnyObjectByType<AudioManager>().Play("Goal");
            FindAnyObjectByType<AudioManager>().Play("CrowdBoo");

        }else if (other.gameObject.CompareTag("Wall"))
        {
            wall.Play();
            
        }else if (other.gameObject.CompareTag("Enemy"))
        {
            ball.Play();
        } else if (other.gameObject.CompareTag("Player"))
        {
            FindAnyObjectByType<AudioManager>().Play("Explode");
        float explosionRadius = 10f; // How far enemies are affected
        float maxImpactForce = 40f; // Max force for nearby enemies


        // Get all enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hitCollider in hitColliders)
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

            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            FindAnyObjectByType<AudioManager>().Play("CrowdBoo");
            Destroy(gameObject);
        }

    }
    }
}
