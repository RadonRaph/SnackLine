using TMPro;
using UnityEngine;

/// <summary>
/// [CORE] Fin de la chaîne : les items qui arrivent ici sont vendus (comptés puis détruits).
/// C'est aussi une Machine : un exemple de machine qui ne sort rien.
/// </summary>
public class SellZone : Machine
{
    [Header("Vente")]
    [Tooltip("Nombre total d'items vendus.")]
    public int totalSold = 0;

    public TMP_Text Text;

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
            totalSold = totalSold + 1;
            // Debug.Log("Vendu : " + item.itemName + " (total vendu : " + totalSold + ")");
            Text.text = totalSold.ToString();
            Destroy(item.gameObject);
        }
    }
}
