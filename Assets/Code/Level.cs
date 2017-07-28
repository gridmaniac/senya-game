using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Level : MonoBehaviour {

    public int id;
    public int state;
    public string title;
    public string telegram;
    public VideoClip video;

    public int[] typesCountTable;
    public float pawChance;

    public string quiz1;
    public string[] quiz1variants;

    public string quiz2;
    public string[] quiz2variants;

    public string quiz3;
    public string[] quiz3variants;

    public Text score;
    public SpriteRenderer[] paws;

    public GameObject largeObject;
    public SpriteRenderer smallIconSprite;
    public GameObject scoreObject;
    public SpriteRenderer path;

    public AudioSource soundStart;
    public AudioSource soundUnlocked;

    private int pawsCount = 0;
    public int Paws
    {
        get { return this.pawsCount; }
    }

    private int scoreCount = 0;
    public int Score
    {
        get { return this.scoreCount; }
    }
    
    public AnimationCurve pulsation;
    private bool isPulsing;
    private float pulseTime;

    private float smallIconAlpha;

    private bool isUnlocking = false;

    private void Awake()
    {
        this.pulseTime = 0.0f;
        this.smallIconAlpha = 0.0f;

        if (PlayerPrefs.HasKey(this.id.ToString()))
        {
            string path = PlayerPrefs.GetString(this.id.ToString());
            if (path != null)
            {
                string[] pars = path.Split('&');
                int pUnlock = int.Parse(pars[0]);
                int pScore = int.Parse(pars[1]);
                int pPaws = int.Parse(pars[2]);

                this.scoreCount = pScore;
                this.pawsCount = pPaws;

                if (id > FindObjectOfType<MenuController>().LastId)
                {
                    FindObjectOfType<MenuController>().LastId = id;
                }

                this.state = pUnlock;
                
                if (pUnlock == 0)
                    FindObjectOfType<MenuController>().ShowSMS(this.telegram, this.video);
                else
                    this.Show();
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Show()
    {
        this.score.text = this.scoreCount.ToString();
        for (var i = 0; i < pawsCount; i++)
        {
            paws[i].enabled = true;
        }

        this.transform.localScale = new Vector3(1, 1, 1);
    }

    public void UnlockLevel()
    {
        string param = "1&" + this.scoreCount.ToString() + "&" + this.pawsCount.ToString();
        PlayerPrefs.SetString(this.id.ToString(), param);
        StartCoroutine(Unlock());
    }

    private IEnumerator Unlock()
    {
        this.isUnlocking = true;
        if (this.path != null)
        {
            this.path.GetComponent<Animator>().Play("PathLong", 0, 0);
        }

        FindObjectOfType<MenuController>().SwitchToClose();
        yield return new WaitForSeconds(1.0f);

        isUnlocking = false;

        this.score.text = this.scoreCount.ToString();
        for (var i = 0; i < pawsCount; i++)
        {
            paws[i].enabled = true;
        }

        this.soundUnlocked.Play();
        this.isPulsing = true;
    }

    private void Update()
    {
        if (this.isPulsing)
        {
            if (this.pulseTime <= 1.0f)
            {
                float v = this.pulsation.Evaluate(this.pulseTime);
                this.transform.localScale =
                    new Vector3(v, v, 1);

                this.pulseTime += Time.deltaTime * 2.0f;
            }
            else
            {
                this.pulseTime = 0.0f;
                this.isPulsing = false;
            }
        }

        if (this.smallIconSprite.color.a != this.smallIconAlpha)
        {
            var color = this.smallIconSprite.color;
            color.a = Mathf.Lerp(color.a, this.smallIconAlpha, Time.deltaTime * 5.0f);
            this.smallIconSprite.color = color;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Camera.main != null)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (this.GetComponentInChildren<Collider2D>() != null)
                {
                    bool isThis = this.GetComponentInChildren<Collider2D>().bounds.Contains(mousePosition);

                    if (isThis)
                    {
                        this.soundStart.Play();
                        this.isPulsing = true;

                        GameStatic.id = this.id;
                        GameStatic.state = this.state;
                        GameStatic.typesCountTable = this.typesCountTable;
                        GameStatic.pawChance = this.pawChance;
                        GameStatic.video = this.video;
                        GameStatic.quiz1 = this.quiz1;
                        GameStatic.quiz1variants = this.quiz1variants;
                        GameStatic.quiz2 = this.quiz2;
                        GameStatic.quiz2variants = this.quiz2variants;
                        GameStatic.quiz3 = this.quiz3;
                        GameStatic.quiz3variants = this.quiz3variants;
                        GameStatic.scoreCount = this.scoreCount;
                        GameStatic.pawsCount = this.pawsCount;

                        FindObjectOfType<MenuController>().StartLevel();
                    }
                }
            }
        }
    }

    public void SwitchToFar()
    {
        this.largeObject.SetActive(false);
        this.scoreObject.SetActive(false);
        this.smallIconAlpha = 1.0f;
    }

    public void SwitchToClose()
    {
        this.smallIconAlpha = 0.0f;
        this.largeObject.SetActive(true);
        this.scoreObject.SetActive(true);

        if (this.path != null && !this.isUnlocking)
        {
            this.path.GetComponent<Animator>().Play("Path", 0, 0);
        }
    }
}
