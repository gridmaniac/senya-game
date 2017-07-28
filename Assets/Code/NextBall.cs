using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextBall : MonoBehaviour {

    /// <summary>
    /// Компонент аниматор
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Компонент для рендеринга
    /// </summary>
    private Image image;

    /// <summary>
    /// Набор спрайтов
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// Инициализация
    /// </summary>
    private void Start()
    {
        this.image = GetComponent<Image>();
        this.animator = GetComponent<Animator>();
        this.animator.speed = 0;
    }
    
    /// <summary>
    /// Установка цвета и показ шара
    /// </summary>
    /// <param name="color">Цвет шара</param>
    public void Show(int color)
    {
        this.image.sprite = sprites[color];

        this.animator.speed = 1;
        this.animator.Play("ShowNextBall", 0);
    }

    /// <summary>
    /// Спрятать шар
    /// </summary>
    public void Hide()
    {
        this.animator.Play("HideNextBall", 0);
    }
}
