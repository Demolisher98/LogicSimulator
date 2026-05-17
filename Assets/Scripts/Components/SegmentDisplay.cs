using UnityEngine;

public class SegmentDisplay : MonoBehaviour, IInputComponent
{
    [SerializeField] SpriteRenderer[] segments;
    [SerializeField] Color offColor;
    bool[] inputs = {false,false,false,false};

    public void SetInput(int index, bool state)
    {
        inputs[index] = state;
    }

    void Update()
    {
        Evaluate();
    }

    void Evaluate()
    {
        bool A = inputs[0];
        bool B = inputs[1];
        bool C = inputs[2];
        bool D = inputs[3];

        // Segment a
        segments[0].color = A || C || (B && D) || (!B && !D) ? Color.red : offColor;

        // Segment b
        segments[1].color = !B || (C && D) || (!C && !D) ? Color.red : offColor;

        // Segment c
        segments[2].color = B || !C || D ? Color.red : offColor;

        // Segment d
        segments[3].color = A
            || (C && !D)
            || (!B && !D)
            || (!B && C)
            || (B && !C && D) ? Color.red : offColor;

        // Segment e
        segments[4].color= !D && (!B || C) ? Color.red : offColor;

        // Segment f
        segments[5].color = A
            || (!C && !D)
            || (B && !C)
            || (B && !D) ? Color.red : offColor;

        // Segment g
        segments[6].color = A
            || (B && !C)
            || (!B && C)
            || (C && !D) ? Color.red : offColor;
    }
}
