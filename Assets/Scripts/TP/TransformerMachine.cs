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

    protected override void OnEnd()
    {
        // TODO 1 : détruire l'item qui est entré dans la machine.
        //          Outil : DestroyInputItems();


        // TODO 2 : créer le nouvel item à partir de resultPrefab, et le ranger dans une variable.
        //          Outil : Item newItem = CreateItem(...);


        // TODO 3 : faire sortir le nouvel item de la machine.
        //          Outil : Output(...);

    }
}
