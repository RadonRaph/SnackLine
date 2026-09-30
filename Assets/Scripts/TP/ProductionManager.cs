using UnityEngine;
using TMPro;

public class ProductionManager : MonoBehaviour
{
    [Header("Références")]
    public SellZone sellZone;
    public TextMeshProUGUI counterText;
    public GameObject victoryPanel;

    [Header("Paramètres")]
    public int targetGoal = 10;

    private bool isFinished = false;

    void Update()
    {
        if (sellZone == null || isFinished) return;

        // Mise à jour de l'affichage du compteur
        if (counterText != null)
        {
            counterText.text = "Burgers produits : " + sellZone.totalSold;
        }

        // Vérification de l'objectif
        if (sellZone.totalSold >= targetGoal)
        {
            TerminerProduction();
        }
    }

    void TerminerProduction()
    {
        isFinished = true;

        // 1. Afficher l'écran de victoire
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // 2. Trouver et désactiver tous les Spawners de la scène
        // (ItemSpawner hérite de MonoBehaviour)
        ItemSpawner[] spawners = Object.FindObjectsByType<ItemSpawner>(FindObjectsSortMode.None);
        foreach (ItemSpawner spawner in spawners)
        {
            spawner.enabled = false; // coupe la génération d'ingrédients
        }
        // Stoppe tous les convoyeurs
        Conveyor[] conveyors = Object.FindObjectsByType<Conveyor>(FindObjectsSortMode.None);
        foreach (Conveyor c in conveyors)
        {
            c.enabled = false;
        }
        Debug.Log("Objectif atteint ! Arrêt complet des sources.");
    }
}