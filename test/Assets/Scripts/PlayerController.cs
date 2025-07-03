using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 100.0f; // Ahora afectará tanto al player como a la cámara
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private CharacterController controller;
    private Vector3 velocity;

    // **NUEVAS/RESTAURADAS VARIABLES PARA LA CÁMARA**
    private Camera playerCamera; // Referencia a la cámara del jugador
    private float xRotation = 0f; // Rotación vertical de la cámara

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        // **RESTAURAR: Obtener la referencia a la cámara hija**
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Update()
    {
        // -- Movimiento del Jugador --
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // -- Gravedad --
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // **NUEVA/RESTAURADA LÓGICA DE CÁMARA**
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Rotación vertical de la cámara (arriba/abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limita la rotación para evitar volteos
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del jugador (izquierda/derecha)
        transform.Rotate(Vector3.up * mouseX);
    }
}