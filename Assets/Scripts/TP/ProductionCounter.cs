using UnityEngine;
using TMPro;

public class ProductionCounter : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("La SellZone de la scène.")]
    public SellZone sellZone;

    [Tooltip("Le texte UI à mettre à jour.")]
    public TextMeshProUGUI counterText;

    [Header("Affichage")]
    public string label = "Produits vendus : ";

    private int lastCount = -1;

    void Update()
    {
        if (sellZone == null || counterText == null) return;

        
        if (sellZone.totalSold == lastCount) return;

        lastCount = sellZone.totalSold;
        counterText.text = label + lastCount;
    }
}