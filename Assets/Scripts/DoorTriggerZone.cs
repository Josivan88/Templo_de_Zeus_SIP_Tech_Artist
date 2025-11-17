using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorData
{
    public Transform door;
    public float startAngleY = 0f;
    public float endAngleY = 90f;
}

public class DoorTriggerZone : MonoBehaviour
{
    [Header("Portas Controladas")]
    public List<DoorData> doors = new List<DoorData>();
    public float openSpeed = 2f;

    [Header("Objetos a Ativar quando o jogador entrar")]
    public List<GameObject> objectsToEnable;

    [Header("Som de Abertura")]
    public AudioSource doorAudio;
    public AudioSource EffectAudio;
    public AudioSource Wow;
    public bool WowPlayed = false;

    [Header("Shader e Luz Controlados")]
    public Renderer targetRenderer;
    public string shaderParameter = "_EffectStrength"; // Nome genérico
    public float shaderLerpSpeed = 1f;

    public Light pointLight; // 🔹 Adicionada Point Light
    private float initialLightIntensity = 1f;

    private bool isOpening = false;
    private bool hasOpened = false;
    private List<Quaternion> initialRotations;

    [Header("Elementos extra")]
    public Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        //Desabilita objetos escondidos
        foreach (var obj in objectsToEnable)
            if (obj != null) obj.SetActive(false);

        // Armazena rotação inicial real de cada porta
        initialRotations = new List<Quaternion>();
        foreach (var doorData in doors)
        {
            if (doorData.door != null)
                initialRotations.Add(doorData.door.localRotation);
            else
                initialRotations.Add(Quaternion.identity);
        }

        // Define o valor inicial do parâmetro do shader
        if (targetRenderer != null && targetRenderer.material.HasProperty(shaderParameter))
        {
            targetRenderer.material.SetFloat(shaderParameter, 1f);
        }

        // 🔹 Guarda a intensidade inicial da Point Light
        if (pointLight != null)
            initialLightIntensity = pointLight.intensity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpening && !hasOpened && other.CompareTag("Player"))
        {
            // Ativa os objetos
            foreach (var obj in objectsToEnable)
                if (obj != null) obj.SetActive(true);

            StartCoroutine(OpenDoors());
        }
    }

    private IEnumerator OpenDoors()
    {
        isOpening = true;

        if (doorAudio != null)
        {
            doorAudio.Play();
        }
        if (EffectAudio != null)
        {
            EffectAudio.Play();
        }

        targetCamera.GetComponent<CameraEarthquakeShake>().StartEarthquakeShake(
            duration: 10f,
            posAmplitude: 0.25f,
            rotAmplitude: 5f,
            freq: 4f
        );

        float time = 0f;
        float shaderValue = 1f;

        while (time < 1f)
        {
            if (Wow != null && time>0.5f && !WowPlayed)
            {
                Wow.Play();
                WowPlayed = true;
            }

            time += Time.deltaTime * openSpeed;
            float t = Mathf.SmoothStep(0, 1, time);

            // Rotaciona cada porta
            for (int i = 0; i < doors.Count; i++)
            {
                DoorData data = doors[i];
                if (data.door == null) continue;

                float currentY = Mathf.Lerp(data.startAngleY, data.endAngleY, t);
                data.door.localRotation = initialRotations[i] * Quaternion.Euler(0, currentY - data.startAngleY, 0);
            }

            // 🔹 Atualiza shader
            if (targetRenderer != null && targetRenderer.material.HasProperty(shaderParameter))
            {
                shaderValue = Mathf.Lerp(1f, 0f, t);
                targetRenderer.material.SetFloat(shaderParameter, shaderValue);
            }

            // 🔹 Atualiza intensidade da luz
            if (pointLight != null)
            {
                pointLight.intensity = Mathf.Lerp(initialLightIntensity, 0f, t);
            }

            yield return null;
        }

        hasOpened = true;
        isOpening = false;
    }
}
