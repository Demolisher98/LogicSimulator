using UnityEngine;

public class EdgeDetector : MonoBehaviour, IInputComponent, IOutputComponent
{
    bool edge = false, lastState;

    void Update()
    {
        if(edge)edge = false;
    }
    
    public bool GetOutput(int index)
    {
        return edge;
    }

    public void SetInput(int index, bool state)
    {
        if (state && lastState == false)
        {
            edge = true;
        }
        lastState = state;
    }
}
