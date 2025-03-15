using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject focalPoint;
    public GameObject smashPowerupPrefab; // New smash power-up
    public float smashPowerupChance = 0.3f; // 30% chance to spawn per wave


    private float spawnRangeX = 10;
    private float spawnZMin = 15; // set min spawn Z
    private float spawnZMax = 25; // set max spawn Z

    public int enemyCount;
    public int waveCount = 1;
    public GameObject[] powerupPrefabs;

    private int lastPowerupIndex = -1; // Stores the last powerup index
    public GameObject player; 

    // Update is called once per frame
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (enemyCount == 0)
        {
            SpawnEnemyWave(waveCount);
        }

    }

    // Generate random spawn position for powerups and enemy balls
    Vector3 GenerateSpawnPosition ()
    {
        float xPos = Random.Range(-spawnRangeX, spawnRangeX);
        float zPos = Random.Range(spawnZMin, spawnZMax);
        return new Vector3(xPos, 0, zPos);
    }

        void SpawnEnemyWave(int enemiesToSpawn)
    {
        Vector3 powerupSpawnOffset = new Vector3(0, 0, -15); // Make powerups spawn at player end

        // Destroy existing powerup before spawning a new one
        GameObject existingPowerup = GameObject.FindGameObjectWithTag("Powerup");
        if (existingPowerup != null)
        {
            Destroy(existingPowerup);
        }

        // Pick a new powerup that is different from the last one
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, powerupPrefabs.Length);
        } 
        while (randomIndex == lastPowerupIndex); // Ensure it's different

        lastPowerupIndex = randomIndex; // Store the new powerup index

        // Spawn the new powerup
        Instantiate(powerupPrefabs[randomIndex], GenerateSpawnPosition() + powerupSpawnOffset, Quaternion.identity);

        // Spawn number of enemy balls based on wave number
        for (int i = 0; i < enemiesToSpawn ; i++)
        {
            randomIndex = Random.Range(0, enemyPrefabs.Length);
            Instantiate(enemyPrefabs[randomIndex], GenerateSpawnPosition(), Quaternion.identity);
        }

        waveCount++;
        ResetPlayerPosition(); // put player back at start

    }

    // Move player back to position in front of own goal
    void ResetPlayerPosition ()
    {
        player.transform.position = new Vector3(0, 0, -7);
        focalPoint.transform.rotation = Quaternion.Euler(0, 0, 0);
        player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

}
