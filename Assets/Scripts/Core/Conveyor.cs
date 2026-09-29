using UnityEngine;

/// <summary>
/// [CORE] Tapis roulant. C'est une Machine comme les autres : sers-t'en d'exemple !
///
///   OnItemEnter : l'item se place au début du tapis (il reste visible)
///   OnProgress  : l'item avance du début du tapis jusqu'à l'OutputPoint
///   OnEnd       : l'item sort du tapis
/// </summary>
public class Conveyor : Machine
{
    [Header("Convoyeur")]
    [Tooltip("Début du tapis.")]
    public Transform startPoint;

    [Tooltip("Optionnel : milieu du virage (pour les convoyeurs d'angle).")]
    public Transform middlePoint;

    protected override void OnItemEnter(Item item)
    {
        item.transform.position = startPoint.position;
    }

    protected override void OnProgress(float progress)
    {
        Item item = items[0];

        if (middlePoint == null)
        {
            // Tapis droit : on va du début à la sortie
            item.transform.position = Vector3.Lerp(startPoint.position, outputPoint.position, progress);
        }
        else if (progress < 0.5f)
        {
            // Virage, 1re moitié : du début au milieu
            item.transform.position = Vector3.Lerp(startPoint.position, middlePoint.position, progress * 2f);
        }
        else
        {
            // Virage, 2e moitié : du milieu à la sortie
            item.transform.position = Vector3.Lerp(middlePoint.position, outputPoint.position, (progress - 0.5f) * 2f);
        }
    }

    protected override void OnEnd()
    {
        Output(items[0]);
    }

    // Dessine le trajet du tapis dans la vue Scene
    void OnDrawGizmos()
    {
        if (startPoint == null || outputPoint == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        if (middlePoint == null)
        {
            Gizmos.DrawLine(startPoint.position, outputPoint.position);
        }
        else
        {
            Gizmos.DrawLine(startPoint.position, middlePoint.position);
            Gizmos.DrawLine(middlePoint.position, outputPoint.position);
        }
        Gizmos.DrawWireSphere(outputPoint.position, 0.08f);
    }
}
