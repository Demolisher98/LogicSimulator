using TMPro;
using UnityEngine;

public class CustomComponent : MonoBehaviour, IInputComponent, IOutputComponent
{
    [HideInInspector] public string fileName;
    [SerializeField] TextMeshProUGUI componentName;

    void LoadComponent()
    {
        
    }

    public bool GetOutput(int index)
    {
        throw new System.NotImplementedException();
    }

    public void SetInput(int index, bool state)
    {
        throw new System.NotImplementedException();
    }
}
