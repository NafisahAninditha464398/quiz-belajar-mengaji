using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Game/Achievement")]
public class AchievementData : ScriptableObject
{
    public string achievementName;
    public string description;
    public bool isUnlocked;
    public Sprite icon; // Ikon yang ditampilkan saat achievement diraih
    public AchievementType achievementType;
    public int required;
}

public enum AchievementType
{
    CorrectAnswerCount,
    PerfectLevelCount,
    LevelsUnlockedCount
}