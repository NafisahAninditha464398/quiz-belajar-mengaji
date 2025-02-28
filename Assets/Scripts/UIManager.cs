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
    public Text sectionTextName;

    public GameObject gameoverPanel; // Panel pop up untuk menampilkan score
    public Text ScoreTxt;
    public Text TextJumlahBenar;
    public Text TextJumlahSalah;

    public GameObject achievementPopUpPanel;
    public Image achievementBadge;
    public Transform achievementContainer;
    public Text achievementNameText;
    public Text descriptionText;
    private Queue<AchievementData> achievementQueue = new Queue<AchievementData>();
    // private bool isDisplayingAchievementPopup = false;

    public GameObject achievementPanel;
    public GameObject achievementPrefab;
    public Transform achievementViewport;

    public GameObject playerNameInputPanel;
    public InputField playerNameInputField;

    public event Action<SectionData, LevelData> OnLevelSelected;
    public event Action OnOkButtonPressed;
    public event Action OnBackButtonPressed;

    public Text playerNameText;

    private PlayerProgress playerProgress;
    private PlayerInfoManager playerInfoManager;

    AudioManager audioManager;


    // Setter untuk Dependency Injection

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void SetPlayerProgress(PlayerProgress progress)
    {
        playerProgress = progress;
        Debug.Log("Called");
    }

    public void SetPlayerInfo(PlayerInfoManager playerInfo)
    {
        playerInfoManager = playerInfo;
    }

    public void UpdatePlayerNameUI()
    {
        playerNameText.text = playerInfoManager.GetPlayerName();
    }

    public void ShowPlayerNameInputPanel()
    {
        playerNameInputPanel.SetActive(true);
    }

    // Dipanggil saat tombol Simpan ditekan oleh pemain
    public void OnSavePlayerName()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            playerInfoManager.SavePlayerName(playerName);
            UpdatePlayerNameUI();
            playerNameInputPanel.SetActive(false);
        }
    }

    public void ShowSectionPanel(List<SectionData> allSections)
    {
        ShowSectionPanelUI();
        GenerateSectionButtons(allSections); // Memanggil fungsi untuk generate button section
    }
    public void ShowLevelPanel(SectionData section)
    {
        Debug.Log($"lastSection: {section.sectionName}");
        ShowLevelPanelUI();
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

    public void ShowSectionPanelUI()
    {
        sectionPanel.SetActive(true); // Menampilkan panel UI
    }
    public void ShowLevelPanelUI()
    {
        levelPanel.SetActive(true); // Menampilkan panel UI
    }

    public void GenerateSectionButtons(List<SectionData> allSections)
    {
        // Menghapus semua button yang ada di container sebelum menambah yang baru
        foreach (Transform child in sectionButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (SectionData section in allSections)
        {
            //Buat button baru dari prefab
            GameObject button = Instantiate(sectionButtonPrefab, sectionButtonContainer);

            //Mengatur sfx button
            Button buttonClick = button.GetComponent<Button>();
            buttonClick.onClick.AddListener(() => audioManager.PlaySFX(audioManager.click));

            // Mengatur nama section di button
            button.GetComponentInChildren<Text>().text = section.sectionName;

            //Mengatur image section di button
            Transform borderImgTransform = button.transform.Find("border-img");
            Transform transformImage = borderImgTransform.transform.Find("section-img");
            Image targetImage = transformImage.GetComponentInChildren<Image>();
            targetImage.sprite = section.sectionImage;

            // Memeriksa apakah section ini sudah terbuka menggunakan PlayerProgress
            bool sectionUnlock = playerProgress.IsSectionUnlocked(section.sectionId);
            button.GetComponent<Button>().interactable = sectionUnlock;
            Transform imgLock = button.transform.Find("lock-img");
            imgLock.gameObject.SetActive(!sectionUnlock);

            Debug.Log($"apakah terbuka: {playerProgress.IsSectionUnlocked(section.sectionId)} di section ID: {section.sectionId}");

            // Menambahkan listener untuk button
            button.GetComponent<Button>().onClick.AddListener(() => OnSectionButtonClicked(section));
        }
    }

    public void GenerateLevelButtons(SectionData section)
    {
        Debug.Log($"Section Level Ini: {section.sectionId}");
        // Bersihkan container level button sebelum mengisi ulang

        sectionTextName.text = section.sectionName;

        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Iterasi melalui level di section yang dipilih
        foreach (LevelData level in section.levels)
        {
            // Buat button baru dari prefab
            GameObject button = Instantiate(levelButtonPrefab, levelButtonContainer);

            //Mengatur sfx button
            Button buttonClick = button.GetComponent<Button>();
            buttonClick.onClick.AddListener(() => audioManager.PlaySFX(audioManager.click));

            // Mengatur nama level di button
            button.GetComponentInChildren<Text>().text = level.levelName;

            // Memeriksa apakah level ini sudah terbuka menggunakan PlayerProgress
            bool levelUnlock = playerProgress.IsLevelUnlocked(section.sectionId, level.levelId);
            button.GetComponent<Button>().interactable = levelUnlock;
            Transform imgLock = button.transform.Find("lock-img");
            imgLock.gameObject.SetActive(!levelUnlock);

            // Menmapilkan highscore
            Text highscoreText = button.transform.GetChild(3).GetComponent<Text>(); //rawan
            if (playerProgress != null)
            {
                highscoreText.text = playerProgress.GetLevelHighScore(section.sectionId, level.levelId).ToString();
            }

            // Menmapilkan progress
            Slider progress = button.transform.GetChild(2).GetComponent<Slider>(); //rawan
            if (playerProgress != null)
            {
                progress.value = (float)playerProgress.GetLevelHighScore(section.sectionId, level.levelId) / level.QnAs.Count;
            }



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
        //Memberi tahu Game Manager, game telah dimulai, info level dan section
        OnLevelSelected?.Invoke(section, level);
    }
    public void ShowGamePanel()
    {
        gamePanel.SetActive(true);
    }

    public void ShowScorePanel(int score, int totalQuestions)
    {
        // isDisplayingScorePopup = true;
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);
        ScoreTxt.text = $"{score} / {totalQuestions}";
        TextJumlahBenar.text = $"{score}";
        TextJumlahSalah.text = $"{totalQuestions - score}";

        audioManager.PlaySFX(audioManager.endQuiz);
    }

    public void AddAchievementPopup(AchievementData achievement) //ShowAchievementPopup
    {
        Debug.Log($"Achievement Unlocked: {achievement.achievementName}");
        achievementQueue.Enqueue(achievement);
    }

    private void DisplayNextAchievement()
    {
        if (achievementQueue.Count > 0)
        {
            audioManager.PlaySFX(audioManager.achievement);
            // isDisplayingAchievementPopup = true;
            AchievementData achievement = achievementQueue.Dequeue();
            achievementNameText.text = achievement.achievementName;
            descriptionText.text = achievement.description;
            achievementBadge.sprite = achievement.icon;
            achievementPopUpPanel.SetActive(true);
        }
        else
        {
            // isDisplayingAchievementPopup = false;
            achievementPopUpPanel.SetActive(false);
        }
    }

    public void OnOkAchievementButtonPressed()
    {
        if (achievementQueue.Count > 0)
        {
            DisplayNextAchievement();
        }
        else
        {
            achievementPopUpPanel.SetActive(false);
            // isDisplayingAchievementPopup = false;
        }
    }

    //dipanggil di button OnClick()
    public void HandleOkButtonClick()
    {
        if (achievementQueue.Count > 0)
        {
            DisplayNextAchievement();
        }
        OnOkButtonPressed?.Invoke();
    }

    public void HandleBackButtonClick()
    {
        OnBackButtonPressed?.Invoke();
    }

    public void ShowAchievementPanel()
    {
        achievementPanel.SetActive(true);
        // List<AchievementData> achievementsUnlock = AchievementManager.Instance.GetUnlockedAchievements();
        List<AchievementData> achievements = AchievementManager.Instance.GetAllAchievements();
        ClearAchievementPanel();

        foreach (var achievement in achievements)
        {
            Debug.Log($"Achievement Sudah Unlocked: {achievement.achievementName}");
            // Tampilkan achievement yang sudah unlock, misalnya melalui UI Manager
            ShowUnlockedAchievement(achievement);
        }
    }

    public void ShowUnlockedAchievement(AchievementData achievement)
    {
        // Buat elemen UI atau update bagian UI khusus achievement
        GameObject achievementUI = Instantiate(achievementPrefab, achievementViewport);
        achievementUI.GetComponentInChildren<Text>().text = "Terkunci";
        achievementUI.GetComponentInChildren<Image>().sprite = achievement.icon;
        Image image = achievementUI.GetComponentInChildren<Image>();
        Button achievementButton = achievementUI.GetComponent<Button>();
        achievementButton.onClick.AddListener(() => audioManager.PlaySFX(audioManager.click));


        Button descButton = achievementUI.transform.Find("Desc").GetComponent<Button>();
        descButton.GetComponentInChildren<Text>().text = achievement.description;
        descButton.onClick.AddListener(() => audioManager.PlaySFX(audioManager.click));

        // Mengakses child dari Image
        Transform childTransform = image.transform.GetChild(0); // Mendapatkan child pertama
        GameObject childObject = childTransform.gameObject;
        childObject.SetActive(true);
        if (achievement.isUnlocked)
        {
            achievementUI.GetComponentInChildren<Text>().text = achievement.achievementName;
            childObject.SetActive(false);
        }
    }

    private void ClearAchievementPanel()
    {
        foreach (Transform child in achievementViewport)
        {
            Destroy(child.gameObject);
        }
    }
}
