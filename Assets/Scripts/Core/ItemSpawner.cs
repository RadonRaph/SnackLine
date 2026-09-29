using UnityEngine;

/// <summary>
/// [CORE] Source d'ingrédients : crée un item toutes les "Interval" secondes sur l'OutputPoint.
/// Place un convoyeur juste devant pour emporter les items.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Tooltip("L'item à créer (un prefab du dossier Prefabs/Items).")]
    public Item itemPrefab;

    [Tooltip("Temps entre deux items, en secondes.")]
    public float interval = 2f;

    [Tooltip("Objet enfant vide : endroit où les items apparaissent. Il doit toucher un convoyeur.")]
    public Transform outputPoint;

    float timer = 0f;
    Item lastItem;

    void Start()
    {
        if (itemPrefab == null)
        {
            Debug.LogError(name + " : le champ Item Prefab est vide. Glisse un prefab du dossier Prefabs/Items dedans (Inspector).", this);
        }
        if (outputPoint == null)
        {
            Debug.LogError(name + " : le champ Output Point est vide. Glisse l'objet enfant OutputPoint dedans (Inspector).", this);
        }
    }

    void Update()
    {
        if (itemPrefab == null || outputPoint == null)
        {
            return;
        }

        // On attend que le dernier item soit emporté par un convoyeur
        if (lastItem != null && lastItem.currentMachine == null)
        {
            return;
        }

        timer = timer + Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            lastItem = Instantiate(itemPrefab, outputPoint.position, Quaternion.identity);
            lastItem.name = itemPrefab.name;
        }
    }

    // Dessine l'OutputPoint dans la vue Scene
    void OnDrawGizmos()
    {
        if (outputPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(outputPoint.position, 0.1f);
        }
    }
}
