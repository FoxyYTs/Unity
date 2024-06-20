using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{   

    public static GameManager Instance { get; private set; }

    public HUB hub;

    public int Puntaje { get { return puntaje; } }

    private int vidas = 3;

    void Awake(){
        if(Instance == null){
            Instance = this;
        } else {
            Debug.Log("Instance already exists, destroying object!");
        }
    }

    private int puntaje;

    public void sumaPuntos(int puntos){
        puntaje += puntos;
        hub.ActualizarPuntaje(puntaje);
    }
    
    public void restaVida(){
        vidas--;
        hub.DesactivarVidas(vidas);
    }
}
