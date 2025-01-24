using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton untuk akses global
    public UIManager uiManager; // Referensi ke UIManager
    public QuizManager quizManager; // Referensi ke QuizManager
    public AchievementManager achievementManager;

    public PlayerInfoManager playerInfoManager;
    public PlayerProgress playerProgress; // Data progres pemain
    public SectionData currentSection; // Section yang sedang dimainkan
    public LevelData currentLevel; // Level yang sedang dimainkan

    public List<SectionData> allSections;

    // private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Menjaga GameManager saat pergantian scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // Menghubungkan event dari UIManager ke GameManager
        uiManager.OnLevelSelected += StartGame;
        quizManager.OnQuizEnd += GameOver;
        // achievementManager.OnAchievementUnlock += uiManager.ShowAchievementPopup;
        achievementManager.OnAchievementUnlock += uiManager.AddAchievementPopup;
        uiManager.OnOkButtonPressed += ProceedToNextLevel;
        uiManager.OnBackButtonPressed += BackToMenu;
    }

    private void Start()
    {
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();

        playerProgress.LoadProgress();
        playerProgress.LoadFromSave(playerProgress.unlockedSectionsList, playerProgress.unlockedSections);

        uiManager.SetPlayerInfo(playerInfoManager);
        uiManager.SetPlayerProgress(playerProgress);  // Dependency Injection

        playerProgress.InitializePlayerProgress();
        achievementManager.LoadAchievementProgress();

        Debug.Log($"Cek section lock: {playerProgress.unlockedSections.Count}");

        CheckAndShowPlayerNameInput();
        uiManager.UpdatePlayerNameUI();

        // Inisialisasi game, bisa memanggil GenerateSectionButtons dari UIManager
        uiManager.ShowSectionPanel(allSections);
        SomeMethod();
        Debug.Log($"Cek inisialisasi: {playerProgress.IsLevelUnlocked(1, 1)}");
    }


    private void OnDisable()
    {
        uiManager.OnLevelSelected -= StartGame;
        quizManager.OnQuizEnd -= GameOver;
        // achievementManager.OnAchievementUnlock -= uiManager.ShowAchievementPopup;
        achievementManager.OnAchievementUnlock += uiManager.AddAchievementPopup;
        uiManager.OnOkButtonPressed -= ProceedToNextLevel;
        uiManager.OnBackButtonPressed -= BackToMenu;
    }

    public void SomeMethod()
    {
        if (currentSection == null)
        {
            Debug.LogWarning("currentSection is null!");
            return; // Keluar dari metode jika currentSection null
        }

        if (currentLevel == null)
        {
            Debug.LogWarning("currentLevel is null!");
            return; // Keluar dari metode jika currentLevel null
        }

        // Logika yang memerlukan currentSection dan currentLevel
        Debug.Log($"Current Section ID: {currentSection.sectionId}, Current Level ID: {currentLevel.levelId}");
    }

    private void CheckAndShowPlayerNameInput()
    {
        if (!playerInfoManager.IsPlayerNameSet())
        {
            uiManager.ShowPlayerNameInputPanel();
        }
    }

    public void StartGame(SectionData section, LevelData level)
    {
        currentSection = section;
        currentLevel = level;

        Debug.Log($"Starting game at Section: {section.sectionName}, Level: {level.levelName}");

        // Mengatur UI, misalnya menyembunyikan panel utama dan menampilkan panel permainan
        uiManager.ShowGamePanel();
        uiManager.HideLevelPanel();
        uiManager.HideSectionPanel();

        //Meload quiz dari QuizManager
        quizManager.LoadQuiz(section, level);
    }

    public void GameOver(int score, int totalQuestions, int correctAnswerCount, bool perfectLevel)
    {
        Debug.Log("GameOver");

        // Tampilkan skor pemain dan panel akhir game
        uiManager.ShowScorePanel(score, totalQuestions);

        // Periksa apakah level berhasil diselesaikan untuk membuka level berikutnya
        UnlockNextLevel(score);

        //Periksa apakah perlu update score
        UpdateScore(score, perfectLevel);

        //Achievement check
        playerProgress.correctAnswerCount += correctAnswerCount;

        achievementManager.CheckAchievementProgress(playerProgress.correctAnswerCount, playerProgress.perfectLevelCount, playerProgress.levelsUnlockedCount);

        // Simpan progres pemain
        playerProgress.PrepareForSave(playerProgress.unlockedSectionsList, playerProgress.unlockedSections);
        playerProgress.SaveProgress();
        uiManager.SetPlayerProgress(playerProgress);
    }

    public bool IsLastLevelInSection(int sectionId, int levelId)
    {
        if (currentSection != null)
        {
            // Mendapatkan level terakhir di section tersebut
            LevelData lastLevelData = currentSection.levels[currentSection.levels.Count - 1];
            return lastLevelData.levelId == levelId; // Mengembalikan true jika levelId adalah level terakhir
        }

        return false; // Jika section tidak ditemukan, return false
    }

    public void UnlockNextSectionAndFirstLevel(int sectionId)
    {
        if (currentSection != null)
        {
            // Mendapatkan indeks dari section saat ini di list allSections
            int nextSectionIndex = allSections.IndexOf(currentSection) + 1;

            // Pastikan section berikutnya ada di dalam batas list
            if (nextSectionIndex < allSections.Count)
            {
                // Mendapatkan data untuk section berikutnya
                SectionData nextSectionData = allSections[nextSectionIndex];

                // Unlock section berikutnya jika belum terbuka
                if (!playerProgress.unlockedSections.Contains(nextSectionData.sectionId))
                {
                    playerProgress.SetUnlockedSection(nextSectionData.sectionId);
                    playerProgress.levelsUnlockedCount++;
                    Debug.Log($"Cek Add section: {playerProgress.IsSectionUnlocked(nextSectionData.sectionId)}");
                }

                // Unlock level pertama di section berikutnya
                if (nextSectionData.levels.Count > 0)
                {
                    int firstLevelId = nextSectionData.levels[0].levelId;
                    playerProgress.SetUnlockedLevel(nextSectionData.sectionId, firstLevelId);
                }
            }
        }
    }

    private void UnlockNextLevel(int score)
    {
        // Logika untuk membuka level berikutnya di section yang sama
        if (score != 0)
        {
            playerProgress.SetUnlockedLevel(currentSection.sectionId, currentLevel.levelId + 1);
            Debug.Log($" playerProgress: {currentLevel.levelId + 1} adalah {playerProgress.IsLevelUnlocked(currentSection.sectionId, currentLevel.levelId + 1)}");
            if (IsLastLevelInSection(currentSection.sectionId, currentLevel.levelId))
            {
                UnlockNextSectionAndFirstLevel(currentSection.sectionId);
            }
        }
    }

    public void ProceedToNextLevel()
    {
        uiManager.ShowLevelPanel(currentSection);
    }

    public void BackToMenu()
    {
        uiManager.ShowSectionPanel(allSections);
    }

    public void UpdateScore(int score, bool perfectLevel)
    {
        if (score > playerProgress.GetLevelHighScore(currentSection.sectionId, currentLevel.levelId))
        {
            playerProgress.UpdateLevelHighScore(currentSection.sectionId, currentLevel.levelId, score);
            if (perfectLevel)
            {
                playerProgress.perfectLevelCount++;
            }
            playerProgress.SaveProgress();

            Debug.Log($"Player Highscore Sekarang: {playerProgress.GetLevelHighScore(currentSection.sectionId, currentLevel.levelId)}");
        }
    }
}
