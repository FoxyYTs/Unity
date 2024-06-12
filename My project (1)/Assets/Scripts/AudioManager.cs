using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour{
    private AudioSource aS;
    // Start is called before the first frame update
    void Start(){
        aS = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip){
        aS.PlayOneShot(clip);
    }
}
