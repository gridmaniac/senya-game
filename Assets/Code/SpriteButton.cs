using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Image image;
    private Sprite OffSprite;
    public Sprite OnSprite;
    public AudioSource soundClick;
    public int action;

    private void Start()
    {
        this.OffSprite = this.image.sprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.image.sprite = this.OnSprite;
        this.soundClick.Play();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.image.sprite = this.OffSprite;

        var controller = FindObjectOfType<MenuController>();
        switch (action)
        {
            case 0:
                controller.SwitchToFar();
                break;
            case 1:
                controller.SwitchToClose();
                break;
            case 2:
                controller.ShowSMS();
                break;
            case 3:
                controller.StartLastLevel();
                break;
        }
    }
}
