using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance; //no singleton needed

    public AudioSource audioSource;

    private void Awake() { instance = this; }

    public void PlaySound(AudioClip clip) {

        audioSource.PlayOneShot(clip);
        //PlayOneShot does not work properly on scene changes, but since the TitleScreen is always there
        //This is all the code we need

        //To reference this object just use the static instance reference
    }
    
}
