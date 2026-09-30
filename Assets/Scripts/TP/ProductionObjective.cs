using UnityEngine;
using TMPro;

public class ProductionObjective : MonoBehaviour
{
    [Header("Paramètres")]
    public int targetCount = 10;
    public TMP_Text counterText;

    [Header("Référence")]
    public SellZone sellZone; // Glisse ton objet SellZone ici dans l'inspecteur

    void Update()
    {
        if (sellZone != null && counterText != null)
        {
            int currentCount = sellZone.totalSold;
            counterText.text = currentCount + " / " + targetCount;

            if (currentCount >= targetCount)
            {
                counterText.text = "Objectif atteint ! (" + currentCount + "/" + targetCount + ")";
            }
        }
    }
}