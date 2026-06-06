using UnityEngine;

public class SafeDestroyer : MonoBehaviour
{
    Simulator simulator;

    void Start()
    {
        simulator = FindAnyObjectByType<Simulator>();
    }

    void OnDestroy()
    {
        simulator.RemoveTransform(transform);
    }
}