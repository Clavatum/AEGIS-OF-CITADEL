using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager Instance;

    public int totalKills = 0;         // �ld�r�len d��man say�s�
    public float goldSpent = 0f;       // Harcanan alt�n
    public float goldEarned = 0f;      // Kazan�lan alt�n
    public float totalGold = 0f;       // Toplam alt�n (kal�c�)

    public float completionTime = 0f;  // Bu oyun i�in tamamlanma s�resi
    public float bestCompletionTime = Mathf.Infinity; // En k�sa tamamlanma s�resi
    public float totalPlayTime = 0f;   // Toplam oynama s�resi (kal�c�)

    private float startTime;           // Oyun ba�lang�� zaman�

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameStats();  // Oyun istatistiklerini y�kle
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        startTime = Time.time;  // Oyun ba�lang�� zaman�
    }

    public void AddKill()
    {
        totalKills++;
    }

    public void SpendGold(float amount)
    {
        goldSpent += amount;
        totalGold -= amount;
    }

    public void EarnGold(float amount)
    {
        goldEarned += amount;
        totalGold += amount;
    }

    public void CompleteGame()
    {
        completionTime = Time.time - startTime;  // Oyun s�resi
        totalPlayTime += completionTime;         // Toplam oynama s�resi artar

        // En k�sa tamamlanma s�resi g�ncellenir
        if (completionTime < bestCompletionTime)
        {
            bestCompletionTime = completionTime;
        }

        // Alt�n ve di�er de�erler kaydedilir
        SaveGameStats();
    }

    // �statistikleri PlayerPrefs ile kaydetme
    public void SaveGameStats()
    {
        PlayerPrefs.SetFloat("TotalGold", totalGold);
        PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
        PlayerPrefs.SetFloat("BestCompletionTime", bestCompletionTime);
        PlayerPrefs.SetInt("TotalKills", totalKills);
        PlayerPrefs.Save();
    }

    // �statistikleri y�kleme
    public void LoadGameStats()
    {
        totalGold = PlayerPrefs.GetFloat("TotalGold", 0f);
        totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f);
        bestCompletionTime = PlayerPrefs.GetFloat("BestCompletionTime", Mathf.Infinity);
        totalKills = PlayerPrefs.GetInt("TotalKills", 0);
    }
}
