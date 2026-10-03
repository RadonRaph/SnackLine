using UnityEngine;

/// <summary>
/// Machine qui assemble plusieurs items en un seul.
/// Exemple : bread + meat-cooked = burger.
/// </summary>
public class CombinerMachine : Machine
{
    [Header("Assemblage")]
    [Tooltip("Le prefab du produit fini.")]
    public Item resultPrefab;

    // Quand un ingrédient entre dans la machine
    public override void OnItemEnter(Item item)
    {
        // Afficher le nom de l'ingrédient
        Debug.Log("Ingrédient : " + item.itemName);

        // Cacher l'ingrédient
        item.gameObject.SetActive(false);
    }

    // Pendant le travail de la machine
    public override void OnProgress(float progress)
    {
        // Mettre à jour la barre de progression
        SetProgressBar(progress);
    }

    // Quand le travail est terminé
    public override void OnEnd()
    {
        // Détruire tous les ingrédients
        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }

        // Créer le produit fini
        Item newItem = CreateItem(resultPrefab);

        // Faire sortir le produit fini
        Output(newItem);

        // Vider la barre de progression
        SetProgressBar(0);
    }
}