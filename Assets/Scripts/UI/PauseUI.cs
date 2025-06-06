using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class PauseUI : MonoBehaviour
{
    public void ContinueGame()
    {
        GameUI.Instance.ChangePaused();
    }

    public async void RestartGame()
    {
        GameData.CurrentLevel = Level.RestartLevel(GameData.CurrentLevel.levelId,
            GameData.CurrentLevel.bestScore);
        GameData.Levels[GameData.CurrentLevel.levelId - 1] = GameData.CurrentLevel;
        GameData.PlayerHealth = 100;
        GameData.Score = 0;
        GameData.CurrentCheckpoint = null;
        GameData.DeadEnemiesId = new List<string>();
        await GameManager.Instance.ChangeUserData();
        
        StartCoroutine(RestartCoroutine());
        Time.timeScale = 1;
    }

    public async void ExitGame()
    {
        StartCoroutine(LoadScene(0));
        await GameManager.Instance.ChangeUserData();
        
        Time.timeScale = 1;
    }

    public void ToLastCheckpoint()
    {
        Level levelData = GameData.CurrentLevel;
        GameData.PlayerHealth = levelData.health;
        GameData.Score = levelData.currentScore;
        GameData.DeadEnemiesId = levelData.deadEnemies;

        StartCoroutine(RestartCoroutine());
        Time.timeScale = 1;
    }

    public async void NextLevel(int nextLevelId)
    {
        StartCoroutine(LoadScene(nextLevelId));
        GameData.CurrentLevel = Level.RestartLevel(nextLevelId, GameData.Levels[nextLevelId - 1].bestScore);
        GameData.Score = 0;
        GameData.PlayerHealth = 100;
        GameData.CurrentCheckpoint = null;
        GameData.DeadEnemiesId = new List<string>();
        GameData.Levels[nextLevelId - 1] = GameData.CurrentLevel;

        await GameManager.Instance.ChangeUserData();
        
        Time.timeScale = 1;
    }

    private IEnumerator LoadScene(int sceneId)
    {
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Загрузка: {progress * 100}%");
            yield return null;
        }
    }

    private IEnumerator RestartCoroutine()
    {
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
            SceneManager.GetActiveScene().buildIndex
        );

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Загрузка: {progress * 100}%");
            yield return null;
        }
    }
}
