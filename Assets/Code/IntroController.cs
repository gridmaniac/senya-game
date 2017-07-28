using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    public bool dirtyVar = false;
    private bool currentVar = false;

    public Animator skipButton;

    private void Update()
    {
        if (this.dirtyVar != this.currentVar)
        {
            this.skipButton.Play("Pulsation", 0);
            this.currentVar = this.dirtyVar;
        }
    }

    public void Skip()
    {
        PlayerPrefs.SetInt("intro", 1);
        PlayerPrefs.SetString("1", "0&0&0");
        SceneManager.LoadSceneAsync("menu");
    }
}
