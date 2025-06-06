using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameData.CurrentLevel.isFinished = true;
            if(GameData.CurrentLevel.bestScore < GameData.Score)
            {
                GameData.CurrentLevel.bestScore = GameData.Score;
            }

            if (GameData.CurrentLevel.levelId != GameData.Levels.Count)
            {
                GameData.Levels[GameData.CurrentLevel.levelId - 1] = GameData.CurrentLevel;
                GameData.Levels[GameData.CurrentLevel.levelId].isAvailable = true;
            }

            await GameManager.Instance.ChangeUserData();

            GetComponent<Collider2D>().enabled = false;
            GameUI.Instance.FinishLevelPage();
        }
    }
}
