using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int puntosTotal { get { return puntaje, }}
    private int puntaje;
    public void sumaPuntos(int puntos){
        puntaje += puntos;
        Debug.Log("Puntaje: " + puntaje);
    }
}
