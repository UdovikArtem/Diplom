using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1000)]
public class GameUI : MonoBehaviour
{
    public static GameUI Instance;

    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private GameObject gameUIPage;
    [SerializeField]
    private GameObject pausePage;
    [SerializeField]
    private GameObject gameOverPage;
    [SerializeField]
    private GameObject finishLevelPage;

    [SerializeField]
    private Scrollbar healthbar;
    [SerializeField]
    private GameObject slidingArea;

    private bool isPaused = false;
    private bool isFinished = false;

    public bool IsPaused {  get { return isPaused; } }
    public bool IsFinished { get { return isFinished; } }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        if (GameData.PlayerName != null)
        {
            nameText.text = GameData.PlayerName;
        }
        ChangeHealthbar();
        ChangeScore();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !isFinished && !Player.Instance.IsDead)
        {
            ChangePaused();
        }
    }

    virtual public void GameOverPage()
    {
        Time.timeScale = 0;
        gameUIPage.SetActive(false);
        gameOverPage.SetActive(true);
    }

    public void FinishLevelPage()
    {
        Time.timeScale = 0;
        isFinished = true;
        finishLevelPage.SetActive(true);
        gameUIPage?.SetActive(false);
    }

    virtual public void ChangePaused()
    {
        if (isPaused)
        {
            isPaused = false;
            gameUIPage.SetActive(true);
            pausePage.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            isPaused = true;
            gameUIPage.SetActive(false);
            pausePage.SetActive(true);
            Time.timeScale = 0;
        }
    }

    virtual public void ChangeHealthbar()
    {
        healthbar.size = Player.Instance.Health / 100f;
        if (healthbar.size > 0)
        {
            slidingArea.SetActive(true);
        }
        else
        {
            slidingArea.SetActive(false);
        }
    }

    virtual public void ChangeScore()
    {
        scoreText.text = "Score: " + GameData.Score;
    }
}
