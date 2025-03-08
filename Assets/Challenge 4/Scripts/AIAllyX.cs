using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAllyX : MonoBehaviour
{
    public float speed = 15f;
    public float duration = 10f; // Ally exists for 10 seconds
    private Rigidbody allyRb;
    private GameObject targetEnemy;

    void Start()
    {
        allyRb = GetComponent<Rigidbody>();
        StartCoroutine(DestroyAllyAfterTime()); // Auto-destroy after duration
    }

    void Update()
    {
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            allyRb.AddForce(direction * speed * Time.deltaTime, ForceMode.Impulse);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        targetEnemy = nearestEnemy;
    }

    IEnumerator DestroyAllyAfterTime()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
