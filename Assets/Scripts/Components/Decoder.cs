using UnityEngine;

public class Decoder : MonoBehaviour, IInputComponent, IOutputComponent
{
    bool[] inputs;
    bool[] outputs;

    void Start()
    {
        inputs = new bool[3];
        outputs = new bool[8];
    }

    int isTrue(bool state)
    {
        return state ? 1 : 0;
    }

    public void SetInput(int index, bool state)
    {
        inputs[index] = state;
    }

    public bool GetOutput(int index)
    {
        for(int i = 0;i < 8;i++)
        {
            outputs[i] = false;
        }
        outputs[1*isTrue(inputs[0])+2*isTrue(inputs[1])+4*isTrue(inputs[2])] = true;
        return outputs[index];
    }
}
