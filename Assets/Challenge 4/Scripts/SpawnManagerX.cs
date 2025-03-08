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
    public GameObject magnetBoostPrefab; // ✅ Magnet Boost Powerup

    public GameObject doubleTroublePrefab; // Assign in Inspector


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

    void SpawnEnemyWave(int wave)
    {
        Debug.Log(" Wave Count: " + wave);

        int cycle = (wave - 1) / 5;
        int baseEnemies = 1 + cycle;

        // ✅ Spawn Normal Enemies
        for (int i = 0; i < baseEnemies; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }

        // ✅ Spawn Heavy Enemies (Wave 2+)
        if (wave >= 2)
        {
            int heavyEnemies = 1 + (wave - 2) / 5;
            for (int i = 0; i < heavyEnemies; i++)
            {
                Instantiate(heavyEnemyPrefab, GenerateSpawnPosition(), heavyEnemyPrefab.transform.rotation);
            }
        }

        // ✅ Spawn Speedsters (Wave 3+)
        if (wave >= 3)
        {
            int speedsters = 1 + (wave - 3) / 5;
            for (int i = 0; i < speedsters; i++)
            {
                Instantiate(speedsterPrefab, GenerateSpawnPosition(), speedsterPrefab.transform.rotation);
            }
        }

        // ✅ Spawn Tricksters (Wave 4+)
        if (wave >= 4)
        {
            int tricksters = 1 + (wave - 4) / 5;
            for (int i = 0; i < tricksters; i++)
            {
                Instantiate(tricksterPrefab, GenerateSpawnPosition(), tricksterPrefab.transform.rotation);
            }
        }

        // ✅ Spawn Exploders (Wave 5+)
        if (wave >= 5)
        {
            int exploders = 1 + (wave - 5) / 5;
            for (int i = 0; i < exploders; i++)
            {
                Instantiate(exploderPrefab, GenerateSpawnPosition(), exploderPrefab.transform.rotation);
            }
        }

        // 55% chance to spawn Double Trouble powerup
        if (Random.Range(0, 100) < 55)
        {
            Instantiate(doubleTroublePrefab, GenerateSpawnPosition(), doubleTroublePrefab.transform.rotation);
            Debug.Log("🔥 Double Trouble Powerup Spawned!");
        }





        // ✅ Spawn Normal Powerup (If No Powerups Exist)
        if (GameObject.FindGameObjectsWithTag("Powerup").Length == 0)
        {
            Vector3 powerupSpawnOffset = new Vector3(0, 0, -15);
            Instantiate(powerupPrefab, GenerateSpawnPosition() + powerupSpawnOffset, powerupPrefab.transform.rotation);
        }

        // ✅ 50% Chance to Spawn Magnet Boost Powerup Instead of 30% 
        if (Random.Range(0, 100) < 50)  // 🔹 Increased to 50% chance per wave
        {
            Instantiate(magnetBoostPrefab, GenerateSpawnPosition(), magnetBoostPrefab.transform.rotation);
            Debug.Log(" 🧲 Magnet Boost Powerup Spawned!");
        }

        // ✅ Increase wave count
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

