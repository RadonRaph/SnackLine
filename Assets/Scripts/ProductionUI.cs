using UnityEngine;
using TMPro;

public class ProductionUI : MonoBehaviour
{
    [Header("Zones de Vente")]
    [SerializeField] private SellZone sellZonePizza;
    [SerializeField] private SellZone sellZoneFrite;
    [SerializeField] private SellZone sellZoneBurger;

    [Header("Machines / Spawners à contrôler")]
    [SerializeField] private GameObject spawnerPizza;
    [SerializeField] private GameObject spawnerFrite;
    [SerializeField] private GameObject machineBurger;

    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI victoryText; // Le texte de fin de production

    [Header("Objectifs")]
    [SerializeField] private int objectifPizza = 5;
    [SerializeField] private int objectifFrite = 5;
    [SerializeField] private int objectifBurger = 5;

    private void Start()
    {
        // Cache le message de fin au démarrage
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        int soldPizza = sellZonePizza != null ? sellZonePizza.totalSold : 0;
        int soldFrite = sellZoneFrite != null ? sellZoneFrite.totalSold : 0;
        int soldBurger = sellZoneBurger != null ? sellZoneBurger.totalSold : 0;
        int totalGlobal = soldPizza + soldFrite + soldBurger;

        bool pizzaAtteinte = soldPizza >= objectifPizza;
        bool friteAtteinte = soldFrite >= objectifFrite;
        bool burgerAtteint = soldBurger >= objectifBurger;

        // Arrêt individuel des machines
        if (spawnerPizza != null) spawnerPizza.SetActive(!pizzaAtteinte);
        if (spawnerFrite != null) spawnerFrite.SetActive(!friteAtteinte);
        if (machineBurger != null) machineBurger.SetActive(!burgerAtteint);

        // Affichage des compteurs
        if (counterText != null)
        {
            string statutPizza = pizzaAtteinte ? " (OK)" : "";
            string statutFrite = friteAtteinte ? " (OK)" : "";
            string statutBurger = burgerAtteint ? " (OK)" : "";

            counterText.text = "Pizzas : " + soldPizza + " / " + objectifPizza + statutPizza + "\n" +
                               "Frites : " + soldFrite + " / " + objectifFrite + statutFrite + "\n" +
                               "Burgers : " + soldBurger + " / " + objectifBurger + statutBurger + "\n" +
                               "Total : " + totalGlobal;
        }

        // Fin de production globale lorsque TOUS les produits ont atteint leur quota
        if (pizzaAtteinte && friteAtteinte && burgerAtteint)
        {
            if (victoryText != null)
            {
                victoryText.gameObject.SetActive(true);
                victoryText.text = "FIN DE PRODUCTION ! OBJECTIFS ATTEINTS !";
            }

            // Optionnel : fige le temps du jeu pour arrêter définitivement l'usine
            // Time.timeScale = 0f;
        }
    }
}