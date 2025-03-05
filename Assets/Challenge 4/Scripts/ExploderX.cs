using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderX : MonoBehaviour
{
    public float speed = 8f; // Movement speed
    public float explosionForce = 15f; // How strong the explosion is
    public float explosionRadius = 5f; // The area of effect of the explosion
    public GameObject explosionEffect; // Optional particle effect for explosion

    private Rigidbody exploderRb;
    private GameObject playerGoal;

    void Start()
    {
        exploderRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");
    }

    void Update()
    {
        // Move towards the player goal
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        exploderRb.AddForce(lookDirection * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        // If colliding with a goal, explode
        if (other.gameObject.name == "Enemy Goal" || other.gameObject.name == "Player Goal")
        {
            Explode();
        }
    }

    void Explode()
    {
        Debug.Log("Exploder Enemy exploded!");

        // Instantiate explosion effect (if assigned)
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Find all nearby objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply explosion force
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }

        // Destroy the exploder enemy after explosion
        Destroy(gameObject);
    }
}
