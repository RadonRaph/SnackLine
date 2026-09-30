using UnityEngine;

public class Scanner : Machine
{
    public GameObject Light;

    public override void OnItemEnter(Item item)
    {
        Light.SetActive(true);
    }
    public override void OnProgress(float progress)
    {
        SetProgressBar(progress);
    }

    public override void OnEnd()
    {
        Light.SetActive(false);
        SetProgressBar(0);
        Output(items[0]);
    }
}
