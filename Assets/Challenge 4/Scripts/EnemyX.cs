using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyX : MonoBehaviour
{
    // Enemy movement speed
    public float speed;
    
    // Rigidbody component for physics-based movement
    private Rigidbody enemyRb;
    
    // Reference to the player's goal
    private GameObject playerGoal;
    
    // Reference to the SpawnManagerX script to access wave count
    private SpawnManagerX smScript;
    
    // Stores the current wave count
    private float waveCount;
    
    // Particle effect for explosion
    public ParticleSystem explosionParticle;
    
    // Audio sources for different collision sounds
    public AudioSource ball;
    public AudioSource wall;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component attached to the enemy
        enemyRb = GetComponent<Rigidbody>();
        
        // Find the player's goal in the scene
        playerGoal = GameObject.Find("Player Goal");
        
        // Get the SpawnManagerX script from the Spawn Manager object
        smScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManagerX>();
        
        // Retrieve the current wave count from the spawn manager
        waveCount = smScript.waveCount;
        
        // Increase enemy speed based on the wave count
        speed = speed + 10 * waveCount;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate direction towards the player's goal
        Vector3 lookDirection = (playerGoal.transform.position - transform.position).normalized;
        
        // Apply force to move the enemy in that direction
        enemyRb.AddForce(lookDirection * speed * Time.deltaTime);
    }

    // Called when the enemy collides with another object
    private void OnCollisionEnter(Collision other)
    {
        // If the enemy collides with the opponent's goal (Enemy Goal)
        if (other.gameObject.name == "Enemy Goal")
        {
            // Play explosion sound
            FindAnyObjectByType<AudioManager>().Play("Explode");
            
            // Destroy the enemy object
            Destroy(gameObject);
            
            // Instantiate explosion particle effect at the enemy's position
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            
            // Award a point to the home team and display goal message
            ScoreManager.instance.AddPointHome();
            ScoreManager.instance.DisplayMessageHome();
            
            // Play goal sound effects
            FindAnyObjectByType<AudioManager>().Play("Goal");
            FindAnyObjectByType<AudioManager>().Play("CrowdCheer");
        } 
        // If the enemy collides with the player's goal (Player Goal)
        else if (other.gameObject.name == "Player Goal")
        {
            // Play explosion sound
            FindAnyObjectByType<AudioManager>().Play("Explode");
            
            // Destroy the enemy object
            Destroy(gameObject);
            
            // Instantiate explosion particle effect at the enemy's position
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            
            // Award a point to the away team and display goal message
            ScoreManager.instance.AddPointAway();
            ScoreManager.instance.DisplayMessageAway();
            
            // Play goal sound effects (booing for away team goal)
            FindAnyObjectByType<AudioManager>().Play("Goal");
            FindAnyObjectByType<AudioManager>().Play("CrowdBoo");
        }
        // If the enemy collides with a wall
        else if (other.gameObject.CompareTag("Wall"))
        {
            // Play wall collision sound effect
            wall.Play();
        }
        // If the enemy collides with another enemy
        else if (other.gameObject.CompareTag("Enemy"))
        {
            // Play ball collision sound effect
            ball.Play();
        }
    }
}
