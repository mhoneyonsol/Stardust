using UnityEngine;

public class FixScale : MonoBehaviour
{
    // Échelle désirée
    private Vector3 desiredScale = new Vector3(0.33f, 0.33f, 0.33f);

    // Appelé une fois au début
    void Start()
    {
        // Fixer l'échelle
        transform.localScale = desiredScale;
    }

    // Optionnel : Vérification continue
    void Update()
    {
        if (transform.localScale != desiredScale)
        {
            Debug.LogWarning($"L'échelle a changé : {transform.localScale}. Réinitialisation...");
            transform.localScale = desiredScale;
        }
    }
}
