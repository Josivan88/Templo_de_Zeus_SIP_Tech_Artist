using UnityEngine;
using System.Collections.Generic;

public class BirdFlockController : MonoBehaviour
{
    [System.Serializable]
    public class Bird
    {
        public Transform transform;
        [HideInInspector] public Vector3 direction;
        [HideInInspector] public float currentSpeed;
        [HideInInspector] public float targetSpeed;
        [HideInInspector] public float timeSinceLastDirectionChange;
    }

    public List<Bird> birds = new List<Bird>();

    [Header("Flight Area (relative to this object's position)")]
    public Vector3 minRange = new Vector3(-5f, -2f, -5f);
    public Vector3 maxRange = new Vector3(5f, 2f, 5f);

    [Header("Timing")]
    [Tooltip("Time in seconds between adjusting the flight direction")]
    public float directionChangeInterval = 3f;
    [Tooltip("How fast the bird smooths its speed to target speed")]
    public float speedLerpTime = 1f;

    [Header("Speed")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    [Header("Rotation")]
    [Tooltip("How fast the bird rotates to face its movement direction")]
    public float rotationLerpTime = 0.3f;

    public enum Axis { X, Y, Z }
    [System.Serializable]
    public struct ForwardAxis
    {
        public Axis axis;
        public bool isNegative;
    }
    public ForwardAxis forwardAxis = new ForwardAxis { axis = Axis.Z, isNegative = false };

    void Start()
    {
        foreach (var bird in birds)
        {
            InitializeBird(bird);
        }
    }

    void Update()
    {
        foreach (var bird in birds)
        {
            bird.timeSinceLastDirectionChange += Time.deltaTime;

            // Smooth speed interpolation
            bird.currentSpeed = Mathf.Lerp(bird.currentSpeed, bird.targetSpeed, Time.deltaTime / speedLerpTime);

            // Update direction periodically
            if (bird.timeSinceLastDirectionChange >= directionChangeInterval)
            {
                Vector3 randomDir = Random.insideUnitSphere;
                bird.direction = (bird.direction + randomDir * 0.3f).normalized;
                bird.timeSinceLastDirectionChange = 0f;
                bird.targetSpeed = Random.Range(minSpeed, maxSpeed);
            }

            // Constrain to flight area
            Vector3 localPos = bird.transform.position - transform.position;
            if (localPos.x < minRange.x || localPos.x > maxRange.x ||
                localPos.y < minRange.y || localPos.y > maxRange.y ||
                localPos.z < minRange.z || localPos.z > maxRange.z)
            {
                Vector3 toCenter = (transform.position - bird.transform.position).normalized;
                bird.direction = Vector3.Lerp(bird.direction, toCenter, Time.deltaTime * 2f).normalized;
            }

            // Move forward
            bird.transform.position += bird.direction * bird.currentSpeed * Time.deltaTime;

            // Compute flat direction for rotation
            Vector3 lookDir = new Vector3(bird.direction.x, 0f, bird.direction.z);
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
                bird.transform.rotation = Quaternion.Slerp(
                    bird.transform.rotation,
                    AdjustRotationForForwardAxis(targetRot),
                    Time.deltaTime / rotationLerpTime
                );
            }
        }
    }

    void InitializeBird(Bird bird)
    {
        bird.direction = Random.onUnitSphere.normalized;
        bird.currentSpeed = Random.Range(minSpeed, maxSpeed);
        bird.targetSpeed = bird.currentSpeed;
        bird.timeSinceLastDirectionChange = 0f;
    }

    Quaternion AdjustRotationForForwardAxis(Quaternion rotation)
    {
        switch (forwardAxis.axis)
        {
            case Axis.X:
                return rotation * Quaternion.Euler(0, forwardAxis.isNegative ? 90 : -90, 0);
            case Axis.Y:
                return rotation * Quaternion.Euler(forwardAxis.isNegative ? -90 : 90, 0, 0);
            case Axis.Z:
            default:
                return rotation * Quaternion.Euler(0, forwardAxis.isNegative ? 180 : 0, 0);
        }
    }
}
