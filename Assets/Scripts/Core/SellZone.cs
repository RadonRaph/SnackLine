using UnityEngine;
using TMPro;

/// <summary>
/// Fin de la chaîne.
/// Les "bag" sont corrects.
/// Tous les autres items sont à refaire.
/// </summary>
public class SellZone : Machine
{
    [Header("Vente")]
    public int totalSold = 0;
    public int totalRefaire = 0;

    [Header("Objectif")]
    public int objectif = 5;

    [Header("Interface")]
    public TMP_Text formulesText;
    public TMP_Text refaireText;
    public GameObject endText;

    public override void OnItemEnter(Item item)
    {
        // Cacher l'item
        item.gameObject.SetActive(false);
    }

    public override void OnProgress(float progress)
    {
        // Pas de progression
    }

    public override void OnEnd()
    {
        foreach (Item item in items)
        {
            // Un bag est une formule correcte
            if (item.itemName == "bag")
            {
                totalSold++;
                Debug.Log("Formule terminée : " + totalSold);
            }
            else
            {
                // Tous les autres items sont à refaire
                totalRefaire++;
                Debug.Log("À refaire : " + totalRefaire);
            }

            // Mettre à jour l'interface
            formulesText.text = " " + totalSold;
            refaireText.text = " " + totalRefaire;

            // Vérifier si l'objectif est atteint
            if (totalSold >= objectif)
            {
                endText.SetActive(true);
            }

            // Détruire l'item
            Destroy(item.gameObject);
        }
    }
}