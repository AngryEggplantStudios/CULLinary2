using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    private enum SpawnState
    {
        Inactive,
        Active,
        Loop,
    }

    [SerializeField] private GameObject enemyToSpawn;
    [SerializeField] private GameObject miniBoss;
    [Tooltip("Random displacement of enemy spawn in X/Z axes")]
    [SerializeField] private float distRange;
    [Tooltip("Is spawner able to be retriggered?")]
    [SerializeField] private bool toLoop;
    [Tooltip("Delay after triggering if toLoop is checked")]
    [SerializeField] private float delayLoopTime;
    [Tooltip("Initial Delay")]
    [SerializeField] private int initialDelay = 10;

    private GameObject monstersParent;
    private int minEnemy;
    private int maxEnemy;
    private int localSpawnCap; //It will not spawn more than this amount in total
    private bool delayFlag = false;
    private SpawnState state;
    private bool canSpawn = true;
    private int spawnAmount = 0;

    private void Start()
    {
        GameTimer.OnStartNewDay += SpawnEnemies;

        this.state = SpawnState.Inactive;
        if (toLoop)
        {
            this.state = SpawnState.Loop;
        }
        if (initialDelay == 0)
        {
            delayFlag = true;
        }
        else
        {
            StartCoroutine(DelayTimer(initialDelay));
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player") && delayFlag)
        {
            if (this.state == SpawnState.Inactive)
            {
                this.state = SpawnState.Active;
                SpawnEnemies();
            }
            else if (this.state == SpawnState.Loop && canSpawn)
            {
                canSpawn = false;
                StartCoroutine(SpawnTimer(delayLoopTime));
                SpawnEnemies();
            }
        }

    }

    private void SpawnEnemies()
    {
        int enemyNum = Random.Range(minEnemy, maxEnemy + 1);
        // Debug.Log("spawning " + enemyNum + " for " + enemyToSpawn.name);
        for (int i = 0; i < enemyNum; i++)
        {
            InstantiateEnemy();
        }
    }

    private void InstantiateEnemy()
    {
        if (spawnAmount < localSpawnCap)
        {
            float distX = Random.Range(-distRange, distRange);
            float distZ = Random.Range(-distRange, distRange);
            Vector3 enemyTransform = new Vector3(transform.position.x + distX, transform.position.y, transform.position.z + distZ);
            Instantiate(enemyToSpawn, enemyTransform, Quaternion.identity);
            enemyToSpawn.GetComponent<MonsterScript>().spawner = gameObject;
            spawnAmount++;
        }
    }

    public void SpawnMiniBoss()
    {
        if (miniBoss)
        {
            Instantiate(miniBoss, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator DelayTimer(int delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        delayFlag = true;
    }

    private IEnumerator SpawnTimer(float delayLoopTime)
    {
        yield return new WaitForSeconds(delayLoopTime);
        canSpawn = true;
    }

    public MonsterName GetMonsterName()
    {
        return enemyToSpawn.GetComponent<MonsterScript>().GetMonsterName();
    }

    public int GetSpawnCap()
    {
        return localSpawnCap;
    }

    public void SetSpawnCap(int value)
    {
        localSpawnCap = value;
        // Debug.Log("spawn cap set to " + value);
    }

    public void SetMinSpawn(int value)
    {
        minEnemy = value;
        // Debug.Log("min enemy spawn set to " + value);
    }

    public void SetMaxSpawn(int value)
    {
        maxEnemy = value;
        // Debug.Log(string.Format("min max spawning for {0} set to {1}-{2}", GetMonsterName(), minEnemy, maxEnemy));
    }

    public void DecrementSpawnCap(int value)
    {
        // Debug.Log("decrement spawn cap");
        if (localSpawnCap > 0)
        {
            localSpawnCap -= value;
        }
    }

    public void OnDestroy()
    {
        GameTimer.OnStartNewDay -= SpawnEnemies;
    }
}
