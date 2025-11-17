using UnityEngine;

public class OpenWebsite : MonoBehaviour
{
    // Função pública que recebe a URL e abre no navegador padrão
    public void OpenURL(string site)
    {
        if (!string.IsNullOrEmpty(site))
        {
            Application.OpenURL(site);
        }
        else
        {
            Debug.LogWarning("URL vazia ou inválida!");
        }
    }
}
