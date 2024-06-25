using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
        if(vidas == 0){
            SceneManager.LoadScene(0);
        }
        hub.DesactivarVidas(vidas);
    }

    public bool sumaVida(){
        if(vidas == 3){
            return false;
        } 
        hub.ActivarVidas(vidas);
        vidas++;
        return true;
    }
}
