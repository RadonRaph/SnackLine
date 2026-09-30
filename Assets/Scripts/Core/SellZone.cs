using UnityEngine;
using TMPro;

/// <summary>
/// [CORE] Fin de la chaîne : les items qui arrivent ici sont vendus
/// (comptés puis détruits).
/// Le nombre vendu et l'objectif de production sont affichés.
/// </summary>
public class SellZone : Machine
{
    [Header("Vente")]
    [Tooltip("Nombre total d'items vendus.")]
    public int totalSold = 0;

    [Header("Interface")]
    public TMP_Text productionText;
    public TMP_Text objectiveText;

    [Header("Objectif")]
    public int productionObjective = 10;

    public override void OnItemEnter(Item item)
    {
        // L'item est caché : il est "dans" la zone de vente
        item.gameObject.SetActive(false);
    }

    public override void OnProgress(float progress)
    {
        // Pas de barre de progression pour la vente
    }

    public override void OnEnd()
    {
        foreach (Item item in items)
        {
            if (totalSold < productionObjective)
            {
                totalSold = totalSold + 1;

                Debug.Log("Vendu : " + item.itemName +
                          " (total vendu : " + totalSold + ")");

                // Mise à jour du compteur
                if (productionText != null)
                {
                    productionText.text =
                        "Burgers vendus : " + totalSold;
                }

                // Mise à jour de l'objectif
                if (objectiveText != null)
                {
                    if (totalSold >= productionObjective)
                    {
                        objectiveText.text = "Objectif atteint !";
                    }
                    else
                    {
                        objectiveText.text =
                            "Objectif : " + totalSold +
                            " / " + productionObjective;
                    }
                }
            }

            Destroy(item.gameObject);
        }
    }
}