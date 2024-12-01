using FMODUnity;
using UnityEngine;

public class BiomeFootsteps : MonoBehaviour
{
    public EventReference desertFootstepsEvent; // Evento para pasos en desierto
    public EventReference snowFootstepsEvent;   // Evento para pasos en nieve
    public EventReference forestFootstepsEvent; // Evento para pasos en jungla

    private FMOD.Studio.EventInstance currentFootsteps; // Instancia del evento actual

    // Método para cambiar de bioma
    public void ChangeBiome(int biome)
    {
        // Detener el evento anterior si está reproduciéndose
        if (currentFootsteps.isValid())
        {
            currentFootsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentFootsteps.release();
        }

        // Cambiar el evento de pasos según el bioma
        switch (biome)
        {
            case 0: // Desierto
                currentFootsteps = RuntimeManager.CreateInstance(desertFootstepsEvent);
                break;
            case 1: // Nieve
                currentFootsteps = RuntimeManager.CreateInstance(snowFootstepsEvent);
                break;
            case 2: // Bosque
                currentFootsteps = RuntimeManager.CreateInstance(forestFootstepsEvent);
                break;
            default:
                Debug.LogWarning("Bioma no reconocido.");
                return;
        }

        // Iniciar el nuevo evento
        if (currentFootsteps.isValid())
        {
            currentFootsteps.start();
        }
        else
        {
            Debug.LogError("No se pudo crear la instancia del evento para el bioma: " + biome);
        }
    }

    // Llamar este método al finalizar
    private void OnDestroy()
    {
        if (currentFootsteps.isValid())
        {
            currentFootsteps.release();
        }
    }
}