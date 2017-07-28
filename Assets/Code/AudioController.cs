using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public bool dirtyVar = false;
    private bool currentVar = false;

    private void Update()
    {
        if (this.dirtyVar != this.currentVar)
        {
            GetComponent<AudioSource>().Play();
            this.currentVar = this.dirtyVar;
        }
    }
}
