using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements.Experimental;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject panelGameStart;

    [SerializeField]
    private TextMeshProUGUI textCurrentScore;
    [SerializeField]
    private TextMeshProUGUI textBestScore;

    private int score = 0;
    public bool IsGameStart { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public int Score
    {
        get => score;
        set
        {
            if (IsGameOver == true) return;
            score = value;
            textCurrentScore.text = score.ToString();
        }
    }

    private IEnumerator Start()
    {

        if(Constants.IsDeactivateMain == true) 
        {
            GameStart();

            yield break;
        }

        while (true) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameStart();

                yield break;
            }
            yield return null;
        }
    }

    private void GameStart()
    {
        IsGameStart = true;
        panelGameStart.SetActive(false);
        textCurrentScore.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        if (IsGameOver == true) return;

        IsGameOver = true;
        panelGameStart.SetActive(true);
        textBestScore.gameObject.SetActive(true);

        int bestScore = PlayerPrefs.GetInt(Constants.BestScore);
        if(score > bestScore)
        {
            PlayerPrefs.SetInt(Constants.BestScore, score);
            textBestScore.text = $"<size=75>NEW</size>\n{score}";
        }
        else
        {
            textBestScore.text = $"<size=75>BEST</size>\n{bestScore}";
        }

        StartCoroutine(nameof(OnGameOver));
    }

        private IEnumerator OnGameOver()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Constants.IsDeactivateMain = true;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    yield break;
                }
                yield return null;
            }
        }

    [ContextMenu("Reset Data")]
    private void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}
