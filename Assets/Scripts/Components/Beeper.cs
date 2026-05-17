using UnityEngine;

public class Beeper : MonoBehaviour, IInputComponent
{
    [SerializeField]AudioSource source;

    public void SetInput(int index, bool state)
    {
        source.enabled = state;
    }
}
