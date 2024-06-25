using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corazon : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Player")){
            bool vRecuperada = GameManager.Instance.sumaVida();
            if(vRecuperada){
                Destroy(this.gameObject);
            }
        }
    }
}
