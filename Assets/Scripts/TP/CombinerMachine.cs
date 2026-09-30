using UnityEngine;

/// <summary>
/// [TP] Machine qui assemble plusieurs items en un seul.
/// Exemple : "bread" + "meat-cooked" donnent un "burger".
///
/// Dans l'Inspector :
///   - Accepted Items : la recette, un nom par ligne (ex : bread, meat-cooked)
///   - Result Prefab  : le prefab du produit fini (ex : Prefabs/Items/burger)
/// La machine attend d'avoir TOUS les ingrédients de la recette avant de travailler.
/// </summary>
public class CombinerMachine : Machine
{
    [Header("Assemblage")]
    [Tooltip("Le prefab du produit fini.")]
    public Item resultPrefab;

    public GameObject VFX;

    private void Start()
    {
        VFX?.SetActive(false);
    }

    // 1. Un ingrédient vient d'entrer dans la machine
    public override void OnItemEnter(Item item)
    {
        item.gameObject.SetActive(false);

    }

    // 2. La machine travaille : progress va de 0 (début) à 1 (fin)
    public override void OnProgress(float progress)
    {
        if (progress < 0.1f)
        {
            VFX?.SetActive(false);
        }
        else
        {
            VFX?.SetActive(true);
        }

        SetProgressBar(progress);



    }

    // 3. Le travail est fini
    public override void OnEnd()
    {
        // TODO 4 : détruire TOUS les ingrédients de la liste items.
        //          Astuce : une boucle foreach (Item item in items) { ... }

        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }

        // TODO 5 : créer le produit fini à partir de resultPrefab.
        Item newItem = CreateItem(resultPrefab);


        // TODO 6 : faire sortir le produit fini de la machine.
        Output(newItem);

        // TODO 7 : vider la barre de progression.
        SetProgressBar(0);
        VFX?.SetActive(false);
    }
}
