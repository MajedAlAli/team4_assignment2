using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public GameObject focalPoint;
    public GameObject smashPowerupPrefab; // New smash power-up
    public float smashPowerupChance = 0.3f; // 30% chance to spawn per wave


    private float spawnRangeX = 10;
    private float spawnZMin = 15; // set min spawn Z
    private float spawnZMax = 25; // set max spawn Z

    public int enemyCount;
    public int waveCount = 1;


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
        Vector3 powerupSpawnOffset = new Vector3(0, 0, -15); // Spawn near the player

        // Destroy any leftover power-ups from the previous wave
        GameObject[] oldPowerups = GameObject.FindGameObjectsWithTag("Powerup");
        GameObject[] oldSmashPowerups = GameObject.FindGameObjectsWithTag("PowerupSmash");

        foreach (GameObject powerup in oldPowerups)
        {
            Destroy(powerup);
        }
        foreach (GameObject powerup in oldSmashPowerups)
        {
            Destroy(powerup);
        }

        // Ensure only ONE power-up spawns per wave, with a 50% chance for each type
        GameObject powerupToSpawn = Random.value < 0.5f ? powerupPrefab : smashPowerupPrefab;
        Instantiate(powerupToSpawn, GenerateSpawnPosition() + powerupSpawnOffset, powerupToSpawn.transform.rotation);

        // Spawn enemies
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }

        waveCount++;
        ResetPlayerPosition(); // Reset player
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
