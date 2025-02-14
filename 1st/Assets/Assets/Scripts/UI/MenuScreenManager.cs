using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuScreenManager : MonoBehaviour
{
    public GameObject statsPanel;
    public GameObject settingsPanel;
    public TMP_Text gamesPlayedText;
    public TMP_Text gamesWonText;

    public TMP_Text totalKillsText;
    public TMP_Text bestCompletionTimeText;
    public TMP_Text totalPlayTimeText;
    public TMP_Text totalGoldText;

    public Button startButton;
    public Button settingsButton;
    public Button statsButton;
    public Button exitButton;
    public Button closeStatsButton;
    public Button closeSettingsButton;

    public TMP_Dropdown qualityDropdown;
    public Slider soundSlider;
    public TMP_Text soundValueText;

    [Header("UI References")]
    public TMP_Text usernameText;

    public static bool hasComebackToMenu = false;

    void Start()
    {
        if (hasComebackToMenu)
        {
            AudioManager.Instance.PlayBackgroundMusic();
        }
        statsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        statsButton.onClick.AddListener(OpenStats);
        exitButton.onClick.AddListener(ExitGame);
        closeStatsButton.onClick.AddListener(CloseStats);
        closeSettingsButton.onClick.AddListener(CloseSettings);

        qualityDropdown.onValueChanged.AddListener(SetQuality);
        PopulateQualityDropdown();

        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQualityLevel);
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();

        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        UpdateSoundValueText();

        UpdateStats();
    }

    private void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        string username = PlayerPrefs.GetString("Username", "Guest");
        usernameText.text = username;
        totalGoldText.text = "Total Gold: " + PlayerPrefs.GetFloat("TotalGold").ToString("F2");
    }

    public void StartGame()
    {
        AudioManager.Instance.StopBackgroundMusic();
        GameStatsManager.Instance.StartGamePlayTime();
        SceneManager.LoadScene("GameScene");
    }

    public void OpenStats()
    {
        statsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        UpdateStats();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        statsPanel.SetActive(false);
    }

    public void CloseStats()
    {
        statsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void UpdateStats()
    {
        gamesPlayedText.text = "Games Played: " + GameStatsManager.Instance.gamesPlayed.ToString();
        gamesWonText.text = "Games Won: " + GameStatsManager.Instance.gamesWon.ToString();

        totalKillsText.text = "Total Kills: " + GameStatsManager.Instance.totalKills;
        totalPlayTimeText.text = "Total Play Time: " + FormatTime(GameStatsManager.Instance.totalPlayTime);

        float bestCompletionTime = GameStatsManager.Instance.bestCompletionTime;
        bestCompletionTimeText.text = bestCompletionTime != Mathf.Infinity
            ? "Best Completion Time: " + FormatTime(bestCompletionTime)
            : "Best Completion Time: N/A";
    }

    void PopulateQualityDropdown()
    {
        qualityDropdown.ClearOptions();

        List<string> qualityNames = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(qualityNames);

        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQualityLevel;
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }

    public void SetSoundVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SoundVolume", volume);
        UpdateSoundValueText();
    }

    void UpdateSoundValueText()
    {
        soundValueText.text = (soundSlider.value * 100).ToString("0") + "%";
    }

    private string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }

    private void OnDestroy()
    {
        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        soundSlider.onValueChanged.RemoveListener(SetSoundVolume);
    }
}
