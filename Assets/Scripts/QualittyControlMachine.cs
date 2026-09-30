using UnityEngine;

/// <summary>
/// [BONUS] Machine de contrôle qualité : inspecte chaque item.
/// Les bons items continuent, les mauvais sont jetés.
///
/// Dans l'Inspector :
///   - Accepted Items : laisser vide (accepte tout, un item à la fois)
///   - Reject Chance  : la chance qu'un item soit raté (0.2 = 20 %)
/// </summary>
public class QualityControlMachine : Machine
{
    [Header("Contrôle qualité")]
    [Tooltip("Chance qu'un item soit rejeté : 0 = jamais, 1 = toujours.")]
    [Range(0f, 1f)]
    public float rejectChance = 0.2f;

    [Tooltip("Hauteur où l'item est inspecté, au-dessus de la machine.")]
    public float inspectHeight = 1.2f;

    [Tooltip("Vitesse de rotation pendant l'inspection (degrés par seconde).")]
    public float spinSpeed = 360f;

    int accepted = 0;
    int rejected = 0;

    // 1. Un item arrive : on le place au-dessus de la machine (il reste visible)
    public override void OnItemEnter(Item item)
    {
        item.transform.position = transform.position + Vector3.up * inspectHeight;
    }

    // 2. Pendant l'inspection : l'item tourne sur lui-même
    public override void OnProgress(float progress)
    {
        SetProgressBar(progress);

        Item item = items[0];
        item.transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
    }

    // 3. Fin de l'inspection : bon ou mauvais ?
    public override void OnEnd()
    {
        Item item = items[0];
        item.transform.rotation = Quaternion.identity;

        if (Random.value < rejectChance)
        {
            // Mauvais : on le jette
            rejected = rejected + 1;
            Debug.Log("Contrôle qualité : " + item.itemName + " rejeté ❌ (rejetés : " + rejected + ")");
            Destroy(item.gameObject);
        }
        else
        {
            // Bon : il continue vers la vente
            accepted = accepted + 1;
            Debug.Log("Contrôle qualité : " + item.itemName + " validé ✅ (validés : " + accepted + ")");
            Output(item);
        }

        SetProgressBar(0);
    }
}