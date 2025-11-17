using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OpeningSequenceController : MonoBehaviour
{
    [System.Serializable]
    public class Take
    {
        public string Name = "New Take";
        public GameObject TakeObject;
        public List<Graphic> uiElements; // Image, RawImage, TextMeshProUGUI, Button (graphic)
        public float fadeInTime = 1f;
        public float showDuration = 1f;
        public float fadeOutTime = 1f;
    }

    [Header("Lista de Takes da Abertura")]
    public List<Take> takes = new List<Take>();

    [Header("Cena Final")]
    public int sceneToLoad = 1;

    void Start()
    {
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {
        foreach (var take in takes)
        {
            take.TakeObject.SetActive(true);
            // FADE IN
            float t = 0f;
            while (t < take.fadeInTime)
            {
                t += Time.deltaTime;
                float a = Mathf.Clamp01(t / take.fadeInTime);

                foreach (var ui in take.uiElements)
                {
                    if (ui != null)
                        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, a);
                }
                yield return null;
            }
            // DURAÇÃO FIXA
            yield return new WaitForSeconds(take.showDuration);

            // FADE OUT
            t = 0f;
            while (t < take.fadeOutTime)
            {
                t += Time.deltaTime;
                float a = 1f - Mathf.Clamp01(t / take.fadeOutTime);

                foreach (var ui in take.uiElements)
                {
                    if (ui != null)
                        ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, a);
                }
                yield return null;
            }
            take.TakeObject.SetActive(false);
        }
        // CARREGA A PRÓXIMA CENA
        SceneManager.LoadScene(sceneToLoad);
    }
}
