using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo : MonoBehaviour{
    public float cdAtaque;
    private bool ataque = true;
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player")){
            if(!ataque)return;
            ataque = false;
            GameManager.Instance.restaVida();
            other.gameObject.GetComponent<CharacterController>().AplicarGolpe();

            Invoke("recargarAtaque", cdAtaque);
        }
    }

    private void recargarAtaque(){
        ataque = true;}
}
