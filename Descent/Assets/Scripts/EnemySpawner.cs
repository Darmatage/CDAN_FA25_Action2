using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyBasic;

    public float spawnRangeStart = 0.5f;
    public float spawnRangeEnd = 1.2f;
    private float timeToSpawn;
    private float spawnTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeToSpawn = Random.Range(spawnRangeStart, spawnRangeEnd);
        spawnTimer += 0.01f;
        if (spawnTimer >= timeToSpawn)
        {
            spawnBasicEnemy();
            spawnTimer = 0f;
        }
    }

    void spawnBasicEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(0f, 10f), Random.Range(-10f, 10f));
        Instantiate(enemyBasic, spawnPosition, Quaternion.identity);
    }
}
