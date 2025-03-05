using System.Collections;
using UnityEngine;

public class TricksterX : MonoBehaviour
{
    public float speed = 10f;
    public float dashForce = 15f; // How strong the dash is
    public float dashInterval = 3f; // How often it dashes

    private Rigidbody tricksterRb;
    private GameObject playerGoal;

    void Start()
    {
        tricksterRb = GetComponent<Rigidbody>();
        playerGoal = GameObject.Find("Player Goal");

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
        if (other.gameObject.name == "Enemy Goal" || other.gameObject.name == "Player Goal")
        {
            Destroy(gameObject);
        }
    }
}
