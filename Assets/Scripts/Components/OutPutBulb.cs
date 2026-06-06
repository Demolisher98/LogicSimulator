using UnityEngine;

public class OutPutBulb : MonoBehaviour, IInputComponent
{
    [SerializeField] SpriteRenderer sprite;

    public void SetInput(int index,bool state)
    {
        sprite.color = state ? Color.red : Color.black;
    }

    public bool GetState()
    {
        return sprite.color == Color.red;
    }
}
