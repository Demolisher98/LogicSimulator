using UnityEngine;

public class Encoder : MonoBehaviour, IInputComponent, IOutputComponent
{
    bool[] inputs;
    bool[] outputs;

    void Start()
    {
        inputs = new bool[8];
        outputs = new bool[3];
    }

    public void SetInput(int index, bool state)
    {
        inputs[index] = state;
        outputs[0] = inputs[1] | inputs[3] | inputs[5] | inputs[7];
        outputs[1] = inputs[2] | inputs[3] | inputs[6] | inputs[7];
        outputs[2] = inputs[4] | inputs[5] | inputs[6] | inputs[7];
    }

    public bool GetOutput(int index)
    {
        return outputs[index];
    }
}
