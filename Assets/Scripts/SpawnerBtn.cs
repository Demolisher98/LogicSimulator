using TMPro;
using UnityEngine;

public class SpawnerBtn : MonoBehaviour
{
    [HideInInspector] public int index;
    [HideInInspector] public bool isCustom = false;

    public void SpawnItem()
    {
        if(!isCustom)
        Simulator.simulator.SpawnItem(index);
    }

    public void SpawnCustomComponent()
    {
        Simulator.simulator.SpawnCustomComponent("Blocks/"+transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
        +".block");
    }
}
