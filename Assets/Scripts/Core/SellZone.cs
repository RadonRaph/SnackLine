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

    // -------
    // compteur par machine
    public int totalHotDog = 0;
    public int totalBurger =0;
    public int totalGateau = 0;
    public int totalGlace = 0;

    public int totalVendu = 0;
    // -------

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
            Debug.Log("Vendu : " + item.itemName + " (total vendu : " + totalSold + ")");
            Destroy(item.gameObject);

            //compteur par machine
            if (item.itemName == "hot-dog")
            {
                totalHotDog = totalHotDog + 1;
            }

            else if (item.itemName =="burger-cheese")
            {
                totalBurger=totalBurger + 1;
            }

            else if (item.itemName == "cake-birthday")
            {
                totalBurger = totalGateau + 1;
            }

            else if (item.itemName == "popsicle-chocolate")
            {
                totalBurger = totalGlace + 1;
            }

            totalVendu = totalHotDog + totalBurger + totalGateau + totalGlace;

        }
    }
}

