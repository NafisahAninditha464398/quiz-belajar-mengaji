using System;
using System.Collections.Generic;
using UnityEngine;

//menyimpan semua variabel data yang berkaitan dengan progress yang dilakukan Player
[System.Serializable]
public class PlayerProgress : MonoBehaviour
{
    // public Dictionary<int, SectionProgress> sectionProgressData = new Dictionary<int, SectionProgress>();
    public List<SectionProgress> sectionsProgress = new List<SectionProgress>();
    public HashSet<int> unlockedSections = new HashSet<int>();
    public List<int> unlockedSectionsList = new List<int>();

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

    // Fungsi Serialize untuk menyimpan progres ke PlayerPrefs
    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PlayerProgress", json);
        PlayerPrefs.Save();
    }

    // Fungsi Deserialize untuk memuat progres dari PlayerPrefs
    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey("PlayerProgress"))
        {
            string json = PlayerPrefs.GetString("PlayerProgress");
            JsonUtility.FromJsonOverwrite(json, this);  // Memuat data JSON ke dalam objek
        }
        else
        {
            InitializePlayerProgress(); // Atau buat inisialisasi default jika tidak ada data
        }
    }

    // Sebelum menyimpan ke JSON, pindahkan data HashSet ke List
    public void PrepareForSave(List<int> unlockedSectionsList, HashSet<int> unlockedSections)
    {
        unlockedSectionsList.Clear();
        unlockedSectionsList.AddRange(unlockedSections);
    }

    // Setelah data di-load dari JSON, pindahkan kembali data ke HashSet
    public void LoadFromSave(List<int> unlockedSectionsList, HashSet<int> unlockedSections)
    {
        unlockedSections.Clear();
        unlockedSections.UnionWith(unlockedSectionsList);
    }


    // Fungsi untuk mendapatkan progres level berdasarkan ID
    public SectionProgress.LevelProgress GetLevelProgress(int sectionId, int levelId)
    {
        EnsureSectionExists(sectionId);
        return EnsureLevelExists(sectionId, levelId);
    }

    public bool IsSectionUnlocked(int sectionId)
    {
        Debug.Log($"IsSectionUnlocked: {unlockedSections.Count}");
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
        // Cek apakah section dengan sectionId sudah ada di daftar sectionsProgress
        SectionProgress sectionProgress = sectionsProgress.Find(s => s.sectionId == sectionId);

        if (sectionProgress == null)
        {
            // Jika tidak ada, buat section baru dan tambahkan ke daftar
            SectionProgress newSectionProgress = new SectionProgress
            {
                sectionId = sectionId,
                levelsProgress = new List<SectionProgress.LevelProgress>()
            };
            sectionsProgress.Add(newSectionProgress);
        }
    }

    // Fungsi untuk memastikan bahwa LevelProgress ada dalam SectionProgress
    private SectionProgress.LevelProgress EnsureLevelExists(int sectionId, int levelId)
    {
        // Pastikan Section dengan sectionId ada
        EnsureSectionExists(sectionId);

        // Cari SectionProgress berdasarkan sectionId
        SectionProgress sectionProgress = sectionsProgress.Find(s => s.sectionId == sectionId);

        // Cari LevelProgress berdasarkan levelId di dalam SectionProgress
        SectionProgress.LevelProgress levelProgress = sectionProgress.levelsProgress.Find(level => level.levelId == levelId);

        // Jika LevelProgress tidak ditemukan, buat LevelProgress baru dan tambahkan ke levelsProgress
        if (levelProgress == null)
        {
            levelProgress = new SectionProgress.LevelProgress
            {
                levelId = levelId,
                highScore = 0,    // Set nilai default highScore jika diperlukan
                isUnlocked = false // Set nilai default isUnlocked jika diperlukan
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
        EnsureSectionExists(sectionId);
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

        // Pastikan Section 1 ada di sectionsProgress
        SectionProgress sectionProgress = sectionsProgress.Find(s => s.sectionId == 1);

        if (sectionProgress == null)
        {
            sectionProgress = new SectionProgress
            {
                sectionId = 1,
                levelsProgress = new List<SectionProgress.LevelProgress>()
            };
            sectionsProgress.Add(sectionProgress);
        }

        // Pastikan Level 1 di Section 1 terbuka
        SetUnlockedLevel(1, 1);
    }
}
