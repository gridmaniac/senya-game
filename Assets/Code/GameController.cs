using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameController : MonoBehaviour {

    public GameObject pauseOverlay;
    public GameObject gameOverOverlay;

    public NotificationController notifier;
    public Score score;

    public AudioSource soundGameOver;

    public Sprite waveSprite;
    public Sprite muteSprite;
    public Image muteButton;

    public Image gameOverLeft;
    public Sprite[] pawsLeftSprites;

    public bool IsGameOver;
    public bool IsPaused;
    public bool IsQuiz;

    public GameObject[] paws;

    private int pawsCount;

    private bool isMuted;
    private float targetPauseAlpha;
    private float pauseAlpha
    {
        get { return this.pauseOverlay.GetComponent<CanvasGroup>().alpha; }
        set { this.pauseOverlay.GetComponent<CanvasGroup>().alpha = value; }
    }
    private float targetGameOverAlpha;
    private float gameOverAlpha
    {
        get { return this.gameOverOverlay.GetComponent<CanvasGroup>().alpha; }
        set { this.gameOverOverlay.GetComponent<CanvasGroup>().alpha = value; }
    }

    private void Start()
    {
        this.ShowPaws();
    }

    private void ShowPaws()
    {
        int pawsCount = GameStatic.pawsCount;
        for (var i = 0; i < pawsCount; i++)
        {
            this.paws[i]
                .GetComponentInChildren<Animator>()
                .Play("PawShow", 0);
        }

        this.pawsCount = pawsCount;
    }

    public void ShowNextPaw()
    {
        this.paws[this.pawsCount]
            .GetComponentInChildren<Animator>()
            .Play("PawShow", 0);

        this.pawsCount++;

        string state = "1";
        if (pawsCount > 4)
        {
            state = "2";

            int newLevelId = GameStatic.id + 1;
            if (newLevelId < 21)
            {
                PlayerPrefs.SetString(newLevelId.ToString(), "0&0&0");
            }

            GameStatic.state = 2;
        }

        string param = state + "&" + this.score.Value.ToString() + "&" + this.pawsCount.ToString();
        PlayerPrefs.SetString(GameStatic.id.ToString(), param);
        
        switch (this.pawsCount)
        {
            case 1:
                this.notifier.NotifyPaws4();
                break;
            case 2:
                this.notifier.NotifyPaws3();
                break;
            case 3:
                this.notifier.NotifyPaws2();
                break;
            case 4:
                this.notifier.NotifyPaws1();
                break;
            case 5:
                this.notifier.NotifyPaws0();
                break;
        }
    }

    private void Update()
    {
        if (Mathf.Abs(this.pauseAlpha - this.targetPauseAlpha) > 0.1f)
        {
            this.pauseAlpha = Mathf.Lerp(this.pauseAlpha, this.targetPauseAlpha, Time.deltaTime * 7.0f);
        }

        if (this.pauseAlpha <= 0.1f && this.IsPaused)
        {
            this.IsPaused = false;
            this.pauseOverlay.SetActive(false);
        }

        if (Mathf.Abs(this.gameOverAlpha - this.targetGameOverAlpha) > 0.1f)
        {
            this.gameOverAlpha = Mathf.Lerp(this.gameOverAlpha, this.targetGameOverAlpha, Time.deltaTime * 7.0f);
        }

        if (this.gameOverAlpha <= 0.1f && this.IsGameOver)
        {
            this.IsGameOver = false;
            this.gameOverOverlay.SetActive(false);
        }
    }

    public void Mute()
    {
        if (!this.isMuted)
        {
            muteButton.sprite = muteSprite;
            AudioListener.volume = 0.0f;
            this.isMuted = true;
        }
        else
        {
            muteButton.sprite = waveSprite;
            AudioListener.volume = 1.0f;
            this.isMuted = false;
        }
    }

    public void GameOver()
    {
        if (!this.IsGameOver)
        {
            this.gameOverOverlay.SetActive(true);
            this.targetGameOverAlpha = 1.0f;
            this.gameOverAlpha = 0.15f;
            this.gameOverLeft.sprite = this.pawsLeftSprites[this.pawsCount];
            this.IsGameOver = true;
            this.soundGameOver.Play();
        }
        else
        {
            this.targetGameOverAlpha = 0.0f;
        }
    }

    public void Pause()
    {
        if (!this.IsPaused)
        {
            this.pauseOverlay.SetActive(true);
            this.targetPauseAlpha = 1.0f;
            this.pauseAlpha = 0.15f;
            this.IsPaused = true;
        }
        else
        {
            this.targetPauseAlpha = 0.0f;
        }
    }

    public void Exit()
    {
        this.targetPauseAlpha = 0.0f;
        SceneManager.LoadSceneAsync("menu");
    }

    public void StopVideo()
    {
        FindObjectOfType<VideoPlayer>().Stop();
        GameObject.Find("ExitVideo").SetActive(false);
    }
}
