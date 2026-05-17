using UnityEngine;

public class FlipFlop : MonoBehaviour, IInputComponent, IOutputComponent
{
    bool Q = false;
    bool[] inputs = {false, false, false};
    bool lastPulse;
    public enum FFType {SR, JK ,D ,T};
    [SerializeField] FFType type;

    public bool GetOutput(int index)
    {
        return index == 0 ? Q : !Q;
    }

    void Update()
    {
        if(inputs[2] == true && lastPulse == false)Evaluate();
        lastPulse = inputs[2];
    }

    public void SetInput(int index, bool state)
    {
        inputs[index] = state;
    }
    
    void Evaluate()
    {
        switch(type)
        {
            case FFType.SR:
                Q = inputs[0] | !inputs[1] & Q;
                    break;
            case FFType.JK:
                Q = (inputs[0] & !Q) | (!inputs[1] & Q);
                    break;
            case FFType.D:
                Q = inputs[0];
                    break;
            case FFType.T:
                Q = inputs[0] ^ Q;
                    break;
        }
    }
}
