using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleDistributor))]
public class ObstacleDistributorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObstacleDistributor obstacleDistributor = (ObstacleDistributor)target;

        if (GUILayout.Button("Distribuir Obstáculos"))
        {
            obstacleDistributor.DistributeObstacles();
        }
    }
}
