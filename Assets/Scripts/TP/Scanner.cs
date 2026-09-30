using UnityEngine;

public class ScannerMachine : Machine
{
    public Light scanLight;

    public override void OnItemEnter(Item item)
    {
        item.gameObject.SetActive(false);

        if (scanLight != null)
            scanLight.enabled = true;
    }

    public override void OnProgress(float progress)
    {
        SetProgressBar(progress);
    }

    public override void OnEnd()
    {
        Item item = items[0];

        item.gameObject.SetActive(true);
        Output(item);

        if (scanLight != null)
            scanLight.enabled = false;

        SetProgressBar(0);
    }
}