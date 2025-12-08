using UnityEngine;

[CreateAssetMenu(menuName = "Quest/New Quest")]
public class QuestData : ScriptableObject
{
    public string questName;
    public QuestStep[] steps;
}

