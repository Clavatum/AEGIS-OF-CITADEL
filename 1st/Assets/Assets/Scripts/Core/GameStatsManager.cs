using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using static AuthMng;


public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    public int totalKills = 0;
    public int totalKillsInGame = 0;
    public float goldSpent = 0f;
    public float goldEarned = 0f;
    public float totalGold = 0f;

    public float completionTime = 0f;
    public float bestCompletionTime = Mathf.Infinity;
    public float totalPlayTime = 0f;

    public int gamesPlayed = 0;
    public int gamesWon = 0;

    private float startTime;
    private bool isGameActive = false;
    public static bool isDataLoaded = false;
    public static bool isSignedIn = false;

    private bool isLoadData;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            var authMng = FindObjectOfType<AuthMng>();
            authMng.OnSignedIn += HandleSignIn;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        SaveGameStats();
    }

    public void HandleSignIn(PlayerProfile profile)
    {
        isSignedIn = true;
        LoadCloudData();
    }

    public void StartGamePlayTime()
    {
        if (!isGameActive)
        {
            startTime = Time.time;
            isGameActive = true;
            gamesPlayed++;
            SaveGameStats();
            SaveToCloud();
        }
    }

    public void StopGamePlayTime()
    {
        if (isGameActive)
        {
            float elapsedTime = Time.time - startTime;
            totalPlayTime += elapsedTime;
            isGameActive = false;
            SaveGameStats();
            SaveToCloud();
        }
    }

    public void CompleteGame()
    {
        completionTime = Time.time - startTime;
        totalPlayTime += completionTime;

        if (YouWinPanelMng.gameWon)
        {
            if (completionTime < bestCompletionTime)
            {
                bestCompletionTime = completionTime;
            }
        }

        SaveGameStats();
        SaveToCloud();
    }

    public void AddKill()
    {
        totalKills++;
        totalKillsInGame++;
    }

    public void SpendGold(float amount)
    {
        goldSpent += amount;
        totalGold -= amount;
        SaveGameStats();
        SaveToCloud();
    }

    public void EarnGold(float amount)
    {
        goldEarned += amount;
        totalGold += amount;
        SaveGameStats();
        SaveToCloud();
    }

    public void IncrementGamesWon()
    {
        gamesWon++;
    }

    public void SaveGameStats()
    {
        PlayerPrefs.SetFloat("TotalGold", totalGold);
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.SetFloat("BestCompletionTime", bestCompletionTime);
        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.SetInt("GamesPlayed", gamesPlayed);
        PlayerPrefs.SetInt("GamesWon", gamesWon);
        PlayerPrefs.Save();
    }

    private async void SaveToCloud()
    {
        if (!isSignedIn) return;

        var data = new Dictionary<string, object>
        {
            { "TotalGold", totalGold },
            { "TotalKills", totalKills },
            { "TotalPlayTime", totalPlayTime },
            { "BestCompletionTime", bestCompletionTime },
            { "GamesPlayed", gamesPlayed },
            { "GamesWon", gamesWon }
        };

        await CloudSaveManager.SaveToCloud(data);
    }

    public async void LoadCloudData()
    {
        if (!isSignedIn) return;

        var cloudData = await CloudSaveManager.LoadFromCloud();

        if (cloudData != null)
        {
            if (cloudData.TryGetValue("TotalGold", out var totalGoldValue) && totalGoldValue is float totalGoldFloat)
                totalGold = totalGoldFloat;

            if (cloudData.TryGetValue("TotalKills", out var totalKillsValue) && totalKillsValue is int totalKillsInt)
                totalKills = totalKillsInt;

            if (cloudData.TryGetValue("TotalPlayTime", out var totalPlayTimeValue) && totalPlayTimeValue is float totalPlayTimeFloat)
                totalPlayTime = totalPlayTimeFloat;

            if (cloudData.TryGetValue("BestCompletionTime", out var bestCompletionTimeValue) && bestCompletionTimeValue is float bestCompletionTimeFloat)
                bestCompletionTime = bestCompletionTimeFloat;

            if (cloudData.TryGetValue("GamesPlayed", out var gamesPlayedValue) && gamesPlayedValue is int gamesPlayedInt)
                gamesPlayed = gamesPlayedInt;

            if (cloudData.TryGetValue("GamesWon", out var gamesWonValue) && gamesWonValue is int gamesWonInt)
                gamesWon = gamesWonInt;

            isDataLoaded = true;
            SaveGameStats();
        }
        else
        {
            Debug.LogWarning("Cloud data is empty.");
        }
    }
}
