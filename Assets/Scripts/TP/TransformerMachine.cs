using UnityEngine;

/// <summary>
/// [TP] Machine qui transforme un item en un autre.
/// Exemple : un four qui transforme "meat-raw" (steak cru) en "meat-cooked" (steak cuit).
///
/// Dans l'Inspector :
///   - Accepted Items : le nom de l'item qui entre (ex : meat-raw)
///   - Result Prefab  : le prefab de l'item qui sort (ex : Prefabs/Items/meat-cooked)
/// </summary>
public class TransformerMachine : Machine
{
    [Header("Transformation")]
    [Tooltip("Le prefab de l'item qui sort de la machine.")]
    public Item resultPrefab;

    public GameObject VFX;
    public int resultCount;

    private void Start()
    {
        VFX?.SetActive(false);
    }

    // 1. Un item vient d'entrer dans la machine
    public override void OnItemEnter(Item item)
    {
        // TODO 1 : cacher l'item, il est maintenant "dans" la machine.
        //          Outil : item.gameObject.SetActive(false);

        item.gameObject.SetActive(false);
    }

    // 2. La machine travaille : progress va de 0 (début) à 1 (fin)
    public override void OnProgress(float progress)
    {
        // TODO 2 : remplir la barre de progression avec la valeur de progress.
        //          Outil : SetProgressBar(progress);
        SetProgressBar(progress);
        VFX?.SetActive(Mathf.Sin(progress*25) > 0);
    }

    // 3. Le travail est fini
    public override void OnEnd()
    {
        Destroy(items[0].gameObject);

        for (int i = 0; i < resultCount; i++)
        {
            Item newItem = CreateItem(resultPrefab);
            Output(newItem);
        }

         SetProgressBar(0);
        VFX?.SetActive(false);

    }
}
