using TMPro;
using UnityEngine;

public class LogicSwitch : MonoBehaviour, IOutputComponent
{
    enum InputType {Switch,Clock};
    bool state = false;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] InputType type;
    public TextMeshProUGUI nameText;

    void Update()
    {
        if(type == InputType.Clock)
        {
            SetState(Simulator.simulator.clockPulse);
        }
    }

    public void SetState(bool state)
    {
        this.state = state;
        sprite.color = state ? Color.red : Color.black;
    }

    public void ToggleState()
    {
        state = !state;
        sprite.color = state ? Color.red : Color.black;
    }

    public bool GetOutput(int index)
    {
        return state;
    }
}