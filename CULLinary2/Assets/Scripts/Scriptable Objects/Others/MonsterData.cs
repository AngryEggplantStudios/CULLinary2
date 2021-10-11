using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monsters/Monster Data")]
public class MonsterData : ScriptableObject
{
    public MonsterName monsterName;
    public string description;
    public int lowerBound;
    public int upperBound;
    public PopulationLevel populationLevel;

}
