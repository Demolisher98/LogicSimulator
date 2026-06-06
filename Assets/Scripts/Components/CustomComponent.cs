using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomComponent : MonoBehaviour, IInputComponent, IOutputComponent
{
    [SerializeField] GameObject inputNode, outputNode, line;
    [SerializeField] float nodeX, lineX;
    [SerializeField] List<List<int>> minTerms = new List<List<int>>();
    [SerializeField] TextMeshProUGUI componentNameText;
    public string componentName;
    public List<string> inputsNames;
    public List<string> outputsNames;
    bool[] inputs, outputs;

    public void BuildBlock(BlockData blockData)
    {
        componentNameText.text = blockData.origin.Split(new char[]{'\\','.'})[1];
        inputs = new bool[blockData.inputs.Count];
        outputs = new bool[blockData.outputs.Count];
        for(int i = 0;i < inputs.Length;i++)
        {
            inputs[i] = false;
        }
        for(int i = 0;i < outputs.Length;i++)
        {
            outputs[i] = false;
        }
        float distFromTop = 1f/(inputs.Length+1);
        for(int i = 1;i <= inputs.Length;i++)
        {
            GameObject inputNode = Instantiate(this.inputNode,transform);
            inputNode.name = (i-1).ToString();
            inputNode.transform.localPosition = new Vector3(nodeX,0.5f - distFromTop * i);
            inputNode.transform.SetSiblingIndex(i-1);
        }
        for(int i = 1;i <= inputs.Length;i++)
        {
            GameObject inputLine = Instantiate(line,transform);
            inputLine.transform.localPosition = new Vector3(lineX,0.5f - distFromTop * i);
        }
        distFromTop = 1f/(outputs.Length+1);
        for(int i = 1;i <= outputs.Length;i++)
        {
            GameObject inputLine = Instantiate(line,transform);
            inputLine.transform.localPosition = new Vector3(-lineX,0.5f - distFromTop * i);
        }
        for(int i = outputs.Length;i >= 1;i--)
        {
            GameObject outputNode = Instantiate(this.outputNode,transform);
            outputNode.name = (i-1).ToString();
            outputNode.transform.localPosition = new Vector3(-nodeX,0.5f - distFromTop * i);
            outputNode.transform.SetAsLastSibling();
        }
        minTerms = blockData.minTerms;
    }

    public void Evaluate()
    {
        int term = 0;
        
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i])
            {
                term |= 1 << i;
            }
        }
        
        for(int i = 0;i < outputs.Length;i++)
        {
            outputs[i] = minTerms[i].Contains(term);
        }
    }

    public bool GetOutput(int index)
    {
        return outputs[index];
    }

    public void SetInput(int index, bool state)
    {
        inputs[index] = state;
        Evaluate();
    }
}