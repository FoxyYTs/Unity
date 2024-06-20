using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class HUB : MonoBehaviour{
    public TextMeshProUGUI puntos;
    public GameObject[] vidas;
 

    public void ActualizarPuntaje(int puntaje){
        puntos.text = puntaje.ToString();
    }

    public void DesactivarVidas(int indice){
        vidas[indice].SetActive(false);
    }

    public void ActivarVidas(int indice){
        vidas[indice].SetActive(true);
    }
}
