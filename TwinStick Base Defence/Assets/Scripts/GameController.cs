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
    

    [SerializeField] float waveBaseTime, clearingTime, waveCooldown;
    [HideInInspector] public static bool waveStarted, clearing, gameover, dontScore;
    WaveData currentWave;
    int waveIndex;
    float waveTime;
    float waveTimer;
    int difficulty;

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
        SpawnWeapon(0, 0, false);
        menuController.SetMenu("Start");
    }
    
    void Update()
    {
        if (!gameover)
        {
            if (waveStarted)
            {
                if (!clearing)
                {
                    if (waveTimer > 0)
                    {
                        float waveProgress = 1 - waveTimer / waveTime;
                        for (int i = 0; i < currentWave.spawnPool.Length; i++)
                        {
                            if (currentWave.spawnPool[i].enemiesSpawned >= currentWave.spawnPool[i].count || waveProgress < currentWave.spawnPool[i].startDistribution || waveProgress >= currentWave.spawnPool[i].endDistribution) continue;

                            if (currentWave.spawnPool[i].spawnTimer <= 0)
                            {
                                SpawnEnemy(currentWave.spawnPool[i].enemy);

                                float newOffset = Random.Range(0, 1f);
                                currentWave.spawnPool[i].spawnTimer = (currentWave.spawnPool[i].maxSpawnTimer * (1 - currentWave.spawnPool[i].timerOffset)) + (currentWave.spawnPool[i].maxSpawnTimer * newOffset);
                                currentWave.spawnPool[i].timerOffset = newOffset;
                                currentWave.spawnPool[i].enemiesSpawned++;
                            }
                            else
                            {
                                currentWave.spawnPool[i].spawnTimer -= Time.deltaTime;
                            }
                        }
                        waveTimer -= Time.deltaTime;
                    }
                    else
                    {
                        clearing = true;
                        waveTimer = clearingTime;
                        waveText.text = "CLEARING";
                    }
                }
                else if (waveTimer <= 0 || FindObjectsOfType<EnemyController>().Length == 0)
                {
                    EndWave();
                }
                else
                {
                    waveTimer -= Time.deltaTime;
                    waveText.text = "CLEARING: " + " " + waveTimer.ToString("0.0");
                }
            }
            else if (waveIndex > 0)
            {
                if (waveTimer > 0)
                {
                    waveText.text = "NEXT WAVE: " + " " + waveTimer.ToString("0.0");
                    waveTimer -= Time.deltaTime;
                }
                else
                {
                    Debug.Log("yes");
                    StartWave();
                }
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
        gameover = false;
        StartWave(1);
    }

    public void StartWave(int setWave)
    {
        waveIndex = setWave;
        StartWave();
    }

    public void StartWave()
    {
        Debug.Log("waveStarted");
        difficulty = 2 + (waveIndex * 2);
        waveTime = waveBaseTime + difficulty;
        List<SpawnData> spawnRange = new List<SpawnData>();
        SpawnData[] newSpawnPool = new SpawnData[difficulty];
        foreach (SpawnData enemy in enemies)
        {
            if (enemy.minDifficulty <= difficulty && (enemy.maxDifficulty == 0 || enemy.maxDifficulty >= difficulty))
            {
                spawnRange.Add(enemy);
            }
        }
        for (int i = 0; i < difficulty; i++)
        {
            newSpawnPool[i] = spawnRange[Random.Range(0, spawnRange.Count)];
        }
        for (int i = 0; i < newSpawnPool.Length; i++) //initialize spawn sets
        {
            newSpawnPool[i].maxSpawnTimer = (waveTime / newSpawnPool[i].count) * (1 / (newSpawnPool[i].endDistribution - newSpawnPool[i].startDistribution));
            newSpawnPool[i].timerOffset = 1;
        }
        currentWave = new WaveData { spawnPool = newSpawnPool};
        waveStarted = true;
        waveTimer = waveTime;
        waveText.text = "WAVE " + waveIndex;
        menuController.SetMenu(null, true, true, false);
    }

    public void EndWave()
    {
        currentWave = null;
        waveStarted = false;
        clearing = false;
        if (!gameover)
        {
            KillAll(false, false);
            waveIndex++;
            waveTimer = waveCooldown;
            menuController.SetMenu("NextWave", true, true, false);
        }
        else
        {
            waveTimer = 0;
            waveText.text = "WAVE " + waveIndex;
        }
    }

    public void EndGame()
    {
        gameover = true;
        EndWave();
        menuController.SetMenu("Gameover", true, true, false);
    }

    public void ResetGame()
    {
        gameover = true;
        EndWave();
        AddScore(-score);
        AddResource(-resources);
        KillAll();
        players[0].Respawn();
        SpawnWeapon(0, 0);
        waveText.text = null;
        menuController.SetMenu("Start",true, true, false);
        Time.timeScale = 1;
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

    public void SpawnWeapon(int id, int player, bool pickup = true)
    {
        int slot;
        if (pickup && players[player].GetEmptySlot(out slot))
        {
            Weapon weapon;
            SpawnWeapon(id, players[player].transform.position, out weapon);
            weapon.Pickup(players[player], slot, true);
        }
        else
        {
            SpawnWeapon(id, players[player].transform.position + new Vector3(0, 2, 2));
        }
    }

    public void SpawnWeapon(int id, Vector3 pos)
    {
        SpawnWeapon(id, pos, out Weapon weapon);
        weapon.equiped = false;
    }

    public void SpawnWeapon(int id, Vector3 pos, out Weapon weapon)
    {
        Instantiate(spawnEffect, pos, Quaternion.identity);
        weapon = Instantiate(weapons[id], pos, weapons[id].transform.rotation).GetComponent<Weapon>();
    }

    void SpawnEnemy(GameObject enemy)
    {
        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * 50;
        Debug.Log("Spawned " + Instantiate(enemy, spawnPos, Quaternion.identity).name + " at: " + (waveTimer > 60 ? ((int)(waveTimer / 60)).ToString("00") + ":" + ((int)(waveTimer % 60)).ToString("00") : waveTimer.ToString("00.00")));
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
}

[System.Serializable]
public struct SpawnData
{
    public string name;
    public GameObject enemy;
    public int count;
    public int minDifficulty, maxDifficulty;
    [Range(0, 1)] public float startDistribution, endDistribution;
    [HideInInspector] public float spawnRatio, maxSpawnTimer, spawnTimer, timerOffset;
    [HideInInspector] public int enemiesSpawned;
}
