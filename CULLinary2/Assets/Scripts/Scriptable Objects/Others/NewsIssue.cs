using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Newspaper Issue", menuName = "Newspaper/Issues")]
public class NewsIssue : ScriptableObject
{
    public int issueId;
    public string headlines;
    public string subhead;

    public int[] recipesUnlocked;
    public MonsterName[] enemiesUnlocked;
}
