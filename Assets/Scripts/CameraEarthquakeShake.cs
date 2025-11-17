using UnityEngine;

public class CameraEarthquakeShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 1f;
    public float positionAmplitude = 0.2f;
    public float rotationAmplitude = 2f;
    public float frequency = 2f;

    public float shakeTimer = 0f;

    private Vector3 originalPos;
    private Quaternion originalRot;

    void Start()
    {
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            float normalizedTime = shakeTimer / shakeDuration; // 1 → 0
            float fade = Mathf.Pow(normalizedTime, 2); // fade-out suave (quadrático)

            // Position shake
            float noiseX = (Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f);
            float noiseY = (Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f);

            Vector3 posOffset = new Vector3(noiseX, noiseY, 0f) * positionAmplitude * fade;
            transform.localPosition = originalPos + posOffset;

            // Rotation shake
            float rotX = (Mathf.PerlinNoise(Time.time * frequency, 1f) * 2f - 1f) * rotationAmplitude * fade;
            float rotY = (Mathf.PerlinNoise(1f, Time.time * frequency) * 2f - 1f) * rotationAmplitude * fade;

            Quaternion rotOffset = Quaternion.Euler(rotX, rotY, 0f);
            transform.localRotation = originalRot * rotOffset;

            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0)
            {
                transform.localPosition = originalPos;
                transform.localRotation = originalRot;
            }
        }
    }

    /// <summary>
    /// Starts an earthquake-like camera shake.
    /// </summary>
    public void StartEarthquakeShake(float duration, float posAmplitude, float rotAmplitude, float freq)
    {
        shakeDuration = duration;
        positionAmplitude = posAmplitude;
        rotationAmplitude = rotAmplitude;
        frequency = freq;

        shakeTimer = duration;
    }
}
