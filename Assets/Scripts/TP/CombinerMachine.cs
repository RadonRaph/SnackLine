using UnityEngine;
using TMPro;

public class CombinerMachine : Machine
{
    [Header("Assemblage")]
    public Item resultPrefab;

    [Header("Ecran de production")]
    public TMP_Text productionText;

    private int nombreProduits = 0;

    public override void OnItemEnter(Item item)
    {
        Debug.Log("Ingrédient reçu : " + item.itemName);

        item.gameObject.SetActive(false);
    }

    public override void OnProgress(float progress)
    {
        SetProgressBar(progress);
    }

    public override void OnEnd()
    {
        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }

        Item newItem = CreateItem(resultPrefab);

        Output(newItem);

        // Un sushi supplémentaire a été assemblé
        nombreProduits++;

        // Mise à jour de l'écran 3D
        productionText.text =
        "SUSHIS \n" + nombreProduits;

        SetProgressBar(0);
    }
}