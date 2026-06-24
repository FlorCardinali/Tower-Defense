using UnityEngine;

public class BossController : MonoBehaviour
{
    // Unity ejecuta esto automáticamente cuando el objeto es destruido (vida = 0)
    void OnDestroy()
    {
        // Le avisamos al GameManager que el Boss cayó
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotificarMuerteBoss();
        }
    }
}