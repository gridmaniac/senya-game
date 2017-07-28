using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class QuizController : MonoBehaviour {
    public GameObject question;
    public GameObject correct;
    public GameObject fail;

    public Text q;
    public Text v1;
    public Text v2;
    public Text v3;
    public Text v4;

    public VideoPlayer vPlayer;
    public GameObject exitVideo;

    private int correctAnswer;

	public void CreateQuiz()
    {
        string quiz = "";
        string[] vars = new string[4];

        int quizNum = Random.Range(0, 3);
        switch (quizNum)
        {
            case 0:
                quiz = (string)GameStatic.quiz1.Clone();
                vars = (string[])GameStatic.quiz1variants.Clone();
                break;

            case 1:
                quiz = (string)GameStatic.quiz2.Clone();
                vars = (string[])GameStatic.quiz2variants.Clone();
                break;

            case 2:
                quiz = (string)GameStatic.quiz3.Clone();
                vars = (string[])GameStatic.quiz3variants.Clone();
                break;
        }

        correctAnswer = Random.Range(0, 4);

        string[] preVars = new string[4];
        preVars[correctAnswer] = vars[0];
        vars[0] = null;

        for (var i = 0; i < 4; i++)
        {
            if (i != correctAnswer)
            {
                for (var k = 0; k < 4; k++)
                {
                    if (vars[k] != null)
                    {
                        preVars[i] = vars[k];
                        vars[k] = null;
                        break;
                    }
                }
            }
        }

        q.text = quiz;
        v1.text = "1. " + preVars[0];
        v2.text = "2. " + preVars[1];
        v3.text = "3. " + preVars[2];
        v4.text = "4. " + preVars[3];

        question.SetActive(true);

        FindObjectOfType<GameController>().IsQuiz = true;
    }

    public void Select1()
    {
        if (correctAnswer == 0)
        {
            question.SetActive(false);
            correct.SetActive(true);
        }
        else
        {
            question.SetActive(false);
            fail.SetActive(true);
        }
    }

    public void Select2()
    {
        if (correctAnswer == 1)
        {
            question.SetActive(false);
            correct.SetActive(true);
        }
        else
        {
            question.SetActive(false);
            fail.SetActive(true);
        }
    }

    public void Select3()
    {
        if (correctAnswer == 2)
        {
            question.SetActive(false);
            correct.SetActive(true);
        }
        else
        {
            question.SetActive(false);
            fail.SetActive(true);
        }
    }

    public void Select4()
    {
        if (correctAnswer == 3)
        {
            question.SetActive(false);
            correct.SetActive(true);
        }
        else
        {
            question.SetActive(false);
            fail.SetActive(true);
        }
    }

    public void ExitFail()
    {
        this.fail.SetActive(false);
        this.gameObject.SetActive(false);
        FindObjectOfType<GameController>().IsQuiz = false;
    }

    public void ExitBonus()
    {
        this.correct.SetActive(false);
        this.gameObject.SetActive(false);
        FindObjectOfType<GameController>().IsQuiz = false;
        FindObjectOfType<Grid>().Kill9Balls();
    }

    public void PlayVideo()
    {
        this.vPlayer.clip = GameStatic.video;
        this.vPlayer.Play();
        FindObjectOfType<GameController>().IsQuiz = false;
        this.fail.SetActive(false);
        this.exitVideo.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
