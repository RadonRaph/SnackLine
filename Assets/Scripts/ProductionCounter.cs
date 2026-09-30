using UnityEngine;
using TMPro;

/// <summary>
/// [BONUS] Affiche le nombre de produits vendus et un message quand la commande est terminée.
/// </summary>
public class ProductionCounter : MonoBehaviour
{
    [Header("Liens")]
    [Tooltip("La SellZone où arrivent les burgers.")]
    public SellZone sellZone;

    [Tooltip("Le texte du compteur.")]
    public TMP_Text counterText;

    [Tooltip("Le texte du message de fin.")]
    public TMP_Text messageText;

    [Header("Commande")]
    public string productName = "Burgers";
    public int goal = 10;

    bool finished = false;

    void Start()
    {
        // Le message est caché au début
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (sellZone == null || counterText == null)
        {
            return;
        }

        int sold = sellZone.totalSold;
        counterText.text = productName + " produits : " + sold + " / " + goal;

        // Objectif atteint : on affiche le message une seule fois
        if (finished == false && sold >= goal)
        {
            finished = true;
            Debug.Log("Commande terminée !");

            if (messageText != null)
            {
                messageText.gameObject.SetActive(true);
                messageText.text = "Commande terminée ! " + goal + " " + productName.ToLower() + " livrés !";
            }
        }
    }
}