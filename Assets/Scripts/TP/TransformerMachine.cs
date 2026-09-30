using UnityEngine;

/// <summary>
/// Machine qui transforme un item en un autre.
/// </summary>
public class TransformerMachine : Machine
{
    [Header("Transformation")]
    [Tooltip("Le prefab de l'item qui sort de la machine.")]
    public Item resultPrefab;

    [Header("Lumière")]
    [Tooltip("Lumière qui clignote pendant le travail.")]
    public Light workingLight;

    // Vitesse du clignotement
    public float blinkSpeed = 10f;

    // Quand un item entre dans la machine
    public override void OnItemEnter(Item item)
    {
        // Cacher l'item
        item.gameObject.SetActive(false);
    }

    // Pendant le travail de la machine
    public override void OnProgress(float progress)
    {
        // Mettre à jour la barre de progression
        SetProgressBar(progress);

        // Faire clignoter la lumière
        if (workingLight != null)
        {
            workingLight.enabled =
                Mathf.Sin(Time.time * blinkSpeed) > 0;
        }
    }

    // Quand le travail est terminé
    public override void OnEnd()
    {
        // Récupérer l'ancien item
        Item oldItem = items[0];

        // Détruire l'ancien item
        Destroy(oldItem.gameObject);

        // Créer le nouvel item
        Item newItem = CreateItem(resultPrefab);

        // Faire sortir le nouvel item
        Output(newItem);

        // Vider la barre de progression
        SetProgressBar(0);

        // Éteindre la lumière
        if (workingLight != null)
        {
            workingLight.enabled = false;
        }
    }
}