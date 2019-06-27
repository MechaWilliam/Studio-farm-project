using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    // can change values in inspector

    public class Wave
    {
        public string name;
        // name of the wave (announce wave)
        public Transform enemy;
        //Prefeb, instatiate enemy
        public int count;
        // Store amount of enemies
        public float rate; // 1 divided by the rate
        // Spawn Rate
    }

    public Wave[] waves;
    private int nextWave = 0;
    // store index of wave

    public Transform[] spawnPoints;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private SpawnState state = SpawnState.COUNTING;

    void Start ()
    {
        waveCountdown = timeBetweenWaves;
    }
     
    void Update ()
    {
        if(state == SpawnState.WAITING)
        {
            // check if enemies are still alive
            if (!EnemyIsAlive())
            {
                //begin a new round
                WaveCompleted();
                
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine( SpawnWave ( waves[nextWave] ) );
                    
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }
    } 

    void WaveCompleted()
    {
        Debug.Log("wave completed");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if(nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! Looping...");
        }
        else
        {
            nextWave++;
        }
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave (Wave _wave)  // wait x seconds inside method
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++) // run number of enemies we want to spawn
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds( 1f / _wave.rate );
        }
           

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        // spawn enemy
        Debug.Log("Spawning Enemy: " + _enemy.name);
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        // chooses random spawn point
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }
}
