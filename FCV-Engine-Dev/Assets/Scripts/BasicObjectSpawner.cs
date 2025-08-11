using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class BasicObjectSpawner : EditorWindow
{
    string objectBaseName = "";
    int objectID = 1;
    GameObject objectToSpawn;
    float objectScale = 1f;
    float spawnRadius = 5f;
    bool is2DMode = true; // Default to 2D mode

    [MenuItem("Tools/Basic Object Spawner")]
    public static void ShowWindow()
    {
        GetWindow(typeof(BasicObjectSpawner));      //GetWindow is a method inherited from the EditorWindow class
    }

    private void OnGUI()
    {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);

        is2DMode = EditorGUILayout.Toggle("2D Mode", is2DMode);
        
        objectBaseName = EditorGUILayout.TextField("Base Name", objectBaseName);
        objectID = EditorGUILayout.IntField("Object ID", objectID);
        objectScale = EditorGUILayout.Slider("Object Scale", objectScale, 0.5f, 3f);
        spawnRadius = EditorGUILayout.FloatField("Spawn Radius", spawnRadius);
        objectToSpawn = EditorGUILayout.ObjectField("Prefab to Spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Spawn Object"))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if(objectToSpawn == null)
        {
            Debug.LogError("Error: Please assign an object to be spawned.");
            return;
        }
        if (objectBaseName == string.Empty) {
            Debug.LogError("Error: Please enter a base name for the object.");
            return;
        }

        Vector2 spawnCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = is2DMode ? new Vector3(spawnCircle.x, spawnCircle.y, 0) : new Vector3(spawnCircle.x, 0f, spawnCircle.y);
        GameObject newObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        newObject.transform.localScale = Vector3.one * objectScale;
        newObject.name = objectBaseName + objectID++;
    }
}
#endif
