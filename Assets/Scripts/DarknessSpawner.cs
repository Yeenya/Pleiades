using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DarknessSpawner : Spawner
{

    void Start()
    {
        indexOfLastSpawned = 0;
        prefabWidth = prefabs[0].GetComponent<SpriteRenderer>().size.x;
        prefabHeight = prefabs[0].GetComponent<SpriteRenderer>().size.y;
        SetSortingLayers();
    }

    void Update()
    {
        
    }

    public override void Spawn()
    {
        GameObject newDarkness = Instantiate(prefabs[Random.Range(0, prefabs.Count)], new Vector3(indexOfLastSpawned * prefabWidth + 50, prefabHeight, 0), Quaternion.identity);
        StartCoroutine(RemoveEnvironmentPlaceholder(newDarkness));
        indexOfLastSpawned++;
    }

    /* Might be useful if we have a lot of sprites
    private void Refresh()
    {

        var folderPath = "Assets/EnvironmentAssets/Darkness";

        var prefabGUIDs = AssetDatabase.FindAssets("t:GameObject", new[] { folderPath });

        foreach (var prefabGUID in prefabGUIDs)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab != null && !darknessPrefabs.Contains(prefab))
            {
                darknessPrefabs.Add(prefab);
            }
        }
    }
    */
}