/*
Purpose: Custom editor view for the Enemy and children classes to visualize pathing points in the scene view.
*/
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy), true)]
public class EnemiesPathEditor : Editor
{
    void OnSceneGUI()
    {
        Enemy enemy = (Enemy)target;    // Get the target enemy object

        for (int i = 0; i < enemy.pathingPoints.Length; i++)    // Loop through each pathing points if any
        {
            Vector3 worldPos = enemy.pathingPoints[i].position; // Get the world position of each points

            EditorGUI.BeginChangeCheck();   // Start checking for changes in the handle position
            Vector3 newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(enemy, "Move Path Point");    // Record the change for Unity's undo system (control/command + z)
                enemy.pathingPoints[i].position = newWorldPos;
            }

            Handles.SphereHandleCap(0, worldPos, Quaternion.identity, 0.5f, EventType.Repaint);

            if (i == 0)
            {
                Handles.Label(worldPos + Vector3.up * 0.5f, "Start");   // Label the first point 
            }

            if (i < enemy.pathingPoints.Length - 1)
            {
                Vector3 nextWorldPos = enemy.pathingPoints[i + 1].position;
                Vector3 direction = (nextWorldPos - worldPos).normalized;

                Handles.DrawLine(worldPos, nextWorldPos);
                Handles.ArrowHandleCap(0, worldPos, Quaternion.LookRotation(direction), 5.0f, EventType.Repaint);
            }
        }
    }
}
