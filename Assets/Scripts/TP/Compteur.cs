
using TMPro;
using UnityEngine;

public class Compteur : MonoBehaviour
{
    [Header("References")]
    public SellZone[] sellZones;
    public TMP_Text textHotDog;
    public TMP_Text textBurger;
    public TMP_Text textGateau;
    public TMP_Text textGlace;
    public TMP_Text textTotalVendu;

    public GameObject textFin;
    public GameObject imageFin;


    private void Update ()

    {
        // additionner tous les compteurs

        int hotDog = 0;
        int burger = 0;
        int gateau = 0;
        int glace = 0;
        int totalVendu = 0;

        foreach(SellZone zone in sellZones)
        {
            if(zone==null) continue;
            hotDog += zone.totalHotDog;
            burger += zone.totalBurger;
            gateau += zone.totalGateau;
            glace += zone.totalGlace;
            totalVendu += zone.totalVendu;

        }

        Show(textHotDog, "Hot-dog vendus : ", hotDog);
        Show(textBurger, "Burgers vendus : ", burger);
        Show(textGateau, "Gâteaux vendus : ", gateau);
        Show(textGlace, "Glaces vendus : ", glace);
        Show(textTotalVendu, "Total d'items vendus : ",totalVendu);

        if(totalVendu >= 30)
        {
            textFin.SetActive(true);
            imageFin.SetActive(true);
        }
    }

    private void Show(TMP_Text text, string label, int value)
    {
        if (text == null) return;
        text.text = label + value;
    }

}





    


