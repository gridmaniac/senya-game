using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    /// <summary>
    /// Набор спрайтов
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// Компонент аниматор
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Звук качания
    /// </summary>
    private AudioSource sound;

    /// <summary>
    /// Компонент рендеринга
    /// </summary>
    private SpriteRenderer spriteRenderer
    {
        get { return GetComponent<SpriteRenderer>(); }

    }
    /// <summary>
    /// Конечная позиция
    /// </summary>
    private Vector2 targetPosition;

    /// <summary>
    /// Минимальное отклонение от конечной позиции
    /// </summary>
    private float bias = 0.02f;

    /// <summary>
    /// Признак удаленности от конечной позиции
    /// </summary>
    public bool IsDistant
    {
        get
        {
            return (Vector2.Distance(this.pos, 
                this.targetPosition) > this.bias);
        }
        
    }

    /// <summary>
    /// Цвет шара
    /// </summary>
    private int color;
    public int Color
    {
        get
        {
            return this.color;
        }
    }

    /// <summary>
    /// Признак инициализации
    /// </summary>
    private bool isInitialized;
    public bool IsInitialized
    {
        get
        {
            return this.isInitialized;
        }
    }

    /// <summary>
    /// Глобальная позиция шара
    /// </summary>
    public Vector3 pos
    {
        get { return this.transform.parent.position; }
        set { this.transform.parent.position = value; }
    }
   
    /// <summary>
    /// Время интерполяции
    /// </summary>
    private float t;

    /// <summary>
    /// Ускорение интерполяции
    /// </summary>
    private float acceleration;

    /// <summary>
    /// Тень шарика
    /// </summary>
    public GameObject Shadow;

    void Start () {
        this.animator = GetComponent<Animator>();
        this.sound = GetComponent<AudioSource>();

        var gridSize = FindObjectOfType<Grid>().GetComponent<SpriteRenderer>().bounds.size.x;
        var scale = gridSize / 11.7f;
        this.transform.parent.localScale = new Vector3(scale, scale, 1);
	}

    public void SetTarget(Vector2 target)
    {
        this.t = .0f;
        this.acceleration = 0.0f;
        this.targetPosition = target;
        this.Z(3);
    }

    public void Move(float s, float a)
    {
        this.t += s + this.acceleration;
        this.acceleration += a;
        this.pos = Vector2.Lerp(this.pos, this.targetPosition, this.t);
    }

    private void Bounce()
    {
        this.sound.Play();
    }


    public void AnimDrop()
    {
        this.animator.Play("BallDrop", 0);
    }

    public void AnimShake()
    {
        this.animator.Play("BallShake", 0);
        this.Z(3);
        InvokeRepeating("Bounce", 0, 0.4f);
    }

    public void AnimBounce()
    {
        this.animator.Play("BallBounce", 0);
        this.Z(3);
    }

    public void AnimIdle()
    {
        this.animator.CrossFade("BallIdle", 0.3f, 0);
        this.Z(2);
        CancelInvoke("Bounce");
    }

    public void Z(int o)
    {
        this.spriteRenderer.sortingOrder = o;
    }

    public void Die(int variant)
    {
        switch (variant)
        {
            case 1:
                this.animator.Play("BallDieRight", 0);
                break;
            case 2:
                this.animator.Play("BallDieYRight", 0);
                break;
            case 3:
                this.animator.Play("BallDieYYRight", 0);
                break;
            case 4:
                this.animator.Play("BallDieLeft", 0);
                break;
            case 5:
                this.animator.Play("BallDieYLeft", 0);
                break;
            case 6:
                this.animator.Play("BallDieYYLeft", 0);
                break;
        }

        this.Shadow.SetActive(true);
        this.Z(3);

        StartCoroutine(Destroy());
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(this.transform.parent.gameObject);
    }

    public void Init(Vector2 origin, int color)
    {
        this.t = .0f;
        this.acceleration = .0f;
        this.targetPosition = origin;
        this.color = color;
        this.spriteRenderer.sprite = this.sprites[this.color];

        this.isInitialized = true;
    }
}
