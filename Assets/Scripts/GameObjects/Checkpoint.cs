using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private string id;

    private Transform player;

    [SerializeField] 
    private TextWriter text;

    public string Id => id;

    private void Start()
    {
        player = Player.Instance.transform;
        Debug.Log(GameData.CurrentCheckpoint);
        if(GameData.CurrentCheckpoint == id)
        {
            player.position = transform.position;
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this); // сохраняем изменения в сцену
            #endif
        }
    }

    // Start is called before the first frame update
    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameData.CurrentCheckpoint = id;
            GameData.CurrentLevel.currentCheckpointId = id;
            GameData.CurrentLevel.deadEnemies = GameData.DeadEnemiesId;
            GameData.CurrentLevel.currentScore = GameData.Score;
            GameData.CurrentLevel.health = GameData.PlayerHealth;

            GameData.Levels[GameData.CurrentLevel.levelId - 1] = GameData.CurrentLevel;
            await GameManager.Instance.ChangeUserData();

            GetComponent<Collider2D>().enabled = false; 
            text.ShowText();
        }
    }

}
