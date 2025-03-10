using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TricksterX : MonoBehaviour
{
    public float dashForce = 15f; // How strong the dash is
    public float dashInterval = 8f; // How often it dashes

    private Rigidbody tricksterRb;
    private GameObject playerGoal;
    public float speed;
    private SpawnManagerX smScript;
    private float waveCount;
    public ParticleSystem explosionParticle;
    public AudioSource ball;
    public AudioSource wall;
    void Start()
    {
        tricksterRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");
        smScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManagerX>();
        waveCount = smScript.waveCount;
        speed = speed + 10 * waveCount;
        // Start the random dashing coroutine
        StartCoroutine(DashRandomly());
    }

    void Update()
    {
        // Move towards the player goal normally
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        tricksterRb.AddForce(lookDirection * speed * Time.deltaTime);
    }

    IEnumerator DashRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(dashInterval); // Wait before dashing

            // Choose a random dash direction
            Vector3 dashDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            // Apply sudden force in that direction
            tricksterRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            Debug.Log("Trickster dashed!");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // If enemy collides with either goal, destroy it
        if (other.gameObject.name == "Enemy Goal")
        {
            FindAnyObjectByType<AudioManager>().Play("Explode");

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
        }

    }
}
