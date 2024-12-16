using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Home[] homes;
    [SerializeField] private Frogger frogger;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Text timeText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text scoreText;

    private int lives = 5;
    private int score = 0;

    private void Awake()
    {
        if (Instance != null)
            DestroyImmediate(gameObject);
        else
            Instance = this;
    }

    private void OnDestroy() => Instance = null;

    private void Start() => StartNewGame();

    private void StartNewGame()
    {
        gameOverMenu.SetActive(false);
        SetLives(5);
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        foreach (var home in homes)
            home.enabled = false;

        Respawn();
    }

    private void Respawn()
    {
        frogger.Respawn();
        StopAllCoroutines();
    }

    public void Died()
    {
        SetLives(lives - 1);
        if (lives > 0)
            Invoke(nameof(Respawn), 1f);
        else
            Invoke(nameof(GameOver), 1f);
    }

    private void GameOver()
    {
        frogger.gameObject.SetActive(false);
        gameOverMenu.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(CheckForPlayAgain());
    }

    private IEnumerator CheckForPlayAgain()
    {
        while (!Input.GetKeyDown(KeyCode.Return))
            yield return null;

        StartNewGame();
    }

    public void HomeOccupied()
    {
        frogger.gameObject.SetActive(false);

        if (IsLevelCleared())
        {
            SetLives(lives + 1);
            Invoke(nameof(StartNewLevel), 1f);
        }
        else
        {
            Invoke(nameof(Respawn), 1f);
        }
    }

    private bool IsLevelCleared()
    {
        foreach (var home in homes)
            if (!home.enabled)
                return false;

        return true;
    }

    private void SetLives(int newLives)
    {
        lives = newLives;
        livesText.text = lives.ToString();
    }
}
