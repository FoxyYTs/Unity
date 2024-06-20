using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour{

    public float Speed;
    public float jumpForce;
    public int saltoMax;
    public LayerMask Ground;
    public AudioClip sonidoSalto;

    private bool ViewRight = true;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private int saltoRestante;

    private Animator anime;

    // Start is called before the first frame update
    void Start(){
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anime = GetComponent<Animator>();
        saltoRestante = saltoMax;
    }

    // Update is called once per frame
    void Update(){  
        movimiento();
        salto();
    }

    bool suelo(){
        RaycastHit2D hit = Physics2D.BoxCast(bc.bounds.center, new Vector2(bc.bounds.size.x, bc.bounds.size.y), 0f, Vector2.down, .1f, Ground);
        return hit.collider != null;
    }

    void salto(){
        if(suelo()){
            saltoRestante = saltoMax;
            anime.SetBool("saltando", false);
        }else{
            anime.SetBool("saltando", true);
        }
        if (Input.GetKeyDown(KeyCode.Space) && saltoRestante > 0){
            
            saltoRestante--;
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            AudioManager.Instance.PlaySound(sonidoSalto);
        }
    }
    void movimiento(){
        float inputMovimiento = Input.GetAxis("Horizontal");
        if(inputMovimiento != 0){
            anime.SetBool("corriendo", true);
        }else{
            anime.SetBool("corriendo", false);
        }
        direccion(inputMovimiento);
        rb.velocity = new Vector2(inputMovimiento * Speed, rb.velocity.y);
    }

    void direccion(float inputMovimiento){
        if ((ViewRight && inputMovimiento < 0) || (!ViewRight && inputMovimiento > 0)){
            ViewRight = !ViewRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

}
