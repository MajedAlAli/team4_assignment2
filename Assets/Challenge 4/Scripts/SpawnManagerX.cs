using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject heavyEnemyPrefab;
    public GameObject speedsterPrefab;
    public GameObject tricksterPrefab;
    public GameObject exploderPrefab; // Exploder Prefab
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
            Debug.Log("Spawned Normal Enemy");
        }

        // Spawn heavy enemies every 3 rounds
        if (waveCount % 3 == 0)
        {
            int heavyEnemyCount = waveCount / 3;
            for (int i = 0; i < heavyEnemyCount; i++)
            {
                Instantiate(heavyEnemyPrefab, GenerateSpawnPosition(), heavyEnemyPrefab.transform.rotation);
                Debug.Log("Spawned Heavy Enemy");
            }
        }

        // Spawn speedster enemies every 4 rounds
        if (waveCount % 4 == 0)
        {
            int speedsterCount = waveCount / 4;
            for (int i = 0; i < speedsterCount; i++)
            {
                Instantiate(speedsterPrefab, GenerateSpawnPosition(), speedsterPrefab.transform.rotation);
                Debug.Log("Spawned Speedster Enemy");
            }
        }

        // Spawn trickster enemies every 5 rounds
        if (waveCount % 5 == 0)
        {
            int tricksterCount = waveCount / 5;
            for (int i = 0; i < tricksterCount; i++)
            {
                Instantiate(tricksterPrefab, GenerateSpawnPosition(), tricksterPrefab.transform.rotation);
                Debug.Log("Spawned Trickster Enemy!");
            }
        }

        // 💥 Spawn exploder enemies every 6 rounds
        if (waveCount % 6 == 0)
        {
            int exploderCount = waveCount / 6;
            for (int i = 0; i < exploderCount; i++)
            {
                Instantiate(exploderPrefab, GenerateSpawnPosition(), exploderPrefab.transform.rotation);
                Debug.Log("Spawned Exploder Enemy!");
            }
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

