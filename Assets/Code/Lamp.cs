using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lamp : MonoBehaviour {

    private Image image;

    private bool isFlickering;
    private bool isFadingOut;
    private bool isFadingIn;

    private int digit = 0;

    public Sprite[] sprites;

	// Use this for initialization
	void Start () {
        this.image = GetComponent<Image>();
        this.isFlickering = true;
    }

    // Update is called once per frame
    void Update() {
        var color = this.image.color;

        if (this.isFlickering)
        {
            color.a = Mathf.Lerp(color.a, Random.Range(0.7f, 1.0f), Time.deltaTime * 25.0f);
            this.image.color = color;
        }

        if (this.isFadingOut)
        {
            color.a = Mathf.Lerp(color.a, 0.4f, Time.deltaTime * 4.0f);
            this.image.color = color;

            if (color.a < 0.5f)
            {
                this.image.sprite = sprites[digit];
                this.isFadingOut = false;
                this.isFadingIn = true;
                
            }
        }

        if (this.isFadingIn)
        {
            color.a = Mathf.Lerp(color.a, 1.0f, Time.deltaTime * 4.0f);
            this.image.color = color;

            if (color.a > 0.9f)
            {
                this.image.sprite = sprites[digit];
                this.isFadingIn = false;
                this.isFlickering = true;
            }
        }
    }

    public void Set(int digit)
    {
        if (this.digit != digit)
        {
            this.digit = digit;
            this.isFlickering = false;
            this.isFadingOut = true;
        }
    }
}
