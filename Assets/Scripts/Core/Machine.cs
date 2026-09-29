using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [CORE] Classe parente de toutes les machines de l'usine (convoyeur, four, assembleur...).
///
/// Une machine est composée de :
///   - InputZone   : un objet enfant avec un Box Collider. Les items qui le touchent entrent dans la machine.
///   - OutputPoint : un objet enfant vide. Les items sortent à cet endroit (il doit toucher la machine suivante).
///   - ProgressBar : (optionnel) un objet enfant qui s'agrandit pendant le travail.
///
/// Déroulement :
///   1. Un item entre dans la machine            -> OnItemEnter(item)
///   2. La machine travaille "Progress Time" s   -> OnProgress(progress) à chaque image (progress va de 0 à 1)
///   3. Le travail est fini                      -> OnEnd()
///
/// ATTENTION : dans ta machine, n'écris pas de fonction Awake, Update ou OnTriggerStay :
/// elles remplaceraient celles de Machine et plus rien ne marcherait.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public abstract class Machine : MonoBehaviour
{
    [Header("Objets de la machine")]
    [Tooltip("Objet enfant avec un Box Collider : zone où les items entrent.")]
    public Transform inputZone;

    [Tooltip("Objet enfant vide : endroit où les items sortent.")]
    public Transform outputPoint;

    [Tooltip("Optionnel : objet enfant qui s'agrandit pendant le travail.")]
    public Transform progressBar;

    [Header("Réglages")]
    [Tooltip("Durée du travail, en secondes.")]
    public float progressTime = 2f;

    [Tooltip("Noms des items acceptés (la recette). La machine démarre quand elle les a TOUS.\n" +
             "Liste vide = accepte n'importe quel item, un par un.")]
    public string[] acceptedItems = new string[0];

    // Les items qui sont dans la machine
    protected List<Item> items = new List<Item>();

    // Vrai pendant que la machine travaille
    protected bool isWorking = false;

    float timer = 0f;
    Item lastOutput;
    float blockedTime = 0f;

    // =====================================================================
    // Les 3 fonctions à écrire dans ta machine
    // =====================================================================

    /// <summary>Un item vient d'entrer. Par défaut, il est caché (il est "dans" la machine).</summary>
    protected virtual void OnItemEnter(Item item)
    {
        item.gameObject.SetActive(false);
    }

    /// <summary>Appelée à chaque image pendant le travail. progress va de 0 (début) à 1 (fin).</summary>
    protected virtual void OnProgress(float progress)
    {
    }

    /// <summary>Le travail est fini : c'est ici que la machine produit quelque chose.</summary>
    protected abstract void OnEnd();

    // =====================================================================
    // Les outils à utiliser dans ta machine
    // =====================================================================

    /// <summary>Crée un nouvel item dans la machine. Ensuite, sors-le avec Output(...).</summary>
    protected Item CreateItem(Item prefab)
    {
        if (prefab == null)
        {
            Debug.LogError(name + " : CreateItem a reçu un prefab vide. " +
                           "Glisse un prefab du dossier Prefabs/Items dans le champ de ta machine (Inspector).", this);
            return null;
        }

        Item newItem = Instantiate(prefab, transform.position, Quaternion.identity);
        newItem.name = prefab.name;
        newItem.currentMachine = this;
        return newItem;
    }

    /// <summary>Sort un item de la machine : il est posé sur l'OutputPoint.</summary>
    protected void Output(Item item)
    {
        if (item == null)
        {
            Debug.LogError(name + " : Output a reçu un item vide (null).", this);
            return;
        }
        if (outputPoint == null)
        {
            Debug.LogError(name + " : le champ Output Point est vide. Glisse l'objet enfant OutputPoint dedans (Inspector).", this);
            return;
        }

        items.Remove(item);
        item.gameObject.SetActive(true);
        item.transform.position = outputPoint.position;
        item.currentMachine = null;
        item.previousMachine = this;
        lastOutput = item;
    }

    /// <summary>Détruit tous les items qui sont dans la machine.</summary>
    protected void DestroyInputItems()
    {
        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }
        items.Clear();
    }

    /// <summary>Renvoie l'item de la machine qui porte ce nom, ou null s'il n'y en a pas.</summary>
    protected Item GetItem(string itemName)
    {
        foreach (Item item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }
        return null;
    }

    // =====================================================================
    // Fonctionnement interne de la machine
    // =====================================================================

    void Awake()
    {
        // La machine ne bouge pas : le Rigidbody sert seulement à détecter les items
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        if (inputZone == null)
        {
            Debug.LogError(name + " : le champ Input Zone est vide. Glisse l'objet enfant InputZone dedans (Inspector).", this);
        }
        else if (inputZone.GetComponent<Collider>() == null)
        {
            Debug.LogError(name + " : l'objet InputZone n'a pas de Box Collider. Ajoute-le (Add Component > Box Collider).", this);
        }
        else
        {
            inputZone.GetComponent<Collider>().isTrigger = true;
        }

        SetProgressBar(0f);
    }

    void Update()
    {
        CheckIfBlocked();

        if (isWorking == false)
        {
            return;
        }

        timer = timer + Time.deltaTime;
        float progress = timer / progressTime;
        if (progress > 1f)
        {
            progress = 1f;
        }

        OnProgress(progress);
        SetProgressBar(progress);

        if (timer >= progressTime)
        {
            isWorking = false;
            SetProgressBar(0f);
            OnEnd();

            if (items.Count > 0)
            {
                Debug.LogWarning(name + " : des items sont restés dans la machine après OnEnd. " +
                                 "Utilise DestroyInputItems() ou Output(item). Ils sont supprimés.", this);
                DestroyInputItems();
            }
        }
    }

    // Appelée tant qu'un objet touche l'InputZone
    void OnTriggerStay(Collider other)
    {
        Item item = other.GetComponent<Item>();
        if (item == null)
        {
            return;
        }

        if (isWorking || IsOutputWaiting() || item.currentMachine != null || item.previousMachine == this)
        {
            return;
        }

        if (CanAccept(item) == false)
        {
            return;
        }

        item.currentMachine = this;
        items.Add(item);
        OnItemEnter(item);

        if (items.Count >= NeededItemCount())
        {
            isWorking = true;
            timer = 0f;
        }
    }

    // Vrai si le dernier item sorti n'a pas encore été pris par la machine suivante
    bool IsOutputWaiting()
    {
        return lastOutput != null && lastOutput.currentMachine == null;
    }

    bool CanAccept(Item item)
    {
        if (acceptedItems.Length == 0)
        {
            return true;
        }

        // Un seul item de chaque sorte
        if (GetItem(item.itemName) != null)
        {
            return false;
        }

        foreach (string accepted in acceptedItems)
        {
            if (accepted == item.itemName)
            {
                return true;
            }
        }
        return false;
    }

    int NeededItemCount()
    {
        if (acceptedItems.Length == 0)
        {
            return 1;
        }
        return acceptedItems.Length;
    }

    void SetProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.localScale = new Vector3(progress, 1f, 1f);
        }
    }

    // Prévient dans la Console si la machine est bloquée depuis 5 secondes
    void CheckIfBlocked()
    {
        if (IsOutputWaiting() == false)
        {
            blockedTime = 0f;
            return;
        }

        blockedTime = blockedTime + Time.deltaTime;
        if (blockedTime > 5f)
        {
            Debug.LogWarning(name + " : attend depuis 5 s que la machine suivante prenne l'item " + lastOutput.itemName + ". " +
                             "C'est normal si la machine suivante est occupée. Sinon, vérifie que l'OutputPoint touche " +
                             "l'InputZone de la machine suivante, et que cette machine accepte cet item (Accepted Items).", this);
            blockedTime = 0f;
        }
    }

    // Dessine l'OutputPoint dans la vue Scene
    void OnDrawGizmos()
    {
        if (outputPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(outputPoint.position, 0.1f);
        }
    }
}
