using UnityEngine;

/// <summary>
/// [BONUS] Caméra de présentation : tourne lentement autour de l'usine.
/// </summary>
public class CameraOrbit : MonoBehaviour
{
    [Tooltip("Le point autour duquel la caméra tourne (le centre de l'usine).")]
    public Transform target;

    [Tooltip("Distance entre la caméra et le centre.")]
    public float distance = 12f;

    [Tooltip("Hauteur de la caméra.")]
    public float height = 8f;

    [Tooltip("Vitesse de rotation, en degrés par seconde.")]
    public float speed = 10f;

    float angle = 0f;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        angle = angle + speed * Time.deltaTime;

        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * new Vector3(0f, height, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}