using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Newspaper Database", menuName = "Database/Newspaper Database")]
public class NewspaperDatabase : ScriptableObject
{
    // Ordered issues start from ID 1
    public List<NewsIssue> orderedIssues;
    // Random issues have ID 0
    public List<NewsIssue> randomIssues;
}
