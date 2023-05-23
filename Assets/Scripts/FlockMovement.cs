using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class FlockMovement : MonoBehaviour
{
    [Range(0.0f, 20.0f)]
    public float turningCoefficient = 10f;
    [Range(0.0f, 20.0f)]
    public float movementSpeed = 10;

    private bool inTween = false;

    private Vector3 cameraOffset;

    private Camera mainCamera;

    private FlockManager flockManager;

    public Transform leadingAnimal;

    public GameObject skyPlaceholder;
    public GameObject underwaterPlaceholder;

    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position += new Vector3(0, leadingAnimal.transform.position.y, 0);
        cameraOffset = mainCamera.transform.position - leadingAnimal.transform.position;
        flockManager = GetComponent<FlockManager>();
        DOTween.To(() => movementSpeed, x => movementSpeed = x, 10, 20);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !inTween)
        {
            if (flockManager.isInWater)
            {
                inTween = true;
                for (int i = 0; i < flockManager.flock.Count; i++)
                {
                    flockManager.flock[i].transform.DOLocalMove(flockManager.alignPositions[i].transform.localPosition, 1);
                }
                StartCoroutine(TweenChecker(1f));
            }
            else
            {
                inTween = true;
                movementSpeed = 45;
                DOTween.To(() => movementSpeed, x => movementSpeed = x, 10, 0.5f);
                StartCoroutine(TweenChecker(0.5f));
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            for (int i = 0; i < flockManager.flock.Count; i++)
            {
                flockManager.flock[i].transform.DOLocalMove(flockManager.spawnPositions[i].transform.localPosition, 1);
            }
            StartCoroutine(TweenChecker(1f));
        }

        leadingAnimal.GetComponent<AnimalManager>().speed = movementSpeed;

        Vector3 deltaDistance = movementSpeed * leadingAnimal.forward * Time.deltaTime;
        leadingAnimal.transform.position += deltaDistance;
        AdjustCamera();
        skyPlaceholder.transform.position += new Vector3(deltaDistance.x, 0, 0);
        underwaterPlaceholder.transform.position += new Vector3(deltaDistance.x, 0, 0);

        ConstrainPositions();
    }

    private void FixedUpdate()
    {
        SetRotation();
    }

    private void AdjustCamera()
    {
        float defaultY = (leadingAnimal.transform.position + cameraOffset).y;
        float modifiedY = (defaultY / (skyPlaceholder.transform.position.y - underwaterPlaceholder.transform.position.y * 0.9f) + 1) / 2;
        float easedY = EaseInOutSigmoid(underwaterPlaceholder.transform.position.y * 0.9f, skyPlaceholder.transform.position.y, modifiedY, 12.5f);
        mainCamera.transform.position = new Vector3(leadingAnimal.transform.position.x + cameraOffset.x, easedY, cameraOffset.z);
    }

    private void SetRotation()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float angle = leadingAnimal.rotation.eulerAngles.z > 270 ? 360 - leadingAnimal.rotation.eulerAngles.z : leadingAnimal.rotation.eulerAngles.z;
        if (verticalInput != 0 && Input.GetButton("Vertical"))
        {
            leadingAnimal.Rotate(0, 0, verticalInput * (1 - (Mathf.Abs(angle % 90)) / 90) * turningCoefficient);
        }
        else
        {

            if (leadingAnimal.rotation.eulerAngles.z <= 5 || leadingAnimal.rotation.eulerAngles.z >= 355)
            {
                leadingAnimal.rotation = Quaternion.identity;
            }
            else if (leadingAnimal.rotation.eulerAngles.z >= 270)
            {
                float before = leadingAnimal.eulerAngles.z;
                leadingAnimal.Rotate(0, 0, (1 - (Mathf.Abs(angle % 90)) / 90) * turningCoefficient);
            }
            else if (leadingAnimal.rotation.eulerAngles.z <= 90)
            {
                leadingAnimal.Rotate(0, 0, -(1 - (Mathf.Abs(angle % 90)) / 90) * turningCoefficient);
            }

        }
        if (leadingAnimal.rotation.eulerAngles.z < 270 && leadingAnimal.rotation.eulerAngles.z > 180)
        {
            leadingAnimal.rotation = Quaternion.Euler(0, 0, 271);
        }
        else if (leadingAnimal.rotation.eulerAngles.z > 90 && leadingAnimal.rotation.eulerAngles.z < 180)
        {
            leadingAnimal.rotation = Quaternion.Euler(0, 0, 89);
        }
    }

    private void ConstrainPositions()
    {
        if (leadingAnimal.transform.position.y > skyPlaceholder.transform.position.y + skyPlaceholder.transform.localScale.y / 2)
        {
            leadingAnimal.transform.position = new Vector3(leadingAnimal.transform.position.x, skyPlaceholder.transform.position.y + skyPlaceholder.transform.localScale.y / 2, leadingAnimal.transform.position.z);
        }
        else if (leadingAnimal.transform.position.y < underwaterPlaceholder.transform.position.y - underwaterPlaceholder.transform.localScale.y / 2)
        {
            leadingAnimal.transform.position = new Vector3(leadingAnimal.transform.position.x, underwaterPlaceholder.transform.position.y - skyPlaceholder.transform.localScale.y / 2, leadingAnimal.transform.position.z);
        }

        if (mainCamera.transform.position.y > skyPlaceholder.transform.position.y)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, skyPlaceholder.transform.position.y, mainCamera.transform.position.z);
        }
        else if (mainCamera.transform.position.y < underwaterPlaceholder.transform.position.y)
        {
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, underwaterPlaceholder.transform.position.y, mainCamera.transform.position.z);
        }
    }

    private IEnumerator TweenChecker(float duration)
    {
        yield return new WaitForSeconds(duration);
        inTween = false;
    }

    float EaseInOutSigmoid(float startValue, float endValue, float time, float steepness)
    {
        float difference = endValue - startValue;
        float sigmoid = 1 / (1 + Mathf.Exp(-steepness * (time - 0.5f)));
        float finalValue = sigmoid * difference + startValue;
        return finalValue;
    }
}
