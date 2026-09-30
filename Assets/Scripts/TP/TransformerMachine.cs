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

    // BONUS : effets visuels (tous optionnels)
    [Header("Effets (bonus)")]
    [Tooltip("Particules jouées pendant le travail (ex : fumée).")]
    public ParticleSystem workEffect;

    [Tooltip("Particules jouées à la fin (ex : étincelles).")]
    public ParticleSystem endEffect;

    [Tooltip("Le modèle 3D de la machine, qui change de couleur.")]
    public Renderer machineRenderer;

    [Tooltip("Couleur de la machine quand elle chauffe.")]
    public Color hotColor = new Color(1f, 0.3f, 0.1f);

    Color normalColor;
    bool colorSaved = false;

    // 1. Un item vient d'entrer dans la machine
    public override void OnItemEnter(Item item)
    {
        // TODO 1 : cacher l'item, il est maintenant "dans" la machine.
        item.gameObject.SetActive(false);

        // BONUS : lancer la fumée
        if (workEffect != null)
        {
            workEffect.Play();
        }

        // BONUS : garder en mémoire la couleur normale (une seule fois)
        if (machineRenderer != null && colorSaved == false)
        {
            normalColor = machineRenderer.material.color;
            colorSaved = true;
        }
    }

    // 2. La machine travaille : progress va de 0 (début) à 1 (fin)
    public override void OnProgress(float progress)
    {
        // TODO 2 : remplir la barre de progression avec la valeur de progress.
        SetProgressBar(progress);

        // BONUS : la machine rougit petit à petit
        if (machineRenderer != null)
        {
            machineRenderer.material.color = Color.Lerp(normalColor, hotColor, progress);
        }
    }

    // 3. Le travail est fini
    public override void OnEnd()
    {
        // TODO 3 : récupérer l'item qui est dans la machine : c'est le premier de la liste items.
        Item oldItem = items[0];

        // TODO 4 : détruire l'ancien item.
        Destroy(oldItem.gameObject);

        // TODO 5 : créer le nouvel item à partir de resultPrefab.
        Item newItem = CreateItem(resultPrefab);

        // TODO 6 : faire sortir le nouvel item de la machine.
        Output(newItem);

        // TODO 7 : vider la barre de progression.
        SetProgressBar(0);

        // BONUS : arrêter la fumée, lancer les étincelles, remettre la couleur normale
        if (workEffect != null)
        {
            workEffect.Stop();
        }

        if (endEffect != null)
        {
            endEffect.Play();
        }

        if (machineRenderer != null)
        {
            machineRenderer.material.color = normalColor;
        }
    }
}