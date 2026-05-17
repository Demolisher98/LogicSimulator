using UnityEngine;

public class SpawnerBtn : MonoBehaviour
{
    [HideInInspector] public int index;

    public void SpawnItem()
    {
        Simulator.simulator.SpawnItem(index);
    }
}
