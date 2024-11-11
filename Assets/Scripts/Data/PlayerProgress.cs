using System.Collections.Generic;
using UnityEngine;

//menyimpan semua variabel data yang berkaitan dengan progress yang dilakukan Player

[CreateAssetMenu(fileName = "PlayerProgress", menuName = "Quiz/Player Progress")]
public class PlayerProgress : ScriptableObject
{
    public Dictionary<int, SectionProgress> sectionProgressData = new Dictionary<int, SectionProgress>();

    public HashSet<int> unlockedSections = new HashSet<int>();

    [System.Serializable]
    public class SectionProgress
    {
        public int sectionId; // ID unik untuk section
        public List<LevelProgress> levelsProgress; // Daftar progres tiap level dalam section

        [System.Serializable]
        public class LevelProgress
        {
            public int levelId; // ID unik untuk level
            public int highScore; // Skor tertinggi yang dicapai di level
            public bool isUnlocked; // Status terbuka atau tidak
        }
    }

    // Fungsi untuk mendapatkan progres level berdasarkan ID
    public SectionProgress.LevelProgress GetLevelProgress(int sectionId, int levelId)
    {
        if (sectionProgressData.ContainsKey(sectionId))
        {
            return sectionProgressData[sectionId].levelsProgress.Find(level => level.levelId == levelId);
        }
        return null;
    }

    public bool IsSectionUnlocked(int sectionId)
    {
        return unlockedSections.Contains(sectionId);
    }

    public bool IsLevelUnlocked(int sectionId, int levelId)
    {
        var levelProgress = GetLevelProgress(sectionId, levelId);
        return levelProgress != null && levelProgress.isUnlocked;
    }

    public int GetLevelHighScore(int sectionId, int levelId)
    {
        var levelProgress = GetLevelProgress(sectionId, levelId);
        return levelProgress != null ? levelProgress.highScore : 0;
    }

    // Fungsi untuk memastikan bahwa SectionProgress ada
    private void EnsureSectionExists(int sectionId)
    {
        if (!sectionProgressData.ContainsKey(sectionId))
        {
            SectionProgress newSectionProgress = new SectionProgress
            {
                sectionId = sectionId,
                levelsProgress = new List<SectionProgress.LevelProgress>()
            };
            sectionProgressData.Add(sectionId, newSectionProgress);
        }
    }

    // Fungsi untuk memastikan bahwa LevelProgress ada dalam SectionProgress
    private SectionProgress.LevelProgress EnsureLevelExists(int sectionId, int levelId)
    {
        EnsureSectionExists(sectionId);
        SectionProgress sectionProgress = sectionProgressData[sectionId];
        SectionProgress.LevelProgress levelProgress = sectionProgress.levelsProgress.Find(level => level.levelId == levelId);
        if (levelProgress == null)
        {
            levelProgress = new SectionProgress.LevelProgress
            {
                levelId = levelId,
                highScore = 0, // Set default highScore jika diperlukan
                isUnlocked = false // Set default isUnlocked jika diperlukan
            };
            sectionProgress.levelsProgress.Add(levelProgress);
        }
        return levelProgress;
    }

    public void SetUnlockedLevel(int sectionId, int levelId)
    {
        SectionProgress.LevelProgress levelProgress = EnsureLevelExists(sectionId, levelId);
        levelProgress.isUnlocked = true;
    }

    public void SetUnlockedSection(int sectionId)
    {
        unlockedSections.Add(sectionId);
    }

    public void UpdateLevelHighScore(int sectionId, int levelId, int score)
    {
        SectionProgress.LevelProgress levelProgress = EnsureLevelExists(sectionId, levelId);
        levelProgress.highScore = score;
    }

    public void InitializePlayerProgress()
    {
        // Pastikan Section 1 terbuka
        unlockedSections.Add(1);

        // Pastikan Section 1 ada di sectionProgressData
        if (!sectionProgressData.ContainsKey(1))
        {
            SectionProgress newSectionProgress = new SectionProgress
            {
                sectionId = 1,
                levelsProgress = new List<SectionProgress.LevelProgress>()
            };
            sectionProgressData.Add(1, newSectionProgress);
        }

        // Pastikan Level 1 di Section 1 terbuka
        SetUnlockedLevel(1, 1);
    }
}
