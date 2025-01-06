using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public GameObject exitPopup; // Assign Panel popup di Editor
    private float backButtonPressedTime = 0f; // Waktu terakhir tombol back ditekan
    private const float doublePressInterval = 0.5f; // Interval double press

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Tombol Back di Android
        {
            if (Time.time - backButtonPressedTime < doublePressInterval)
            {
                ShowExitPopup(); // Tampilkan popup
            }
            else
            {
                backButtonPressedTime = Time.time;
                Debug.Log("Tekan kembali sekali lagi untuk keluar.");
            }
        }
    }

    public void ShowExitPopup()
    {
        exitPopup.SetActive(true); // Tampilkan popup
    }

    public void HideExitPopup()
    {
        exitPopup.SetActive(false); // Sembunyikan popup
    }

    public void ExitApplication()
    {
        Application.Quit(); // Keluar dari aplikasi
    }
}
