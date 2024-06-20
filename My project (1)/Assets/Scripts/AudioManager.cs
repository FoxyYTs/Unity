using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class AudioManager : MonoBehaviour{
    public static AudioManager Instance {get; private set;}
    private AudioSource aS;

    private void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Debug.Log("Instance already exists, destroying object!");
        }
    }
    // Start is called before the first frame update
    void Start(){
        aS = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip){
        aS.PlayOneShot(clip);
    }
}
