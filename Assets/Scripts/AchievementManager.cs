using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    public event Action<AchievementData> OnAchievementUnlock;

    [SerializeField] private List<AchievementData> achievements;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public void CheckAchievementProgress(int correctAnswerCount)
    // {
    //     foreach (var achievement in achievements)
    //     {
    //         if (!achievement.isUnlocked && correctAnswerCount >= achievement.requiredCorrectAnswers)
    //         {
    //             UnlockAchievement(achievement);
    //         }
    //     }
    // }

    public void CheckAchievementProgress(int correctAnswerCount, int perfectLevelCount, int levelsUnlockedCount)
    {
        foreach (var achievement in achievements)
        {
            if (!achievement.isUnlocked)
            {
                if (achievement.achievementType.ToString() == "CorrectAnswerCount" && correctAnswerCount >= achievement.required)
                {
                    UnlockAchievement(achievement);
                    Debug.Log($"achievement.requiredCorrectAnswers: {achievement.required},  achievement.achievementName: {achievement.achievementName})");
                }
                if (achievement.achievementType.ToString() == "PerfectLevelCount" && perfectLevelCount >= achievement.required)
                {
                    UnlockAchievement(achievement);
                    Debug.Log($"achievement.requiredPerfectLevels: {achievement.required},  achievement.achievementName: {achievement.achievementName})");
                }
                if (achievement.achievementType.ToString() == "LevelsUnlockedCount" && levelsUnlockedCount + 1 >= achievement.required)
                {
                    UnlockAchievement(achievement);
                    Debug.Log($"achievement.requiredLevelsUnlockes: {achievement.required},  achievement.achievementName: {achievement.achievementName})");
                }
            }
        }
    }

    private void UnlockAchievement(AchievementData achievement)
    {
        achievement.isUnlocked = true;
        OnAchievementUnlock?.Invoke(achievement);
        SaveAchievementProgress(achievement);
    }

    private void SaveAchievementProgress(AchievementData achievement)
    {
        PlayerPrefs.SetInt(achievement.achievementName, 1);
        PlayerPrefs.Save();
        Debug.Log("Save achievement");
    }

    public void LoadAchievementProgress()
    {
        Debug.Log("Load achievement");
        foreach (var achievement in achievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt(achievement.achievementName, 0) == 1;
        }
    }

    public List<AchievementData> GetUnlockedAchievements()
    {
        List<AchievementData> unlockedAchievements = new List<AchievementData>();
        foreach (var achievement in achievements)
        {
            if (achievement.isUnlocked)
            {
                unlockedAchievements.Add(achievement);
            }
        }
        return unlockedAchievements;
    }
    public List<AchievementData> GetAllAchievements()
    {
        return achievements;
    }

}
