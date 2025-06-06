using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class GameCloud : MonoBehaviour
{
    public static GameCloud Instance;

    private const string PLAYER_CLOUD_KEY = "PLAYER_DATA";

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

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

    }

    virtual public async Task SaveData()
    {
        SaveUserData userData = new()
        {
            name = GameData.PlayerName,
            levels = GameData.Levels
        };

        Dictionary<string, object> data = new Dictionary<string, object>() { {PLAYER_CLOUD_KEY, userData} };

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }

    public async Task LoadData()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>{ PLAYER_CLOUD_KEY});
        SaveUserData userData = new SaveUserData();
        if (data.TryGetValue(PLAYER_CLOUD_KEY, out var name))
        {
            userData = name.Value.GetAs<SaveUserData>();

            GameData.PlayerName = userData.name;
            GameData.Levels = userData.levels;
        }
        Debug.Log(GameData.Levels.ToString());
    }
}
