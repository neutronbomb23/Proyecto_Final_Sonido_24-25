using FMODUnity;
using UnityEngine;

public class BiomeSoundManager : MonoBehaviour
{
    [SerializeField] private string snowSnapshot = "snapshot:/NieveSnapshot";
    [SerializeField] private string desertSnapshot = "snapshot:/DesiertoSnapshot";
    [SerializeField] private string forestSnapshot = "snapshot:/BosqueSnapshot";
    private FMOD.Studio.EventInstance activeSnapshot;
    private FMOD.Studio.EventInstance stepsEvent;

    private void Start()
    {
        stepsEvent = RuntimeManager.CreateInstance("event:/Steps");
    }

    public void SetBiome(int biomeType)
    {
        // Cambia el Snapshot
        if (activeSnapshot.isValid())
        {
            activeSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        switch (biomeType)
        {
            case 0: // Nieve
                activeSnapshot = RuntimeManager.CreateInstance(snowSnapshot);
                break;
            case 1: // Desierto
                activeSnapshot = RuntimeManager.CreateInstance(desertSnapshot);
                break;
            case 2: // Bosque
                activeSnapshot = RuntimeManager.CreateInstance(forestSnapshot);
                break;
        }

        activeSnapshot.start();

        // También cambiamos el parámetro del bioma para los pasos
        stepsEvent.setParameterByName("Bioma", biomeType);
        stepsEvent.start();
    }
}
