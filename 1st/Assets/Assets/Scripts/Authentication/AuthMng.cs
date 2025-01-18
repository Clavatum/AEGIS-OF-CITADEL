using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;
using System;


public class AuthMng : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField userNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text logTxt;

    [SerializeField] private TMP_Text userNameText;

    public event Action<PlayerProfile> OnSignedIn;

    private PlayerInfo playerInfo;
    private PlayerProfile playerProfile;
    private GameStatsManager gameStatsManager;

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        gameStatsManager = GameStatsManager.Instance;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No Internet Connection");
            logTxt.text = "Internet connection lost!";
        }
    }

    // Button event
    public async void SignIn()
    {
        string userName = userNameInput.text;
        string password = passwordInput.text;

        await SignInWithUsernamePasswordAsync(userName, password);

    }

    // Button event
    public async void SignUp()
    {
        string userName = userNameInput.text;
        string password = passwordInput.text;

        if (!AssertionScript.IsValidUsername(userName, out string usernameError))
        {
            logTxt.text = usernameError;
            return;
        }

        if (!AssertionScript.IsValidPassword(password, out string passwordError))
        {
            logTxt.text = passwordError;
            return;
        }

        await SignUpWithUsernamePasswordAsync(userName, password);
    }

    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
            logTxt.text = "SignUp is successful.";
            LoadGameSceneByIndex(1, username);

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;
            PlayerPrefs.SetString("Username", name);

            OnSignedIn?.Invoke(playerProfile);

            var cloudData = await CloudSaveManager.LoadFromCloud();
            if (cloudData == null || cloudData.Count == 0)
            {
                Debug.Log("New player detected. Initializing stats.");
                await CloudSaveManager.InitializeNewPlayerData();
                await CloudSaveManager.ApplyCloudDataToGame();
            }
            else
            {
                Debug.Log("Returning player detected. Loading stats.");
                await CloudSaveManager.ApplyCloudDataToGame();
            }
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "SignUp failed: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "Request failed: " + ex.Message;
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
            logTxt.text = "SignIn is successful.";
            LoadGameSceneByIndex(1, username);

            playerInfo = AuthenticationService.Instance.PlayerInfo;
            var name = await AuthenticationService.Instance.GetPlayerNameAsync();

            playerProfile.playerInfo = playerInfo;
            playerProfile.Name = name;
            PlayerPrefs.SetString("Username", name);

            OnSignedIn?.Invoke(playerProfile);

            var cloudData = await CloudSaveManager.LoadFromCloud();
            if (cloudData == null || cloudData.Count == 0)
            {
                Debug.Log("New player detected. Initializing stats.");
                await CloudSaveManager.InitializeNewPlayerData();
                await CloudSaveManager.ApplyCloudDataToGame();
            }
            else
            {
                Debug.Log("Returning player detected. Loading stats.");
                await CloudSaveManager.ApplyCloudDataToGame();
            }
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "SignIn failed: " + ex.Message;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            logTxt.text = "Request failed: " + ex.Message;
        }
    }

    public static void LoadGameSceneByIndex(int sceneIndex, string username)
    {
        PlayerPrefs.SetString("Username", username);
        SceneManager.LoadScene(sceneIndex);
    }

    [Serializable]
    public struct PlayerProfile
    {
        public PlayerInfo playerInfo;
        public string Name;
    }
}
