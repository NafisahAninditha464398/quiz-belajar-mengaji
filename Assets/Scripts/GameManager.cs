using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton untuk akses global
    public UIManager uiManager; // Referensi ke UIManager
    public QuizManager quizManager; // Referensi ke QuizManager
    public AchievementManager achievementManager;

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
        FindObjectOfType<UIManager>().OnLevelSelected += StartGame;
        FindObjectOfType<QuizManager>().OnQuizEnd += GameOver;
        FindObjectOfType<AchievementManager>().OnAchievementUnlock += uiManager.ShowAchievementPopup;
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        playerProgress.InitializePlayerProgress();
        achievementManager.LoadAchievementProgress();
        // Inisialisasi game, bisa memanggil GenerateSectionButtons dari UIManager
        uiManager.ShowSectionPanel();
        SomeMethod();
        Debug.Log($"Cek inisialisasi: {playerProgress.IsLevelUnlocked(1, 1)}");
        Debug.Log($"Cek inisialisasi: {playerProgress.IsLevelUnlocked(1, 2)}");
        Debug.Log($"Player Highscore: {playerProgress.GetLevelHighScore(1, 1)}");
    }

    private void OnDisable()
    {
        FindObjectOfType<UIManager>().OnLevelSelected -= StartGame;
        FindObjectOfType<QuizManager>().OnQuizEnd -= GameOver;
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

    public void StartGame(SectionData section, LevelData level)
    {
        currentSection = section;
        currentLevel = level;
        // isGameOver = false;

        Debug.Log($"Starting game at Section: {section.sectionName}, Level: {level.levelName}");

        // Mengatur UI, misalnya menyembunyikan panel utama dan menampilkan panel permainan
        uiManager.ShowGamePanel();
        uiManager.HideLevelPanel();
        uiManager.HideSectionPanel();

        //Meload quiz dari QuizManager
        quizManager.LoadQuiz(section, level);

        // Reset data level jika perlu
        // Mulai loop game atau tampilkan pertanyaan pertama
    }

    public void GameOver(int score, int totalQuestions)
    {
        Debug.Log("GameOver");
        // isGameOver = true;

        // Tampilkan skor pemain dan panel akhir game
        uiManager.ShowScorePanel(score, totalQuestions);

        // Periksa apakah level berhasil diselesaikan untuk membuka level berikutnya
        UnlockNextLevel(score);

        //Periksa apakah perlu update score
        UpdateScore(score);

        // Simpan progres pemain
        SavePlayerProgress();
    }

    private void UnlockNextLevel(int score)
    {
        // Logika untuk membuka level berikutnya di section yang sama
        if (score != 0)
        {
            playerProgress.SetUnlockedLevel(currentSection.sectionId, currentLevel.levelId + 1);
            Debug.Log($" playerProgress: {currentLevel.levelId + 1} adalah {playerProgress.IsLevelUnlocked(currentSection.sectionId, currentLevel.levelId + 1)}");
        }
    }

    private void SavePlayerProgress()
    {
        // Logika untuk menyimpan data progres pemain (misalnya ke file atau PlayerPrefs)
        Debug.Log("Player progress saved.");
        // Implementasi spesifik bisa berupa serialisasi ke file atau menggunakan PlayerPrefs
    }

    public void UpdateScore(int score)
    {
        if (score > playerProgress.GetLevelHighScore(currentSection.sectionId, currentLevel.levelId))
        {
            playerProgress.UpdateLevelHighScore(currentSection.sectionId, currentLevel.levelId, score);
            Debug.Log($"Player Highscore Sekarang: {playerProgress.GetLevelHighScore(currentSection.sectionId, currentLevel.levelId)}");
        }
    }
}