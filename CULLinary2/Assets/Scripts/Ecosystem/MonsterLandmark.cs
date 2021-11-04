using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLandmark : MonoBehaviour
{
    public MonsterName monsterName;

    static Dictionary<MonsterName, GameObject> landmarks =
        new Dictionary<MonsterName, GameObject>();

    void Start()
    {
        landmarks.Add(monsterName, this.gameObject);
    }

    public static GameObject GetByMonsterName(MonsterName name)
    {
        GameObject obj = null;
        if (landmarks.TryGetValue(name, out obj))
        {
            return obj;
        }
        else
        {
            Debug.Log("No landmark for " + name);
            return null;
        }
    }
}
