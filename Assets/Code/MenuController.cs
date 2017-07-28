using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class MenuController : MonoBehaviour {
    public GameObject gui;
    public GameObject camera;
    public GameObject videoPlayer;
    public Text telegramText;
    public AudioSource telegramSound;
    public GameObject videoControls;
    public GameObject info;

    public Text score;
    public Text title;

    public int LastId;

    public GameObject map1;
    public GameObject map1Top;
    public GameObject map2;
    public GameObject map2Top;

    public Camera cam;

    public AudioSource soundZoom;
    public GameObject mOverlay;

    public GameObject top;
    private float topAlpha;
    private float topOffset;

    private float targetCamSize;
    private Vector3 targetCamPos;
    private float camZoomSpeed = 5.0f;
    private bool isCamSticky;

    private bool isWatched;

    private Vector3 mouseOrigin;
    private Vector3 mousePos
    {
        get { return this.cam.ScreenToWorldPoint(Input.mousePosition); }
    }

    private bool close;

    private bool isMOverlay;
    private float targetMOverlayAlpha;
    private float mOverlayAlpha
    {
        get { return this.mOverlay.GetComponent<CanvasGroup>().alpha; }
        set { this.mOverlay.GetComponent<CanvasGroup>().alpha = value; }
    }

    private float videoLength;

    void Awake()
    {
        //PlayerPrefs.DeleteAll();

        /*PlayerPrefs.SetString("1", "1&365&2");
        PlayerPrefs.SetString("2", "1&1040&0");
        PlayerPrefs.SetString("3", "1&2626&0");
        PlayerPrefs.SetString("4", "1&422&0");
        PlayerPrefs.SetString("5", "1&748&0");
        PlayerPrefs.SetString("6", "1&3248&0");
        PlayerPrefs.SetString("7", "1&426&0");
        PlayerPrefs.SetString("8", "1&828&0");
        PlayerPrefs.SetString("9", "1&1220&0");
        PlayerPrefs.SetString("10", "1&230&0");
        PlayerPrefs.SetString("11", "1&422&0");
        PlayerPrefs.SetString("12", "1&659&0");
        PlayerPrefs.SetString("13", "1&142&0");
        PlayerPrefs.SetString("14", "1&552&0");
        PlayerPrefs.SetString("15", "1&426&0");
        PlayerPrefs.SetString("16", "1&1245&0");
        PlayerPrefs.SetString("17", "1&729&0");
        PlayerPrefs.SetString("18", "1&1220&0");
        PlayerPrefs.SetString("19", "1&761&0");
        PlayerPrefs.SetString("20", "1&4220&0");*/

        if (!PlayerPrefs.HasKey("intro"))
            SceneManager.LoadScene("intro");
    }

    // Use this for initialization
    void Start () {
        this.targetCamSize = 5.0f;
        this.targetCamPos = this.cam.transform.position;
        this.close = false;

        var levels = FindObjectsOfType<Level>();
        var level = this.GetLevelById(this.LastId);
        this.SwitchToCloseInstant(level.transform.position);

        this.title.text = level.title;

        int sum = 0;
        foreach (var l in levels)
        {
            sum += l.Score;
        }

        this.score.text = sum.ToString();
    }
	
	// Update is called once per frame
	void Update () {
		if (this.cam.orthographicSize != targetCamSize)
        {
            this.cam.orthographicSize 
                = Mathf.Lerp(this.cam.orthographicSize, 
                targetCamSize, Time.deltaTime * this.camZoomSpeed);
        }

        if (Vector3.Distance(this.cam.transform.position, targetCamPos) > 0.05f && this.isCamSticky)
        {
            this.cam.transform.position 
                = Vector3.Lerp(this.cam.transform.position, 
                this.targetCamPos, Time.deltaTime * 7.0f);
        }
        else this.isCamSticky = false;

        if (close)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.mouseOrigin = this.mousePos;
            }

            if (Input.GetMouseButton(0))
            {
                this.cam.transform.Translate(this.mouseOrigin - this.mousePos);
            }
        }

        if (this.top.GetComponent<CanvasGroup>().alpha != this.topAlpha)
        {
            this.top.GetComponent<CanvasGroup>().alpha
                = Mathf.Lerp(
                    this.top.GetComponent<CanvasGroup>().alpha,
                    this.topAlpha, Time.deltaTime * 5.0f);
        }

        if (this.top.GetComponent<RectTransform>().offsetMax.y != this.topOffset)
        {
            var rect = this.top.GetComponent<RectTransform>();
            var offset = rect.offsetMax.y;
            offset = Mathf.Lerp(offset, this.topOffset, Time.deltaTime * 5.0f);
            rect.offsetMax = new Vector2(rect.offsetMax.x, offset);
        }

        if (Mathf.Abs(this.mOverlayAlpha - this.targetMOverlayAlpha) > 0.1f)
        {
            this.mOverlayAlpha = Mathf.Lerp(this.mOverlayAlpha, this.targetMOverlayAlpha, Time.deltaTime * 7.0f);
        }

        if (this.mOverlayAlpha <= 0.1f && this.isMOverlay)
        {
            this.isMOverlay = false;
            this.mOverlay.SetActive(false);
        }
    }

    private Level GetLevelById(int id)
    {
        var levels = FindObjectsOfType<Level>();
        foreach (var level in levels)
        {
            if (level.id == id) return level;
        }
        return null;
    }

    public void SwitchToClose()
    {
        var levels = FindObjectsOfType<Level>();
        var pos = this.GetLevelById(this.LastId).transform.position;

        this.targetCamSize = 1.0f;
        this.targetCamPos 
            = new Vector3(pos.x, pos.y, this.cam.transform.position.z);

        foreach (var level in levels)
        {
            level.SwitchToClose();
        }

        this.close = true;
        this.isCamSticky = true;

        this.map2.SetActive(true);
        this.map2Top.SetActive(true);

        this.map1.SetActive(false);
        this.map1Top.SetActive(false);

        this.topAlpha = 0.0f;
        this.topOffset = 250.0f;

        this.soundZoom.Play();
    }

    public void SwitchToCloseInstant(Vector3 pos)
    {
        this.targetCamSize = 1.0f;
        this.targetCamPos
            = new Vector3(pos.x, pos.y, this.cam.transform.position.z);

        this.cam.orthographicSize = 1.0f;
        this.cam.transform.position 
            = new Vector3(pos.x, pos.y, this.cam.transform.position.z);

        this.close = true;

        this.map2.SetActive(true);
        this.map2Top.SetActive(true);

        this.map1.SetActive(false);
        this.map1Top.SetActive(false);

        this.top.GetComponent<CanvasGroup>().alpha = 0;

        var rect = this.top.GetComponent<RectTransform>();
        rect.offsetMax = new Vector2(rect.offsetMax.x, 250.0f);

        this.topAlpha = 0.0f;
        this.topOffset = 250.0f;

        this.soundZoom.Play();
    }

    public void SwitchToFar()
    {
        this.targetCamSize = 5.0f;
        this.targetCamPos = new Vector3(0, 0, -1);

        var levels = FindObjectsOfType<Level>();
        foreach (var level in levels)
        {
            level.SwitchToFar();
        }
        
        this.close = false;
        this.isCamSticky = true;

        this.map2.SetActive(false);
        this.map2Top.SetActive(false);

        this.map1.SetActive(true);
        this.map1Top.SetActive(true);

        this.topAlpha = 1.0f;
        this.topOffset = 0.0f;

        this.soundZoom.Play();
    }

    public void ShowSMS(string message, VideoClip clip)
    {
        if (!this.isMOverlay)
        {
            this.telegramText.text = message;
            this.videoPlayer.GetComponent<VideoPlayer>().clip = clip;
            this.telegramSound.Play();
            this.mOverlay.SetActive(true);
            this.targetMOverlayAlpha = 1.0f;
            this.mOverlayAlpha = 0.15f;
            this.isMOverlay = true;
        }
    }

    public void ShowSMS(bool isWatched = true)
    {
        if (!this.isMOverlay)
        {
            this.isWatched = isWatched;
            var level = this.GetLevelById(this.LastId);

            this.telegramText.text = level.telegram;
            this.videoPlayer.GetComponent<VideoPlayer>().clip = level.video;
            this.telegramSound.Play();
            this.mOverlay.SetActive(true);
            this.targetMOverlayAlpha = 1.0f;
            this.mOverlayAlpha = 0.15f;
            this.isMOverlay = true;
        }
    }

    public void ShowInfo()
    {
        this.targetMOverlayAlpha = 0.0f;
        this.info.SetActive(true);
    }

    public void CloseInfo()
    {
        this.info.SetActive(false);
        if (this.isWatched)
        {
            this.GetLevelById(this.LastId).UnlockLevel();
        }
        else
        {
            this.ShowSMS(false);
        }
        
    }

    public void PlayVideo()
    {
        this.gui.SetActive(false);
        this.camera.GetComponent<Camera>().enabled = false;
        this.videoPlayer.GetComponent<Camera>().enabled = true;
        this.videoPlayer.GetComponent<VideoPlayer>().Play();

        if (this.isWatched)
        {
            this.videoLength = 10.0f;
        }
        else
        {
            this.videoLength = (float)this.videoPlayer.GetComponent<VideoPlayer>().clip.length - 1.0f;
        }

        this.isWatched = true;

        StartCoroutine(ShowVideoControls());
    }

    public void ExitVideo()
    {
        this.videoPlayer.GetComponent<Camera>().enabled = false;
        this.camera.GetComponent<Camera>().enabled = true;
        this.videoPlayer.GetComponent<VideoPlayer>().Stop();
        this.gui.SetActive(true);
        this.videoControls.SetActive(false);
        this.ShowInfo();
    }

    private IEnumerator ShowVideoControls()
    {
        yield return new WaitForSeconds(this.videoLength);
        this.videoControls.SetActive(true);
    }

    public void StartLevel()
    {
        this.camZoomSpeed = 0.05f;
        this.targetCamSize = 0.1f;
        SceneManager.LoadSceneAsync("game");
    }

    public void StartLastLevel()
    {
        var level = this.GetLevelById(this.LastId);

        GameStatic.id = level.id;
        GameStatic.state = level.state;
        GameStatic.typesCountTable = level.typesCountTable;
        GameStatic.pawChance = level.pawChance;
        GameStatic.video = level.video;
        GameStatic.quiz1 = level.quiz1;
        GameStatic.quiz1variants = level.quiz1variants;
        GameStatic.quiz2 = level.quiz2;
        GameStatic.quiz2variants = level.quiz2variants;
        GameStatic.quiz3 = level.quiz3;
        GameStatic.quiz3variants = level.quiz3variants;
        GameStatic.scoreCount = level.Score;
        GameStatic.pawsCount = level.Paws;

        SceneManager.LoadSceneAsync("game");
    }
}
