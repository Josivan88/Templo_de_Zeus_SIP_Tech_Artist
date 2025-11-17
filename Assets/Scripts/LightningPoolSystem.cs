using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningPoolSystem : MonoBehaviour
{
    public enum AxisAlignment { XPlus, XMinus, YPlus, YMinus, ZPlus, ZMinus }

    [Header("Pool Settings")]
    public GameObject lightningPrefab;
    public int poolSize = 10;
    public float lightningDuration = 1f;
    public float spawnInterval = 0.5f;
    public AxisAlignment alignmentAxis = AxisAlignment.ZPlus;

    [Header("Raycast / Random")]
    public float rayDistance = 30f;
    public LayerMask collisionMask = ~0;
    public bool showDebugLines = true;

    [Header("Curvature Control")]
    [Range(0f, 0.3f)] public float midOffsetFactor = 0.1f;

    private List<GameObject> pool = new List<GameObject>();
    private float nextSpawnTime;

    void Start()
    {
        if (lightningPrefab == null)
        {
            Debug.LogError("Lightning prefab not assigned.");
            enabled = false;
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(lightningPrefab, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnLightning();
            nextSpawnTime = Time.time + spawnInterval + Random.Range(-0.2f, 0.2f);
        }
    }

    void SpawnLightning()
    {
        GameObject lightning = GetAvailable();
        if (lightning == null) return;

        Vector3 origin = transform.position;
        Vector3 dir = Random.onUnitSphere;
        if (dir.y < -0.4f) dir.y = -dir.y;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, rayDistance, collisionMask))
        {
            Vector3 target = hit.point;

            if (showDebugLines)
                Debug.DrawLine(origin, target, Color.cyan, lightningDuration);

            SetupLightning(lightning, origin, target);
            StartCoroutine(Lifecycle(lightning));
        }
    }

    void SetupLightning(GameObject lightning, Vector3 startPos, Vector3 endPos)
    {
        Transform tStart = lightning.transform.Find("Start");
        Transform tMid = lightning.transform.Find("Mid");
        Transform tFinish = lightning.transform.Find("Finish");

        if (!tStart || !tMid || !tFinish)
        {
            Debug.LogWarning("Lightning prefab must contain Start, Mid, and Finish transforms.");
            return;
        }

        // Direção e rotação global do raio
        Vector3 dir = (endPos - startPos).normalized;
        lightning.transform.position = startPos;
        lightning.transform.rotation = Quaternion.LookRotation(dir);

        float distance = Vector3.Distance(startPos, endPos);

        // Posicionamento linear
        tStart.localPosition = Vector3.zero;

        Vector3 midWorld = startPos + dir * (distance * 0.5f);
        Vector3 offset = Random.insideUnitSphere * (distance * midOffsetFactor);
        offset = Vector3.ProjectOnPlane(offset, dir);
        midWorld += offset;
        tMid.position = midWorld;

        tFinish.position = endPos;

        // Luz do final começa apagada
        Light finishLight = tFinish.GetComponentInChildren<Light>();
        if (finishLight) finishLight.enabled = false;
    }

    IEnumerator Lifecycle(GameObject lightning)
    {
        lightning.SetActive(true);

        Transform tFinish = lightning.transform.Find("Finish");
        Light finishLight = tFinish ? tFinish.GetComponentInChildren<Light>() : null;

        Renderer rend = lightning.transform.Find("Ray")?.GetComponent<Renderer>();
        Material mat = rend ? rend.material : null;

        float elapsed = 0f;
        float evoTime = lightningDuration * 0.25f;

        if (mat && mat.HasProperty("_Evolution"))
            mat.SetFloat("_Evolution", 0f);

        bool lightActivated = false;

        while (elapsed < lightningDuration)
        {
            elapsed += Time.deltaTime;

            if (mat && mat.HasProperty("_Evolution"))
            {
                float evo = Mathf.Clamp01(elapsed / evoTime);
                mat.SetFloat("_Evolution", evo);

                // Ativa a luz quando _Evolution chega a 1
                if (!lightActivated && evo >= 1f)
                {
                    if (finishLight) finishLight.enabled = true;
                    lightActivated = true;
                }
            }

            yield return null;
        }

        // Desliga a luz e devolve o raio ao pool
        if (finishLight) finishLight.enabled = false;
        lightning.SetActive(false);
    }

    GameObject GetAvailable()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }
        return null;
    }
}
