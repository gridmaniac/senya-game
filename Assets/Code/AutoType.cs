using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoType : MonoBehaviour {

    public string text = "";
    public bool dirtyVar = false;
    public float delay = 0.1f;
    public Text output;

    private bool currentVar = false;

    // Update is called once per frame
    void Update () {
		if (this.dirtyVar != this.currentVar)
        {
            InvokeRepeating("AddLetter", 0.0f, this.delay);
            this.currentVar = this.dirtyVar;
        }
	}

    void AddLetter()
    {
        if (this.text.Length > 0 && this.output != null)
        {
            string t = this.output.text;
            t += this.text[0];
            this.text = this.text.Remove(0, 1);
            this.output.text = t;
        }
        
    }
}
