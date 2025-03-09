using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExploderX : MonoBehaviour
{
    public float speed = 8f; // Movement speed
    public float explosionForce = 20f; // Explosion force
    public float explosionRadius = 5f; // Explosion area of effect
    public GameObject explosionEffectPrefab; // Particle effect for explosion

    private Rigidbody exploderRb;
    private GameObject player;
    private SpawnManagerX smScript;
    private float waveCount;
    public ParticleSystem explosionParticle;
    public AudioSource ball;
    public AudioSource wall;
    void Start()
    {
        exploderRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player"); // Make sure Player is tagged as "Player"

        if (exploderRb == null)
        {
            Debug.LogError(" Exploder is missing a Rigidbody! Add one in the Inspector.");
        }

        if (explosionEffectPrefab == null)
        {
            Debug.LogError(" Explosion Effect is not assigned! Drag 'Explosionn' prefab into the ExploderX script in the Inspector.");
        }
    }
        // Start is called before the first frame update

    void Update()
    {
        // Move towards the player goal
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        exploderRb.AddForce(lookDirection * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(" Exploder collided with: " + collision.gameObject.name + " | Tag: " + collision.gameObject.tag);

        // Only explode if colliding with the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(" Exploder HIT THE PLAYER! Initiating explosion...");
            Explode();
        }
    }



    void Explode()
    {
        Debug.Log(" Exploder Enemy exploded!");

        // Instantiate explosion effect (if assigned)
        if (explosionEffectPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 2f); // Destroy explosion effect after 2 seconds
            Debug.Log(" Explosion Effect Created!");
        }
        else
        {
            Debug.LogError(" No Explosion Effect Assigned!");
        }

        // Find all nearby objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log(" Applying Explosion Force to: " + nearbyObject.name);
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        // Destroy the exploder enemy after explosion
        Destroy(gameObject);
    }

}

