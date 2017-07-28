using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    /// <summary>
    /// Координаты ячейки
    /// </summary>
    private Vector2 coord;
    public Vector2 GetCoord
    {
        get { return this.coord; }
    }

    /// <summary>
    /// Позиция ячейки в мировом пространстве
    /// </summary>
    public Vector2 pos
    {
        get { return this.transform.position; }
    }

    /// <summary>
    /// Шар в ячейке
    /// </summary>
    private Ball ball;
    public Ball Ball
    {
        set { this.ball = value; }
        get { return this.ball; }
    }

    /// <summary>
    /// Признак выбранной ячейки
    /// </summary>
    private bool isSelected = false;

    /// <summary>
    /// Ссылка на сетку
    /// </summary>
    private Grid grid {
        get { return FindObjectOfType<Grid>(); }
    }

    private SpriteRenderer activeScore;
    public SpriteRenderer TwoScore;
    public SpriteRenderer FiveScore;
    private float scoreTime = 0.0f;


    private bool isShowingScore;

    public int Score = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.TwinkleScore();
        this.OperateCells();
    }

    private void TwinkleScore()
    {
        if (this.isShowingScore)
        {
            if (this.scoreTime < 2 * Mathf.PI)
            {
                var color = this.activeScore.color;
                color.a = Mathf.Sin(this.scoreTime) * 0.7f;
                this.activeScore.color = color;

                this.scoreTime += Time.deltaTime * 2.0f;
            }
            else
            {
                var color = this.activeScore.color;
                color.a = 0.0f;
                this.activeScore.color = color;

                this.scoreTime = 0.0f;
                this.isShowingScore = false;
            }
        }
    }

    private void OperateCells()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool isThis = this.GetComponent<Collider2D>().bounds.Contains(mousePosition);

            if (isThis && this.isSelected && ball != null)
            {
                ball.AnimIdle();
                this.isSelected = false;
                this.grid.UnselectCell();
            }
            else if (isThis && ball != null)
            {
                if (!FindObjectOfType<GameController>().IsPaused
                    && !FindObjectOfType<GameController>().IsGameOver
                    && !FindObjectOfType<GameController>().IsQuiz)
                {
                    this.ball.AnimShake();
                    this.isSelected = true;
                    this.grid.SelectCell(this);
                }
            }
            else if (ball != null)
            {
                ball.AnimIdle();
                this.isSelected = false;
            }
            else if (isThis)
            {
                if (!FindObjectOfType<GameController>().IsPaused
                    && !FindObjectOfType<GameController>().IsGameOver
                    && !FindObjectOfType<GameController>().IsQuiz)
                {
                    this.grid.TryTurn(this);
                }
            }
        }
    }

    public void SetCoord(Vector2 coord)
    {
        this.coord = coord;
    }

    public void ShowScore()
    {
        switch (this.Score)
        {
            case 2:
                this.activeScore = this.TwoScore;
                break;
            case 5:
                this.activeScore = this.FiveScore;
                break;
        }

        this.scoreTime = 0.0f;
        this.isShowingScore = true;
        this.Score = 0;
    }
}
