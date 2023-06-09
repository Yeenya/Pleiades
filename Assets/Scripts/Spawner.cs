using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField]
    protected List<GameObject> prefabs;

    [SerializeField]
    protected List<GameObject> obstacles;

    [SerializeField]
    protected GameObject playerReference;

    protected GameObject starPrefab;

    protected SpawnManager spawnManager;


    //public int indexOfLastSpawned;

    public float prefabWidth;
    public float prefabHeight;

    protected void SetSortingLayers()
    {
        foreach (GameObject prefab in prefabs)
        {
            prefab.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    public virtual void Spawn(bool spawnStar, bool spawnObstacle, bool forceZero)
    { }
    public virtual void Spawn(bool spawnStar, bool spawnObstacle)
    { }
    public virtual void Spawn(bool spawnStar)
    { }
    public virtual void Spawn()
    { }    

    public static IEnumerator RemoveEnvironmentPlaceholder(GameObject prefab)
    {
        yield return new WaitForSeconds(60f);
        Destroy(prefab);
    }
}
