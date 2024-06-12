using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class HUB : MonoBehaviour{
    public GameManager gM;
    public TextMeshProUGUI puntos;

    void Update(){
        puntos.text = gM.Puntaje.ToString();
    }
}
