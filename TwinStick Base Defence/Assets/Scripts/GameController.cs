using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    public int playerCount;

    public GameObject[] weapons;
    public int[] costs;
    public GameObject spawnEffect;

    [SerializeField] SpawnData[] enemies;
    
    int waveIndex;
    float waveTimer;
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
    [SerializeField] Image setResourcesIcon;
    public static Image resourcesIcon;

    public static PlayerController[] players;

    MenuController menuController;

    void Start()
    {
        menuController = FindObjectOfType<MenuController>();
        resourcesIcon = setResourcesIcon;
        players = new PlayerController[playerCount];
        for(int i = 0;i<playerCount; i++)
        {
            players[i] = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        }
        SpawnWeapon(0, 0);
        menuController.SetMenu("Start");
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            menuController.SetMenu("Buy");
        }
    }

    public void StartGame()
    {
        StartWave(1);
        menuController.SetMenu(null,true,true,false);
    }

    public void StartWave(int setWave)
    {
        waveIndex = setWave;
        StartWave();
    }

    public void StartWave()
    {
        KillAll(false, false);
        difficulty = 2 + (waveIndex * 2);
        SpawnData[] newSpawnPool = new SpawnData[difficulty];
        for (int i = 0; i < difficulty; i++)
        {
            newSpawnPool[i] = enemies[Random.Range(0, enemies.Length)];
        }
        currentWave = new WaveData { spawnPool = newSpawnPool, timer = waveDuration };
        waveStarted = true;
        waveText.text = "WAVE " + waveIndex;
    }

    public void EndWave()
    {
        currentWave = null;
        waveStarted = false;
        if (!gameover)
        {
            waveIndex++;
            Invoke("StartWave", waveCooldown);
        }
    }

    public void EndGame()
    {
        gameover = true;
        CancelInvoke("StartWave");
        EndWave();
        menuController.SetMenu("Gameover",true, true, false);
    }

    public void ResetGame()
    {
        AddScore(-score);
        AddResource(-resources);
        KillAll();
        gameover = false;
        players[0].Respawn();
        SpawnWeapon(0, 0);
        waveText.text = null;
        menuController.SetMenu("Start");
    }

    public void KillAll(bool killResource = true, bool killWeapons = true, bool killEnemies = true)
    {
        if(killResource)
        {
            foreach (Resource resource in FindObjectsOfType<Resource>())
            {
                Destroy(resource.gameObject);
            }
        }
        if (killWeapons)
        {
            foreach (Weapon weapon in FindObjectsOfType<Weapon>())
            {
                Destroy(weapon.gameObject);
            }
        }
        if (killEnemies)
        {
            dontScore = true;
            foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
            {
                enemy.Damage(1000, Vector3.zero, enemy.transform.position);
            }
            dontScore = false;
        }
    }

    public void Buy(int id)
    {
        if (resources >= costs[id])
        {
            SpawnWeapon(id, 0);
            AddResource(-costs[id]);
            menuController.Return();
        }
    }

    public void SpawnWeapon(int id, int player)
    {
        SpawnWeapon(id, players[player].transform.position + new Vector3(0, 2, 2));
    }

    public void SpawnWeapon(int id, Vector3 pos)
    {
        Instantiate(weapons[id], pos,weapons[id].transform.rotation).GetComponent<Weapon>().equiped = false;
        Instantiate(spawnEffect, pos, Quaternion.identity);
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * 50;
        Instantiate(enemy, spawnPos, Quaternion.identity);
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = "SCORE: " + score.ToString("00000");
    }

    public void AddResource(int value = 1)
    {
        resources += value;
        resourceText.text = "x " + resources.ToString("000");
        resourcesIcon.GetComponent<Animator>().SetBool("Trigger",true);
    }

    public void PauseUnpause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            menuController.SetMenu("Pause");
        }
        else
        {
            Time.timeScale = 1;
            menuController.Return(0,true);
        }
    }

    public void ToggleAutoSwitch(bool toggle)
    {
        players[0].autoSwitch = toggle;
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
