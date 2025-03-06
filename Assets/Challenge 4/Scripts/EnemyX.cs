using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyX : MonoBehaviour
{
    public float speed;
    private Rigidbody enemyRb;
    private GameObject playerGoal;
    private SpawnManagerX smScript;
    private float waveCount;
    public ParticleSystem explosionParticle;

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
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            ScoreManager.instance.AddPointHome();
            ScoreManager.instance.DisplayMessageHome();
            FindObjectOfType<AudioManager>().Play("Goal");
        } 
        else if (other.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            ScoreManager.instance.AddPointAway();
            ScoreManager.instance.DisplayMessageAway();
            FindObjectOfType<AudioManager>().Play("Goal");
        }else if (other.gameObject.CompareTag("Wall"))
        {
            FindObjectOfType<AudioManager>().Play("CollisionWall");
        }else if (other.gameObject.CompareTag("Enemy"))
        {
            FindObjectOfType<AudioManager>().Play("CollisionBall");
        }

    }

}
