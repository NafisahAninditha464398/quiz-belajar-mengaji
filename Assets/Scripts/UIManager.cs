using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject sectionButtonPrefab; // Prefab untuk button section
    public Transform sectionButtonContainer; // Tempat untuk menampung button
    public GameObject sectionPanel; // Panel untuk menampilkan section
    public GameObject gamePanel; // Panel untuk menampilkan gameplay

    public GameObject levelButtonPrefab; // Prefab untuk button level
    public Transform levelButtonContainer; // Tempat untuk menampung button
    public GameObject levelPanel; // Panel untuk menampilkan level

    public GameObject gameoverPanel; // Panel pop up untuk menampilkan score
    public Text ScoreTxt;

    public GameObject achievementPopUpPanel;
    public Image achievementBadge;

    public GameObject achievementPanel;
    public GameObject achievementPrefab;
    public Transform achievementContainer;

    public event Action<SectionData, LevelData> OnLevelSelected;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowSectionPanel()
    {
        sectionPanel.SetActive(true);
        GenerateSectionButtons(); // Memanggil fungsi untuk generate button section
    }
    public void ShowLevelPanel(SectionData section)
    {
        levelPanel.SetActive(true);
        GenerateLevelButtons(section);
    }

    public void HideSectionPanel()
    {
        sectionPanel.SetActive(false);
    }
    public void HideLevelPanel()
    {
        levelPanel.SetActive(false);
    }

    public void GenerateSectionButtons()
    {
        // Menghapus semua button yang ada di container sebelum menambah yang baru
        foreach (Transform child in sectionButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Mendapatkan semua section dari GameManager
        List<SectionData> allSections = GameManager.Instance.allSections;

        foreach (SectionData section in allSections)
        {
            //Buat button baru dari prefab
            GameObject button = Instantiate(sectionButtonPrefab, sectionButtonContainer);

            // Mengatur nama section di button
            button.GetComponentInChildren<Text>().text = section.sectionName;

            // Memeriksa apakah section ini sudah terbuka menggunakan PlayerProgress
            button.GetComponent<Button>().interactable = GameManager.Instance.playerProgress.IsSectionUnlocked(section.sectionId);
            Debug.Log($"apakah terbuka: {GameManager.Instance.playerProgress.IsSectionUnlocked(section.sectionId)} di section ID: {section.sectionId}");

            // Menambahkan listener untuk button
            button.GetComponent<Button>().onClick.AddListener(() => OnSectionButtonClicked(section));
        }
    }

    public void GenerateLevelButtons(SectionData section)
    {
        Debug.Log($"Section Level Ini: {section.sectionId}");
        // Bersihkan container level button sebelum mengisi ulang
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Iterasi melalui level di section yang dipilih
        foreach (LevelData level in section.levels)
        {
            // Buat button baru dari prefab
            GameObject button = Instantiate(levelButtonPrefab, levelButtonContainer);

            // Mengatur nama level di button
            button.GetComponentInChildren<Text>().text = level.levelName;

            // Memeriksa apakah level ini sudah terbuka menggunakan PlayerProgress
            button.GetComponent<Button>().interactable = GameManager.Instance.playerProgress.IsLevelUnlocked(section.sectionId, level.levelId);

            // Menambahkan listener untuk button
            button.GetComponent<Button>().onClick.AddListener(() => OnLevelButtonClicked(section, level));
        }
    }

    private void OnSectionButtonClicked(SectionData section)
    {
        ShowLevelPanel(section);
    }
    private void OnLevelButtonClicked(SectionData section, LevelData level)
    {
        //bilang game udah mulai, info level dan section
        OnLevelSelected?.Invoke(section, level);
    }
    public void ShowGamePanel()
    {
        gamePanel.SetActive(true);
    }

    public void ShowScorePanel(int score, int totalQuestions)
    {
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);
        ScoreTxt.text = $"{score} / {totalQuestions}";
    }

    public void ShowAchievementPopup(AchievementData achievement)
    {
        Debug.Log($"Achievement Unlocked: {achievement.achievementName}");
        achievementPopUpPanel.SetActive(true);
        achievementBadge.sprite = achievement.icon;
    }

    public void OnOkButtonClick()
    {
        SectionData lastSection = GameManager.Instance.currentSection;
        ShowLevelPanel(lastSection);
    }

    public void ShowAchievementPanel()
    {
        achievementPanel.SetActive(true);
        List<AchievementData> achievements = AchievementManager.Instance.GetUnlockedAchievements();
        foreach (var achievement in achievements)
        {
            if (achievement.isUnlocked)
            {
                // Tampilkan achievement yang sudah unlock, misalnya melalui UI Manager
                ShowUnlockedAchievement(achievement);
            }
        }
    }

    public void ShowUnlockedAchievement(AchievementData achievement)
    {
        // Buat elemen UI atau update bagian UI khusus achievement
        GameObject achievementUI = Instantiate(achievementPrefab, achievementContainer);
        achievementUI.GetComponentInChildren<Text>().text = achievement.achievementName;
        achievementUI.GetComponentInChildren<Image>().sprite = achievement.icon;
    }
}
