using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{   
    public int valor = 1;
    public GameManager gM;
    public AudioManager aM;
    public AudioClip sonidoPuntos;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")){
            aM.PlaySound(sonidoPuntos);
            gM.sumaPuntos(valor);
            Destroy(this.gameObject);
        }
    }
}
