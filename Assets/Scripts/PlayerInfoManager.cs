using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    private const string PlayerNameKey = "PlayerName";

    // Menyimpan nama pemain ke PlayerPrefs
    public void SavePlayerName(string playerName)
    {
        PlayerPrefs.SetString(PlayerNameKey, playerName);
        PlayerPrefs.Save();
    }

    // Mengambil nama pemain dari PlayerPrefs
    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, "Guest"); // Default "Guest" jika belum ada nama
    }

    //Memeriksa apakah sudah ada PlayerName
    public bool IsPlayerNameSet()
    {
        return PlayerPrefs.HasKey(PlayerNameKey);
    }
}
