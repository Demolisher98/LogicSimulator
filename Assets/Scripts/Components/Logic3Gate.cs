using UnityEngine;

public class Logic3Gate : MonoBehaviour, IInputComponent, IOutputComponent
{
    public enum Gate {AND,OR,NAND,NOR};
    public bool[] inputs;
    bool output;
    [SerializeField] Gate gate;

    void Start()
    {
        Evaluate();
    }

    void OnDestroy()
    {
        output = false;
    }

    public void Evaluate()
    {
        switch (gate)
        {
            case Gate.AND:
                output = inputs[0] & inputs[1] & inputs[2];
                break;
            case Gate.OR:
                output = inputs[0] | inputs[1] | inputs[2];
                break;
            case Gate.NAND:
                output = !(inputs[0] && inputs[1] && inputs[2]);
                break;
            case Gate.NOR:
                output = !(inputs[0] & inputs[1] & inputs[2]);
                break;
        }
    }

    public void SetInput(int index,bool state)
    {
        inputs[index] = state;
        Evaluate();
    }

    public void SetGate(Gate gate)
    {
        this.gate = gate;
    }

    public bool GetOutput(int index)
    {
        return output;
    }
}
