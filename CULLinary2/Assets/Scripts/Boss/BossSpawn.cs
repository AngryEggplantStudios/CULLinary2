using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{

    [SerializeField] private GameObject enemyToSpawn;
    [Tooltip("Minimum amount of enemies to spawn per trigger")]
    [SerializeField] private int minEnemy;
    [Tooltip("Maximum amount of enemies to spawn per trigger")]
    [SerializeField] private int maxEnemy;
    [Tooltip("Random displacement of enemy spawn in X/Z axes")]
    [SerializeField] private float distRange;
    [SerializeField] GameObject parentWhichContainsAllMobs;

	public void Start()
	{
        //Create gameobject to contain all spawns, if not available, if available retrieve from scene. Needed in case clown is respawned
        GameObject gameObject = GameObject.Find(this.gameObject.name + "Container");
        if (gameObject == null)
		{
            gameObject = new GameObject(this.gameObject.name + "Container");
        }
        gameObject.transform.parent = parentWhichContainsAllMobs.transform;
        gameObject.AddComponent<BossSpawnSuicideScript>();
        parentWhichContainsAllMobs = gameObject;
    }

	public void activateSpawn()
    {
        int enemyNum = Random.Range(minEnemy, maxEnemy + 1);
        instantiateEnemy();
    }

    private void instantiateEnemy()
    {
        float distX = Random.Range(-distRange, distRange);
        float distZ = Random.Range(-distRange, distRange);
        Vector3 enemyTransform = new Vector3(transform.position.x + distX, transform.position.y, transform.position.z + distZ);
        GameObject mobSpawned = Instantiate(enemyToSpawn, enemyTransform, Quaternion.identity);
        mobSpawned.transform.SetParent(parentWhichContainsAllMobs.transform);
    }
    
    public void destroyAllSpawns()
    {
        parentWhichContainsAllMobs.GetComponent<BossSpawnSuicideScript>().destroyAllMobs();
    }

}
