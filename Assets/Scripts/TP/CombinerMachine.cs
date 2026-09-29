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

    protected override void OnItemEnter(Item item)
    {
        // TODO 1 : afficher dans la Console le nom de l'ingrédient qui arrive.
        //          Outils : Debug.Log(...); et item.itemName


        // TODO 2 : cacher l'ingrédient, il est maintenant "dans" la machine.
        //          Outil : item.gameObject.SetActive(false);

    }

    protected override void OnEnd()
    {
        // TODO 3 : détruire tous les ingrédients qui sont dans la machine.


        // TODO 4 : créer le produit fini à partir de resultPrefab.


        // TODO 5 : faire sortir le produit fini de la machine.

    }
}
