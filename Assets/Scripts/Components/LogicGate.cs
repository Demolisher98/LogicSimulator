using UnityEngine;

public class LogicGate : MonoBehaviour, IInputComponent, IOutputComponent
{
    public enum Gate {NOT,AND,OR,NAND,NOR,XOR,XNOR};
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
            case Gate.NOT:
                output = !inputs[0];
                break;
            case Gate.AND:
                output = inputs[0] & inputs[1];
                break;
            case Gate.OR:
                output = inputs[0] | inputs[1];
                break;
            case Gate.NAND:
                output = !(inputs[0] && inputs[1]);
                break;
            case Gate.NOR:
                output = !(inputs[0] | inputs[1]);
                break;
            case Gate.XOR:
                output = inputs[0] ^ inputs[1];
                break;
            case Gate.XNOR:
                output = !(inputs[0] ^ inputs[1]);
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