using UnityEngine;
using FMODUnity;

public class CharacterController3D : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;

    public EventReference walkEvent;  // Evento para pasos al caminar
    public EventReference jumpEvent;  // Evento para el salto
    public EventReference landEvent;  // Evento para el aterrizaje

    private FMOD.Studio.EventInstance walkInstance;
    private CharacterController controller;
    private Vector3 velocity;
    private float verticalLookRotation = 0f;
    private bool isWalking = false;
    private bool wasGrounded = true;

    public Transform cameraTransform;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Verificar que el evento no sea nulo
        if (walkEvent.IsNull)
        {
            Debug.LogError("walkEvent no está asignado en el Inspector.");
            return;
        }

        // Lock the cursor to the game screen
        Cursor.lockState = CursorLockMode.Locked;

        // Crear instancia del evento de caminar
        walkInstance = RuntimeManager.CreateInstance(walkEvent);
        if (walkInstance.isValid())
        {
            Debug.Log("Walk instance created successfully.");
        }
        else
        {
            Debug.LogError("No se pudo crear la instancia del evento de caminar.");
        }
    }

    private void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * speed * Time.deltaTime);

        // Handle walking sound
        HandleWalkingSound(move);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            PlayOneShotSound(jumpEvent);
        }

        // Handle landing sound
        if (!wasGrounded && controller.isGrounded)
        {
            PlayOneShotSound(landEvent);
        }

        wasGrounded = controller.isGrounded;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);
    }

    private void HandleWalkingSound(Vector3 move)
    {
        // Verificamos el estado de reproducción
        FMOD.Studio.PLAYBACK_STATE playbackState;
        walkInstance.getPlaybackState(out playbackState);

        // Umbral de velocidad mínimo para activar el sonido
        float moveThreshold = 0.05f; // Ajusta este valor según el comportamiento que desees

        // Comprobamos si estamos moviéndonos con un valor significativo
        bool isMoving = move.magnitude > moveThreshold;

        if (controller.isGrounded && isMoving) // Si el jugador está en el suelo y se está moviendo
        {
            if (playbackState != FMOD.Studio.PLAYBACK_STATE.PLAYING)  // Si no está reproduciendo
            {
                walkInstance.start();  // Empezamos el sonido si no está ya sonando
                isWalking = true;
                Debug.Log("Walking sound started.");
            }
        }
        else
        {
            if (isWalking)  // Solo detenemos el sonido si estaba sonando
            {
                walkInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); // Detener inmediatamente el sonido
                isWalking = false;
                Debug.Log("Walking sound stopped.");
            }
        }
    }


    private void PlayOneShotSound(EventReference soundEvent)
    {
        if (soundEvent.IsNull)
        {
            Debug.LogWarning("FMOD EventReference is null.");
            return;
        }

        RuntimeManager.PlayOneShot(soundEvent, transform.position);
    }

    private void OnDestroy()
    {
        if (walkInstance.isValid())
        {
            walkInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            walkInstance.release();
        }
    }
}
