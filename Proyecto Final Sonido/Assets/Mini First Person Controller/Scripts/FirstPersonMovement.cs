using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System;

public class CharacterController3D : MonoBehaviour {
    public float speed = 6.0f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 100f;

    public EventReference walkEvent;  // Evento para pasos al caminar


    private CharacterController controller;
    private Vector3 velocity;
    private float verticalLookRotation = 0f;
    private bool isWalking = false;
    private bool wasGrounded = true;


    public enum FootstepType{
        Desert = 0,
        Snow = 1,
        Forest = 2
    }

    [SerializeField]
    private string[] footstepEvents = {
        "event:/Pasos_Desierto",
        "event:/Pasos_Nieve",
        "event:/Pasos_Bosque"
    };

    private EventInstance[] footstepInstances; // Array de instancias de eventos
    FootstepType currentFootsteps;

    public Transform cameraTransform;

    private void Start() {
        controller = GetComponent<CharacterController>();

        footstepInstances = new EventInstance[footstepEvents.Length];

        for (int i = 0; i < footstepEvents.Length; i++) {
            footstepInstances[i] = RuntimeManager.CreateInstance(footstepEvents[i]);
        }
        currentFootsteps = FootstepType.Forest;

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the game screen
    }

    private void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * speed * Time.deltaTime);

        // Apply gravity
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        wasGrounded = controller.isGrounded;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalLookRotation, 0f, 0f);

        UpdateSound(move);
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
        // Liberar recursos de todas las instancias
        foreach (var instance in footstepInstances)
        {
            if (instance.isValid())
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
            }
        }
    }

    private void UpdateSound(Vector3 move)
    {
        // Inicia el evento de pasos si el jugador tiene una velocidad en X y está en el suelo
        if ((move.x != 0 || move.z != 0) && controller.isGrounded)
        {
            // Obtener el estado de reproducción del evento
            PLAYBACK_STATE playbackState;
            footstepInstances[(int)currentFootsteps].getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) {
                footstepInstances[(int)currentFootsteps].start();
            }
        }
        else { // De lo contrario, detener el evento de pasos
            Debug.Log(currentFootsteps);
            Debug.Log(footstepInstances);
            footstepInstances[(int)currentFootsteps].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Verifica la capa del objeto con el que colisiona
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Desert") && currentFootsteps != FootstepType.Desert)
        {
            ChangeFootstepType(FootstepType.Desert);
        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Snow") && currentFootsteps != FootstepType.Snow)
        {
            ChangeFootstepType(FootstepType.Snow);
        }
        else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Forest") && currentFootsteps != FootstepType.Forest)
        {
            ChangeFootstepType(FootstepType.Forest);
        }
    }

    private void ChangeFootstepType(FootstepType newFootstepType)
    {
        // Detener el sonido actual
        footstepInstances[(int)currentFootsteps].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        // Cambiar al nuevo tipo de terreno
        currentFootsteps = newFootstepType;

        Debug.Log($"Cambiado a pasos de: {currentFootsteps}");
    }
}
