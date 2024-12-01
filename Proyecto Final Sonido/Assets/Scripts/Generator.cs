using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDistributor : MonoBehaviour
{
    [Header("Obstáculos")]
    public List<GameObject> obstacles; // Lista de prefabs de obstáculos que se asigna desde el editor
    public int obstacleCount = 10; // Cantidad de obstáculos a generar
    public int nObstacles = 1000;

    [Header("Grupo de Obstáculos")]
    public GameObject parentObject; // Objeto padre para organizar los obstáculos

    public void DistributeObstacles()
    {
        if (obstacles == null || obstacles.Count == 0)
        {
            Debug.LogError("Por favor, asigna al menos un prefab de obstáculo en la lista.");
            return;
        }

        // Obtener el plano automáticamente
        MeshRenderer planeRenderer = GetComponent<MeshRenderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Este script debe estar en un objeto con un componente MeshRenderer (el plano).");
            return;
        }

        // Calcular tamaño y posición del plano
        Vector3 planeSize = planeRenderer.bounds.size;
        Vector3 planePosition = planeRenderer.bounds.center;

        // Asegúrate de que existe un objeto padre para organizar los obstáculos
        if (parentObject == null)
        {
            parentObject = new GameObject("ObstaclesParent");
        }

        // Instanciar los obstáculos
        for (int i = 0; i < nObstacles; i++)
        {
            // Elegir un prefab aleatorio de la lista
            GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Count)];

            // Instanciar el obstáculo
            GameObject obstacle = Instantiate(obstaclePrefab);

            // Asignar posición aleatoria dentro de los límites del plano
            float randomX = Random.Range(planePosition.x - planeSize.x / 2, planePosition.x + planeSize.x / 2);
            float randomZ = Random.Range(planePosition.z - planeSize.z / 2, planePosition.z + planeSize.z / 2);
            obstacle.transform.position = new Vector3(randomX, planePosition.y, randomZ);

            // Configurar como hijo del objeto padre
            obstacle.transform.parent = parentObject.transform;
        }
    }
}
