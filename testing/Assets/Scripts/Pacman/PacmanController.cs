using UnityEngine;
using TMPro;
// using UnityEngine.Tilemaps; // ¡Ya no es necesario para los pellets!

public class PacManController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector2 currentDirection = Vector2.left;
    private Vector2 nextDirection = Vector2.left;
    private Rigidbody2D rb;
    private Animator animator;

    public TextMeshProUGUI scoreText;
    private int score = 0;

    public int pelletPoints = 10;
    public int powerPelletPoints = 50;

    // Ya NO NECESITAS estas referencias de Tilemap si los pellets son GameObjects
    // public Tilemap Pallets;
    // public Tilemap PowerPallets;

    public LayerMask wallLayerMask; 
    public LayerMask pelletLayerMask;
    public LayerMask powerPelletLayerMask; 

    public float raycastOffset = 0.2f;
    public float raycastDistance = 0.6f;

    void Start(){
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>(); 
        if (rb == null) 
        {
            Debug.LogError("PacManController requiere un componente Rigidbody2D!");
        }

        UpdateScoreText();
    }

    void Update()
    {
        HandleInput();
        TryChangeDirection();
    }

    void FixedUpdate()
    {
        MovePacMan();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            nextDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            nextDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            nextDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            nextDirection = Vector2.right;
        }
    }

    void TryChangeDirection()
    {
        Vector2 centerRayOrigin = (Vector2)transform.position + nextDirection * 0.1f; 

        Vector2 perpendicularOffset = Vector2.zero;
        if (nextDirection == Vector2.up || nextDirection == Vector2.down)
        {
            perpendicularOffset = Vector2.right * raycastOffset;
        }
        else if (nextDirection == Vector2.left || nextDirection == Vector2.right)
        {
            perpendicularOffset = Vector2.up * raycastOffset;
        }

        Vector2 leftRayOrigin = centerRayOrigin - perpendicularOffset;
        Vector2 rightRayOrigin = centerRayOrigin + perpendicularOffset;

        RaycastHit2D hitCenter = Physics2D.Raycast(centerRayOrigin, nextDirection, raycastDistance, wallLayerMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(leftRayOrigin, nextDirection, raycastDistance, wallLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(rightRayOrigin, nextDirection, raycastDistance, wallLayerMask);
        
        Debug.DrawRay(centerRayOrigin, nextDirection * raycastDistance, (hitCenter.collider != null) ? Color.red : Color.green, 0.1f);
        Debug.DrawRay(leftRayOrigin, nextDirection * raycastDistance, (hitLeft.collider != null) ? Color.red : Color.green, 0.1f);
        Debug.DrawRay(rightRayOrigin, nextDirection * raycastDistance, (hitRight.collider != null) ? Color.red : Color.green, 0.1f);

        if (hitCenter.collider == null && hitLeft.collider == null && hitRight.collider == null) 
        {
            currentDirection = nextDirection;
        }
    }

    void MovePacMan()
    {
        rb.linearVelocity = currentDirection * moveSpeed;

        if (currentDirection == Vector2.up)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90); 
        }
        else if (currentDirection == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); 
        }
        else if (currentDirection == Vector2.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        else if (currentDirection == Vector2.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180); 
        }
    }

    // Mantén este método para depuración, si sigue colisionando con paredes.
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLISIÓN FÍSICA detectada con: " + collision.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // NO necesitamos WorldToCell ni SetTile(null) porque son GameObjects individuales
        // Solo necesitamos destruirlos
        
        // Verifica si el objeto colisionado está en la capa de Pellets normales
        if (((1 << other.gameObject.layer) & pelletLayerMask) != 0) 
        {
            AddScore(pelletPoints);
            Destroy(other.gameObject); // ¡Destruye el GameObject del pellet!
            Debug.Log("¡Pastilla normal comida! Puntuación: " + score);
        }
        // Verifica si el objeto colisionado está en la capa de PowerPellets
        else if (((1 << other.gameObject.layer) & powerPelletLayerMask) != 0) 
        {
            AddScore(powerPelletPoints);
            Destroy(other.gameObject); // ¡Destruye el GameObject del power pellet!
            Debug.Log("¡Pastilla de poder comida! Puntuación: " + score);
            // Aquí puedes añadir la lógica para el modo de poder (ej. activar corrutina en otro script)
        }
    }

    void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}