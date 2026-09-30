using NUnit.Framework.Internal.Commands;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class Compteur : MonoBehaviour
{
    [Header("References")]

    public SellZone sellZone;
    public TMP_Text textHotDog;
    public TMP_Text textBurger;
    public TMP_Text textGateau;
    public TMP_Text textGlace;
    public TMP_Text textTotalVendu;

    public string label = " vendu(s).";


    private void Update ()

    {
        Show(textHotDog, "Nombre de hot-dog vendus :", sellZone.totalHotDog);
        Show(textBurger, "Nombre de burgers vendus :", sellZone.totalBurger);
        Show(textGateau, "Nombre de gateaux vendus :", sellZone.totalGateau);
        Show(textGlace, "Nombre de glaces vendus :", sellZone.totalGlace);
        Show(textTotalVendu, "Nombre total d'items  :", sellZone.totalVendu);
    }

    private void Show(TMP_Text text, string label, int value)
    {
        if (text == null) return;
        text.text = label + value;
    }

}





    


