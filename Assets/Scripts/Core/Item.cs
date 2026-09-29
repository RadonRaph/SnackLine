using UnityEngine;

/// <summary>
/// [CORE] Un ingrédient ou un produit qui circule dans l'usine (pain, steak, burger...).
/// Chaque prefab du dossier Prefabs/Items possède ce script.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Item : MonoBehaviour
{
    [Tooltip("Nom de l'item. Les machines l'utilisent dans leur recette (Accepted Items).")]
    public string itemName;

    // Machine qui contient l'item en ce moment (null = l'item est libre)
    [HideInInspector] public Machine currentMachine;

    // Dernière machine d'où l'item est sorti : il ne peut pas y retourner
    [HideInInspector] public Machine previousMachine;

    void Awake()
    {
        // L'item ne tombe pas : ce sont les machines qui le déplacent
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        // Sans ça, un item immobile "s'endort" et les machines ne le voient plus
        body.sleepThreshold = 0f;

        GetComponent<BoxCollider>().isTrigger = true;

        if (string.IsNullOrEmpty(itemName))
        {
            itemName = name;
        }
    }
}
