using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDistributor : MonoBehaviour
{
    [Header("Obst�culos")]
    public List<GameObject> obstacles; // Lista de prefabs de obst�culos que se asigna desde el editor
    public int obstacleCount = 10; // Cantidad de obst�culos a generar
    public int nObstacles = 1000;

    [Header("Grupo de Obst�culos")]
    public GameObject parentObject; // Objeto padre para organizar los obst�culos

    public void DistributeObstacles()
    {
        if (obstacles == null || obstacles.Count == 0)
        {
            Debug.LogError("Por favor, asigna al menos un prefab de obst�culo en la lista.");
            return;
        }

        // Obtener el plano autom�ticamente
        MeshRenderer planeRenderer = GetComponent<MeshRenderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Este script debe estar en un objeto con un componente MeshRenderer (el plano).");
            return;
        }

        // Calcular tama�o y posici�n del plano
        Vector3 planeSize = planeRenderer.bounds.size;
        Vector3 planePosition = planeRenderer.bounds.center;

        // Aseg�rate de que existe un objeto padre para organizar los obst�culos
        if (parentObject == null)
        {
            parentObject = new GameObject("ObstaclesParent");
        }

        // Instanciar los obst�culos
        for (int i = 0; i < nObstacles; i++)
        {
            // Elegir un prefab aleatorio de la lista
            GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Count)];

            // Instanciar el obst�culo
            GameObject obstacle = Instantiate(obstaclePrefab);

            // Asignar posici�n aleatoria dentro de los l�mites del plano
            float randomX = Random.Range(planePosition.x - planeSize.x / 2, planePosition.x + planeSize.x / 2);
            float randomZ = Random.Range(planePosition.z - planeSize.z / 2, planePosition.z + planeSize.z / 2);
            obstacle.transform.position = new Vector3(randomX, planePosition.y, randomZ);

            // Configurar como hijo del objeto padre
            obstacle.transform.parent = parentObject.transform;
        }
    }
}
