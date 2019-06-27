using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    public int playerCount;
    [SerializeField] SpawnData[] enemies;
    
    int waveIndex;
    int difficulty;
    [SerializeField] float waveDuration, waveCooldown;
    [SerializeField] Button waveButton;
    [HideInInspector] public static bool waveStarted, gameover, dontScore;
    WaveData currentWave;

    [SerializeField] Text waveText;

    int score;
    [SerializeField] Text scoreText;

    int resources;
    [SerializeField] Text resourceText;

    public static PlayerController[] players;

    void Start()
    {
        players = new PlayerController[playerCount];
        for(int i = 0;i<playerCount; i++)
        {
            players[i] = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        }
    }
    
    void Update()
    {
        if (waveStarted)
        {
            if (currentWave.timer > 0)
            {
                for (int i = 0; i < currentWave.spawnPool.Length; i++)
                {
                    if (currentWave.spawnPool[i].spawnTimer <= 0)
                    {
                        SpawnEnemy(currentWave.spawnPool[i].enemy);
                        currentWave.spawnPool[i].spawnTimer = waveDuration / currentWave.spawnPool[i].count;
                    }
                    else
                    {
                        currentWave.spawnPool[i].spawnTimer -= Time.deltaTime;
                    }
                }
                currentWave.timer -= Time.deltaTime;
                //waveText.text = "WAVE " + waveIndex + " " + (currentWave.timer > 60 ? ((int)(currentWave.timer / 60)).ToString("00") + ":" + ((int)(currentWave.timer % 60)).ToString("00") : currentWave.timer.ToString("00.00"));
            }
            else
            {
                EndWave();
            }
        }
    }

    public void StartWave(int setWave)
    {
        waveIndex = setWave;
        StartWave();
    }

    public void StartWave()
    {
        dontScore = true;
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            enemy.Damage(1000, Vector3.zero, enemy.transform.position);
        }
        difficulty = 2 + (waveIndex * 2);
        SpawnData[] newSpawnPool = new SpawnData[difficulty];
        for (int i = 0; i < difficulty; i++)
        {
            newSpawnPool[i] = enemies[Random.Range(0, enemies.Length)];
        }
        currentWave = new WaveData { spawnPool = newSpawnPool, timer = waveDuration };
        waveStarted = true;
        dontScore = false;
        waveText.text = "WAVE " + waveIndex;
        //players[0].Heal(100);
    }

    public void EndWave()
    {
        currentWave = null;
        waveStarted = false;
        waveIndex++;
        Invoke("StartWave", waveCooldown);
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * 50;
        Instantiate(enemy, spawnPos, Quaternion.identity);
    }

    public void AddScore(int points)
    {
        score += points;
        scoreText.text = "SCORE: " + score.ToString("00000");
    }

    public void ResetScore()
    {
        AddScore(-score);
    }
}

[System.Serializable]
public class WaveData
{
    public SpawnData[] spawnPool;
    [HideInInspector] public float timer;
}

[System.Serializable]
public struct SpawnData
{
    public string name;
    //public int minDifficulty, maxDifficulty; - would be used to select certain enemies as waves progress but for now im just adding all of them(in start wave)
    public GameObject enemy;
    public int count;
    [HideInInspector] public float spawnTimer;
}
