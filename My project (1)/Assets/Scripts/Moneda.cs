using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneda : MonoBehaviour
{   
    public int valor = 1;
    public AudioClip sonidoPuntos;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.CompareTag("Player")){
            AudioManager.Instance.PlaySound(sonidoPuntos);
            GameManager.Instance.sumaPuntos(valor);
            Destroy(this.gameObject);
        }
    }
}
