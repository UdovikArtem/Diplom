using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private int CountOfLevels = 3;
    private bool isCorrect = false;
    private string text = "";
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CloudSignUp(string name, string password)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn && !AuthenticationService.Instance.IsAuthorized)
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(name, password);
            Debug.Log("SignUp is successful.");
            return true;
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            text = "Пользователь уже существует";
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
            text = "Не получилось подключиться";
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            text = "Произошла непредвиденная ошибка";
        }

        return false;
    }

    public async Task<bool> CloudSignIn(string name, string password)
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn && !AuthenticationService.Instance.IsAuthorized)
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(name, password);
            Debug.Log("SignIn is successful.");
            return true;
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            text = "Неверно введено имя пользователя или пароль";
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            text = "Не получилось подключиться";
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            text = "Произошла непредвиденная ошибка";
        }

        return false;
    }


    public async Task<bool> SaveUser(string name, string password)
    {
        isCorrect = await CloudSignUp(name, password);

        if (isCorrect)
        {
            string path = Application.persistentDataPath + "/save_user_file.json";


            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);

                ListUsersData dataCheck = JsonUtility.FromJson<ListUsersData>(json);

                bool isExist = false;
                foreach (var user in dataCheck.users)
                {
                    if (user.name.Equals(name))
                    {
                        isExist = true;
                    }
                }

                if (!isExist)
                {
                    SaveUserNameData data = new SaveUserNameData(name, password);

                    dataCheck.users.Add(data);

                    MakeNewUser(dataCheck, name);
                    return true;
                }
                else
                {
                    MenuUIHandler.Instance.LoadMessege("Пользователь с таким именем уже существует");
                    return false;
                }
            }
            else
            {
                ListUsersData data = new ListUsersData();
                data.users.Add(new SaveUserNameData(name, password));
                //File.Create(path);
                MakeNewUser(data, name);
                return true;
            }
        }
        else
        {
            MenuUIHandler.Instance.LoadMessege(text);
            return false;
        }
    }

    public async void MakeNewUser(ListUsersData data, string userName)
    {
        string newJson = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/save_user_file.json", newJson);

        SaveUserData newUserData = new SaveUserData();
        newUserData.name = userName;
        List<Level> levels = new List<Level>();
        for (int id = 1; id <= CountOfLevels; id++) {
            Level level = new Level();
            level.levelId = id;
            if (id == 1)
            {
                level.isAvailable = true;
            }
            else
            {
                level.isAvailable = false;
            }
            level.bestScore = 0;
            levels.Add(level);
        }
        newUserData.levels = levels;

        string userDataJson = JsonUtility.ToJson(newUserData);
        string directoryPath = Application.persistentDataPath + "/" + userName;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(directoryPath + "/data.json", userDataJson);

        GameData.PlayerName = userName;
        GameData.PlayerHealth = 100;
        GameData.Levels = levels;

        await GameCloud.Instance.SaveData();
    }

    public async Task<bool> LoadUser(string name, string password)
    {
        isCorrect = await CloudSignIn(name, password);

        string path = Application.persistentDataPath + "/save_user_file.json";
        string directoryPath = Application.persistentDataPath + "/" + name;
        string userDataPath = directoryPath + "/data.json";
        if (!isCorrect)
        {
            
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ListUsersData data = JsonUtility.FromJson<ListUsersData>(json);

                foreach (var user in data.users)
                {
                    if (user.name.Equals(name) && user.password.Equals(password))
                    {
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }                      
                        if (File.Exists(userDataPath))
                        {
                            string userDatajson = File.ReadAllText(userDataPath);
                            SaveUserData loadUserData = JsonUtility.FromJson<SaveUserData>(userDatajson);
                            GameData.PlayerName = name;
                            GameData.Levels = loadUserData.levels;
                        }
                        else
                        {
                            MenuUIHandler.Instance.LoadMessege(text);
                            return false;
                        }
                        return true;
                    }
                }

                MenuUIHandler.Instance.LoadMessege(text);
            }

            return false;
        }
        else
        {
            await GameCloud.Instance.LoadData();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                ListUsersData data = JsonUtility.FromJson<ListUsersData>(json);

                bool isUserExists = false;
                foreach (var user in data.users)
                {
                    if (user.name.Equals(name) && user.password.Equals(password))
                    {
                        isUserExists = true;
                        break;
                    }
                }
                if (!isUserExists)
                {
                    SaveUserNameData userData = new SaveUserNameData(name, password);
                    
                    data.users.Add(userData);
                    string newJson = JsonUtility.ToJson(data);
                    File.WriteAllText(Application.persistentDataPath + "/save_user_file.json", newJson);
                }
            }
            else
            {
                ListUsersData data = new ListUsersData();
                data.users.Add(new SaveUserNameData(name, password));
                string newJson = JsonUtility.ToJson(data);
                File.WriteAllText(Application.persistentDataPath + "/save_user_file.json", newJson);
            }
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            SaveUserData newUserData = new SaveUserData();
            newUserData.name = GameData.PlayerName;
            newUserData.levels = GameData.Levels;
            string userDataJson = JsonUtility.ToJson(newUserData);
            File.WriteAllText(userDataPath, userDataJson);

            return true;
        }
    }

    virtual public async Task ChangeUserData()
    {
        SaveUserData newUserData = new SaveUserData();
        newUserData.name = GameData.PlayerName;
        newUserData.levels = GameData.Levels;
        if (GameData.CurrentLevel != null)
        {
            newUserData.levels[GameData.CurrentLevel.levelId - 1] = GameData.CurrentLevel;
        }

        string userDataJson = JsonUtility.ToJson(newUserData);
        File.WriteAllText(Application.persistentDataPath + "/" + GameData.PlayerName + "/data.json", userDataJson);
        if (isCorrect)
        {
            await GameCloud.Instance.SaveData();
        }
    }
}
