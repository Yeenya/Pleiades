using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlockManager : MonoBehaviour
{
    public GameObject animalPrefab;
    public GameObject leadingAnimal;
    public List<GameObject> flock = new List<GameObject>();
    public List<GameObject> spawnPositions= new List<GameObject>();
    public List<GameObject> alignPositions = new List<GameObject>();
    public FadeManager fadeManager;
    public EndManager endManager;

    private FlockMovement movement;
    public bool isInWater = false;
    public bool colliding = true;
    public bool isInvincible = false;

    public Vector3 flockCentre;

    private void Start()
    {
        movement = GetComponent<FlockMovement>();
        leadingAnimal = transform.GetChild(0).gameObject;
        movement.leadingAnimal = leadingAnimal.transform;
        flock.Add(leadingAnimal);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            AddAnimalToTheFlock();
        }
    }

    public void SolveCollisionEnter(Collider2D collision)
    {
        if (collision.CompareTag("Star"))
        {
            AddAnimalToTheFlock();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            if (movement.movementSpeed >= 25 || isInvincible)
            {
                Destroy(collision.gameObject);
            }
            else
            {
                Collision(collision);
            }
        }
        else if (collision.CompareTag("Environment"))
        {
            if (isInvincible) return;

            Collision(collision);
        }
        else if (collision.CompareTag("End"))
        {
            isInvincible = true;
            fadeManager.FadeOut();
            endManager.goodOrBad = true;
        }
    }

    public void SolveCollisionExit(Collider2D collision)
    {
        //isInvincible = false;
    }

    private void Collision(Collider2D collision)
    {
        if (!colliding) return;
        int flockCount = flock.Count;
        if (flockCount > 1)
        {
            GameObject lastBird = flock[flockCount - 1];
            flock.RemoveAt(flockCount - 1);

            Destroy(lastBird);
            if (collision.CompareTag("Obstacle")) Destroy(collision.gameObject);
            StartCoroutine(Invincibility());
        }
        else
        {
            print("Game Over");
            fadeManager.FadeOut();
            endManager.goodOrBad = false;
            Destroy(this.gameObject);
        }
    }

    private void AddAnimalToTheFlock()
    {
        movement.movementSpeed += 0.6f;
        GameObject newBird = Instantiate(animalPrefab, spawnPositions[flock.Count].transform.position, Quaternion.identity, transform);
        newBird.transform.position = new Vector3(newBird.transform.position.x, newBird.transform.position.y, 0);
        newBird.GetComponent<AnimalManager>().manager = this;
        newBird.GetComponent<AnimalManager>().movement = GetComponent<FlockMovement>();
        flock.Add(newBird);
        if (flock.Count == 7)
        {
            InitiateEnd();
        }
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        foreach (GameObject animal in flock)
        {
            animal.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(new Color(0.5f, 0.5f, 0.5f), 0.25f);
        }
        yield return new WaitForSeconds(3f);
        isInvincible = false;
        foreach (GameObject animal in flock)
        {
            animal.transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(Color.white, 0.5f);
        }
    }

    private void InitiateEnd()
    {
        GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>().SpawnEnd();
    }
}

