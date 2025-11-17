using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.GlobalIllumination;

public class TriggerActivator : MonoBehaviour
{
    [Header("Player Tag")]
    [Tooltip("Tag do jogador que ativa o trigger.")]
    public string playerTag = "Player";

    [Header("Sons Temporizados")]
    public bool InsideTemple;
    public List<AudioSource> TimeAudios;
    public List<float> TimeToPlayAudios;
    public List<bool> AudiosPlayed;
    public float EnterTime;
    public float ElapsedTime;

    [Header("Evento de demonstração do card")]
    public AudioSource InitialZeusVoice;
    public bool isShowing = false;
    public GameObject CardToShow;
    public float CardShowDuration=10f;
    public Camera targetCamera;
    public Light revelationLight;
    private float initialLightIntensity = 1f;
    public List<Renderer> targetRenderer;
    public List<string> shaderParameter;
    public List<float> InitialValue;
    public List<float> FinalValue;
    public float shaderLerpSpeed = 1f;
    public List<GameObject> EffectsToHide;
    public List<GameObject> ZeusEffectsToHide;

    [Header("Objetos a Ativar quando o jogador entrar")]
    public List<GameObject> objectsToEnable;

    [Header("Objetos a Desativar quando o jogador entrar")]
    public List<GameObject> objectsToDisable;


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

        // Define o valor inicial do parâmetro do shader
        for (int i = 0; i < targetRenderer.Count; i++)
        {
            if (targetRenderer[i] != null && targetRenderer[i].material.HasProperty(shaderParameter[i]))
            {
                InitialValue[i] = targetRenderer[i].material.GetFloat(shaderParameter[i]);
            }
        }

        // 🔹 Guarda a intensidade inicial da Point Light
        if (revelationLight != null)
            initialLightIntensity = revelationLight.intensity;
    }

    void Update()
    {
        if (InsideTemple)
        {
            ElapsedTime = Time.time - EnterTime;

            for (int i = 0; i < TimeAudios.Count; i++)
            {
                if (ElapsedTime >= TimeToPlayAudios[i] && TimeAudios[i] != null && !AudiosPlayed[i])
                {
                    TimeAudios[i].Play();
                    AudiosPlayed[i] = true;
                }
            }

            if (ElapsedTime > InitialZeusVoice.clip.length && !isShowing)
            {
                StartCoroutine(ShowDivineCard());
                isShowing = true;
            }
        }
    }

    private IEnumerator ShowDivineCard()
    {
        CardToShow.SetActive(true);

        targetCamera.GetComponent<CameraEarthquakeShake>().StartEarthquakeShake(
            duration: 12f,
            posAmplitude: 0.25f,
            rotAmplitude: 5f,
            freq: 4f
        );

        float time = 0f;
        float shaderValue = 1f;

        while (time < 1f)
        {
            time += Time.deltaTime/CardShowDuration;
            float t = Mathf.SmoothStep(0, 1, time);

            // Atualiza shaders
            for (int i = 0; i < targetRenderer.Count; i++)
            {
                if (targetRenderer != null && targetRenderer[i].material.HasProperty(shaderParameter[i]))
                {
                    shaderValue = Mathf.Lerp(InitialValue[i], FinalValue[i], t);
                    targetRenderer[i].material.SetFloat(shaderParameter[i], shaderValue);
                }
            }

            // Atualiza intensidade da luz
            if (revelationLight != null)
            {
                revelationLight.intensity = Mathf.Lerp(initialLightIntensity, 0f, t);
            }

            yield return null;
        }

        foreach (var obj in EffectsToHide)
            if (obj != null) obj.SetActive(false);

        yield return new WaitForSeconds(TimeAudios[2].clip.length + 1f);

        foreach (var obj in ZeusEffectsToHide)
            if (obj != null) obj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        EnterTime = Time.time;
        InsideTemple = true;
        if (other.CompareTag(playerTag))
        {
            // Ativa os objetos da primeira lista
            foreach (var obj in objectsToEnable)
                if (obj != null) obj.SetActive(true);

            // Desativa os objetos da segunda lista
            foreach (var obj in objectsToDisable)
                if (obj != null) obj.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // Faz o inverso ao sair do trigger
            foreach (var obj in objectsToEnable)
                if (obj != null) obj.SetActive(false);

            foreach (var obj in objectsToDisable)
                if (obj != null) obj.SetActive(true);
        }
    }
}
