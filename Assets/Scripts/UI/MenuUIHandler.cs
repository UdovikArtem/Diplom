using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public static MenuUIHandler Instance;

    [SerializeField]
    private GameObject startPage;
    [SerializeField]
    private GameObject writeNamePage;
    [SerializeField]
    private GameObject messegePage;
    [SerializeField]
    private GameObject levelPage;

    [SerializeField]
    private TMP_InputField nameInpput;
    [SerializeField]
    private TMP_InputField passwordInpput;
    [SerializeField]
    private TextMeshProUGUI messegeText;
    [SerializeField]
    private TextMeshProUGUI[] levelsScore;


    private bool isContinue;
    // Start is called before the first frame update

    private void Start()
    {
        Instance = this;

        if (AuthenticationService.Instance.IsAuthorized)
        {
            OpenlevelPage();
        }
    }

    public void CheckNameAndPassword()
    {
        string name = nameInpput.text.Trim();
        string password = passwordInpput.text.Trim();

        if (name.Length >= 3 && name.Length <= 20 && isPasswordValid(password))
        {
            if (isContinue)
            {
                StartCoroutine(LoadUserWrapper(name, password));
            }
            else
            {
                StartCoroutine(SaveUserWrapper(name, password));
            }

        }
        else
        {
            LoadMessege("Имя пользователя дожно содержать от 3 до 20 букв, цифры или сиволы  ., -, @ и _," +
                " а пароль - от 8 до 30 букв, цифр или сиволов. В пароле должна быть одна буква верхнего и нежнего регистра," +
                "одна буква и один символ.");
            messegeText.fontSize = 20;
        }
        DelleteInput();
    }

    private IEnumerator LoadUserWrapper(string name, string password)
    {
        var task = GameManager.Instance.LoadUser(name, password);
        yield return new WaitUntil(() => task.IsCompleted);

        bool result = task.Result;
        if (result)
        {
            OpenlevelPage();
        }
    }

    private IEnumerator SaveUserWrapper(string name, string password)
    {
        var task = GameManager.Instance.SaveUser(name, password);
        yield return new WaitUntil(() => task.IsCompleted);

        bool result = task.Result;
        if (result)
        {
            OpenlevelPage();
        }
    }

    public void DelleteInput()
    {
        nameInpput.text = "";
        passwordInpput.text = "";
    }

    public void ContinueGame()
    {
        OpenWriteNamePage();
        isContinue = true;
    }

    public void StartGame()
    {
        OpenWriteNamePage();
        isContinue = false;
    }

    public void BackClick()
    {
        startPage.SetActive(true);
        writeNamePage.SetActive(false);
        levelPage.SetActive(false);
        DelleteInput();
    }

    public async void SignOut()
    {
        BackClick();
        await GameManager.Instance.ChangeUserData();
        AuthenticationService.Instance.SignOut();
    }

    public void OpenWriteNamePage()
    {
        startPage.SetActive(false);
        writeNamePage.SetActive(true);
        messegePage.SetActive(false);
        levelPage.SetActive(false);
        messegeText.fontSize = 36;

        nameInpput.ActivateInputField();
    }

    public void LoadMessege(string text)
    {
        writeNamePage.SetActive(false);
        messegePage.SetActive(true);
        levelPage.SetActive(false);
        messegeText.text = text;
    }

    public void OpenlevelPage()
    {
        startPage.SetActive(false);
        writeNamePage.SetActive(false);
        messegePage.SetActive(false);
        levelPage.SetActive(true);
        Debug.Log(GameData.Levels.ToString());
        for(int i = 0; i < GameData.Levels.Count; i++)
        {
            if (GameData.Levels[i].isAvailable)
            {
                levelsScore[i].text = "Лучший счет: " + GameData.Levels[i].bestScore;
            }
            else
            {
                levelsScore[i].text = "Не доступен";
            }
        }
    }

    public void OpenLevel(int levelId)
    {
        Debug.Log(GameData.Levels);
        Level thatLevel = GameData.Levels[levelId - 1];
        if (thatLevel.isAvailable && (thatLevel.isFinished || thatLevel.isFirstStart))
        {
            if (thatLevel.isFinished)
            {
                thatLevel = Level.RestartLevel(levelId, thatLevel.bestScore);
                GameData.Levels[levelId - 1] = thatLevel;
            }
            else
            {
                thatLevel = Level.RestartLevel(levelId);
                GameData.Levels[levelId - 1] = thatLevel;
            }
        }
        else if (!GameData.Levels[levelId - 1].isAvailable)
        {
            return;
        }
        GameData.CurrentLevel = thatLevel;

        GameData.PlayerHealth = thatLevel.health;
        GameData.CurrentCheckpoint = thatLevel.currentCheckpointId;
        GameData.DeadEnemiesId = thatLevel.deadEnemies;
        GameData.Score = thatLevel.currentScore;

        StartCoroutine(StartLevel(levelId));
    }

    private IEnumerator StartLevel(int levelId)
    {
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelId);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Загрузка: {progress * 100}%");
            yield return null;
        }
    }

    public bool isPasswordValid(string password)
    {
        if (password.Length < 8 || password.Length > 30)
        {
            return false;
        }

        bool hasUpperCase = false;
        bool hasLowerCase = false;
        bool hasDigit = false;
        bool hasSymbol = false;

        foreach (char c in password)
        {
            if (char.IsUpper(c))
            {
                hasUpperCase = true;
            }
            else if (char.IsLower(c))
            {
                hasLowerCase = true;
            }
            else if (char.IsDigit(c))
            {
                hasDigit = true;
            }
            else if (!char.IsLetterOrDigit(c))
            {
                hasSymbol = true;
            }
        }

        return hasUpperCase && hasLowerCase && hasDigit && hasSymbol;
    }
}
