using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monsters/Monster Data")]
public class MonsterData : ScriptableObject
{
    public MonsterName monsterName;
    public string unlockedDescription;
    public string lockedDescription;
    public int monsterId;
    public Sprite lockedIcon;
    public Sprite unlockedIcon;
    public string lockedName;
    public string unlockedName;
    public int healthAmount;
    public string remarksDescription;
    public string enemyTypeDescription;
    public int lowerBound;
    public int upperBound;
    public PopulationLevel populationLevel;

}
