using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject heavyEnemyPrefab;
    public GameObject speedsterPrefab;
    public GameObject tricksterPrefab;
    public GameObject exploderPrefab;
    public GameObject powerupPrefab;

    private float spawnRangeX = 10;
    private float spawnZMin = 15;
    private float spawnZMax = 25;

    public int enemyCount;
    public int waveCount = 1;

    public GameObject player;

    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveCount);
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(spawnZMin, spawnZMax);
        return new Vector3(xPos, 0, zPos);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 0, -15);

        if (GameObject.FindGameObjectsWithTag("Powerup").Length == 0)
        {
            Instantiate(powerupPrefab, GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab.transform.rotation);
        }

        Debug.Log("Wave Count: " + waveCount);

        // Spawn normal enemies
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
            Debug.Log(" Spawned Normal Enemy");
        }

        //  After Round 3, spawn one more Heavy Enemy each wave
        if (waveCount >= 3)
        {
            Instantiate(heavyEnemyPrefab, GenerateSpawnPosition(), heavyEnemyPrefab.transform.rotation);
            Debug.Log(" Spawned Heavy Enemy!");
        }

        //  After Round 3, spawn one more Speedster each wave
        if (waveCount >= 3)
        {
            Instantiate(speedsterPrefab, GenerateSpawnPosition(), speedsterPrefab.transform.rotation);
            Debug.Log(" Spawned Speedster Enemy!");
        }

        //  After Round 3, spawn one more Trickster each wave
        if (waveCount >= 3)
        {
            Instantiate(tricksterPrefab, GenerateSpawnPosition(), tricksterPrefab.transform.rotation);
            Debug.Log(" Spawned Trickster Enemy!");
        }

        //  After Round 3, spawn one more Exploder each wave
        if (waveCount >= 3)
        {
            Instantiate(exploderPrefab, GenerateSpawnPosition(), exploderPrefab.transform.rotation);
            Debug.Log(" Spawned Exploder Enemy!");
        }

        waveCount++;
        ResetPlayerPosition();
    }

    void ResetPlayerPosition()
    {
        player.transform.position = new Vector3(0, 1, -7);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
