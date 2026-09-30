using UnityEngine;

// machine qui tranche, donne deux items en sortie
public class TrancheuseMachine : Machine
{
    [Header("Tranchage")]
    [Tooltip("Le prefab d'une tranche'.")]
    public Item resultPrefab;

    [Tooltip("Le nombre de tranches qui sortent de la machine.")]
    public int nbTranche = 2;

    // 1. Un item vient d'entrer dans la machine
    public override void OnItemEnter(Item item)
    {
        // TODO 1 : cacher l'item, il est maintenant "dans" la machine.
        item.gameObject.SetActive(false);

    }

    // 2. La machine travaille : progress va de 0 (début) à 1 (fin)
    public override void OnProgress(float progress)
    {
        // TODO 2 : remplir la barre de progression avec la valeur de progress.
        SetProgressBar (progress);
    }

    // 3. Le travail est fini
    public override void OnEnd()
    {
        // TODO 3 : récupérer l'item qui est dans la machine : c'est le premier de la liste items.
        Item oldItem = items[0];

        // TODO 4 : détruire l'ancien item.
        Destroy(oldItem.gameObject);

        // Faire sortir deux tranches
        for(int i =0; i < nbTranche; i++)
        {
            Item tranche = CreateItem(resultPrefab);
            Output(tranche);
        }

        //Vider la barre de progression.
        SetProgressBar(0);

    }
}
