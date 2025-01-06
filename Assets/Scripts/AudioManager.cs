using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXsource;
    public AudioClip click;
    public AudioClip right;
    public AudioClip wrong;
    public AudioClip endQuiz;
    public AudioClip achievement;

    public Button muteButton;     // Tombol mute/unmute
    public Sprite muteIcon;       // Ikon mute
    public Sprite unmuteIcon;     // Ikon unmute

    private bool isMuted;

    void Start()
    {
        LoadMuteSettings();
        UpdateButtonIcon();
        muteButton.onClick.AddListener(ToggleMute);
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXsource.PlayOneShot(clip);
    }

    void ToggleMute()
    {
        isMuted = !isMuted;
        SFXsource.mute = isMuted;
        PlayerPrefs.SetInt("MuteSFX", isMuted ? 1 : 0);  // Simpan pengaturan mute (1 = mute, 0 = unmute)
        PlayerPrefs.Save();  // Pastikan pengaturan tersimpan
        UpdateButtonIcon();
    }

    void UpdateButtonIcon()
    {
        Image buttonImage = muteButton.GetComponent<Image>();
        buttonImage.sprite = isMuted ? muteIcon : unmuteIcon;
    }

    void LoadMuteSettings()
    {
        isMuted = PlayerPrefs.GetInt("MuteSFX", 0) == 1;  // Muat pengaturan mute (0 sebagai default)
        SFXsource.mute = isMuted;
    }



}
