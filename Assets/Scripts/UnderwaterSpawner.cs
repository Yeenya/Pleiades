using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnderwaterSpawner : Spawner
{

    protected ushort lastHeight;

    void Start()
    {
        indexOfLastSpawned = 0;
        prefabWidth = prefabs[0].GetComponent<SpriteRenderer>().size.x * 0.97f;
        prefabHeight = prefabs[0].GetComponent<SpriteRenderer>().size.y;
        SetSortingLayers();
        starPrefab = GetComponent<SpawnManager>().starPrefab;
    }

    public override void Spawn(bool spawnStar, bool spawnObstacle)
    {
        List<GameObject> suitablePrefabs = prefabs.Where(prefab => prefab.name.StartsWith(lastHeight.ToString())).ToList();
        GameObject prefabToSpawn = suitablePrefabs[Random.Range(0, suitablePrefabs.Count)];
        lastHeight = ushort.Parse(prefabToSpawn.name.Substring(prefabToSpawn.name.Length - 1));
        GameObject newUnderwater = Instantiate(prefabToSpawn, new Vector3(indexOfLastSpawned * prefabWidth + 50, -prefabHeight / 2, 0), Quaternion.identity);
        if (spawnStar)
        {
            Instantiate(starPrefab, newUnderwater.transform.GetChild(0));
        }
        if (spawnObstacle)
        {
            Instantiate(obstacles[Random.Range(0, obstacles.Count)], new Vector3(indexOfLastSpawned * prefabWidth + 50, -prefabHeight / 2, 0), Quaternion.identity, newUnderwater.transform);
        }

        StartCoroutine(RemoveEnvironmentPlaceholder(newUnderwater));
        indexOfLastSpawned++;
    }
}
