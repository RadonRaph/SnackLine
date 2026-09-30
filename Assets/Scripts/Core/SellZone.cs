using UnityEngine;
using TMPro;

/// <summary>
/// Fin de la chaîne.
/// Tous les items sont récupérés et détruits.
/// Les "bag" sont corrects, les autres sont à refaire.
/// 
public class SellZone : Machine
{
    [Header("Vente")]
    public int totalSold = 0;
    public int totalRefaire = 0;

    [Header("Interface")]
    public TMP_Text formulesText;
    public TMP_Text refaireText;

    // Quand un item arrive
    public override void OnItemEnter(Item item)
    {
        // Cacher l'item
        item.gameObject.SetActive(false);
    }

    public override void OnProgress(float progress)
    {
        // Pas de progression
    }

    // Quand l'item arrive à la fin
    public override void OnEnd()
    {
        foreach (Item item in items)
        {
            // Si c'est un bag, le produit est correct
            if (item.itemName == "bag")
            {
                totalSold++;
                Debug.Log("Formule terminée : " + item.itemName);
            }
            else
            {
                // Tous les autres items sont à refaire
                totalRefaire++;
                Debug.Log("À refaire : " + item.itemName);
            }

            // Mettre à jour l'interface
            formulesText.text = " " + totalSold;
            refaireText.text = " " + totalRefaire;

            // Détruire l'item dans tous les cas
            Destroy(item.gameObject);
        }
    }
}