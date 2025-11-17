using UnityEngine;

public class FreezeChildWorldTransform : MonoBehaviour
{
    [Header("Escolha o que congelar (em espaço global)")]
    public bool freezePosition = true;
    public bool freezeRotation = true;
    public bool freezeScale = true;

    private Vector3 worldPosition;
    private Quaternion worldRotation;
    private Vector3 worldScale;

    void Start()
    {
        // Armazena os valores globais iniciais
        worldPosition = transform.position;
        worldRotation = transform.rotation;
        worldScale = transform.lossyScale;
    }

    void LateUpdate()
    {
        // Congela posição
        if (freezePosition)
            transform.position = worldPosition;

        // Congela rotação
        if (freezeRotation)
            transform.rotation = worldRotation;

        // Congela escala (ajustando pela escala do pai)
        if (freezeScale)
        {
            if (transform.parent != null)
            {
                transform.localScale = new Vector3(
                    worldScale.x / transform.parent.lossyScale.x,
                    worldScale.y / transform.parent.lossyScale.y,
                    worldScale.z / transform.parent.lossyScale.z
                );
            }
            else
            {
                transform.localScale = worldScale;
            }
        }
    }
}

