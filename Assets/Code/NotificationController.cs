using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour {

    public Sprite nCombo2;
    public Sprite nCombo3;
    public Sprite nCombo4;
    public Sprite nCombo5;
    public Sprite nCombo6;

    public Sprite nPaws4;
    public Sprite nPaws3;
    public Sprite nPaws2;
    public Sprite nPaws1;
    public Sprite nPaws0;

    public Sprite nQuiz;

    public Image content;

    public AudioSource soundCombo;
    public AudioSource soundPaw;

    public QuizController quiz;

    private Animator animator;
    private bool isQuizStarted = false;

    private void Start()
    {
        this.animator = GetComponent<Animator>();

        switch (GameStatic.pawsCount)
        {
            case 1:
                this.content.sprite = nPaws4;
                break;
            case 2:
                this.content.sprite = nPaws3;
                break;
            case 3:
                this.content.sprite = nPaws2;
                break;
            case 4:
                this.content.sprite = nPaws1;
                break;
            case 5:
                this.content.sprite = nPaws0;
                break;
        }
    }

    private IEnumerator Notify(Sprite notification, AudioSource sound, bool isQuiz)
    {
        yield return new WaitForSeconds(0.5f);
        if (isQuiz)
        {
            this.isQuizStarted = true;
            this.content.sprite = nQuiz;
            this.animator.Play("NotifyStay", 0, 0);
        }
        else
        {
            if (notification != null)
                this.content.sprite = notification;
            if (sound != null)
                sound.Play();
            this.animator.Play("Notify", 0, 0);

            if (this.isQuizStarted)
                StartCoroutine(NotifyQuizDeffered());
        }
    }

    private IEnumerator NotifyQuizDeffered()
    {
        yield return new WaitForSeconds(1.0f);
        this.content.sprite = nQuiz;
        this.animator.Play("NotifyStay", 0, 0);
    }

    public void NotifyQuiz()
    {
        StartCoroutine(Notify(null, null, true));
    }

    public void NotifyCombo2()
    {
        StartCoroutine(Notify(this.nCombo2, this.soundCombo, false));
    }

    public void NotifyCombo3()
    {
        StartCoroutine(Notify(this.nCombo3, this.soundCombo, false));
    }

    public void NotifyCombo4()
    {
        StartCoroutine(Notify(this.nCombo4, this.soundCombo, false));
    }

    public void NotifyCombo5()
    {
        StartCoroutine(Notify(this.nCombo5, this.soundCombo, false));
    }

    public void NotifyCombo6()
    {
        StartCoroutine(Notify(this.nCombo6, this.soundCombo, false));
    }

    public void NotifyPaws4()
    {
        StartCoroutine(Notify(this.nPaws4, this.soundPaw, false));
    }

    public void NotifyPaws3()
    {
        StartCoroutine(Notify(this.nPaws3, this.soundPaw, false));
    }

    public void NotifyPaws2()
    {
        StartCoroutine(Notify(this.nPaws2, this.soundPaw, false));
    }

    public void NotifyPaws1()
    {
        StartCoroutine(Notify(this.nPaws1, this.soundPaw, false));
    }

    public void NotifyPaws0()
    {
        StartCoroutine(Notify(this.nPaws0, this.soundPaw, false));
    }

    public void StartQuiz()
    {
        if (this.isQuizStarted)
        {
            this.animator.Play("Notify", 0, 1);
            this.isQuizStarted = false;

            quiz.gameObject.SetActive(true);
            quiz.CreateQuiz();
        }
        
    }
}
