using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Database", menuName = "Database/Monster Database")]
public class MonsterDatabase : ScriptableObject
{
    public List<MonsterData> allMonsters;
}
