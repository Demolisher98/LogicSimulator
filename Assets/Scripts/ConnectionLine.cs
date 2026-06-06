using UnityEngine;
using System.Collections.Generic;

public class ConnectionLine : MonoBehaviour
{
    LineRenderer line;
    IOutputComponent outputComponent;
    IInputComponent inputComponent;
    List<Vector3> points;
    [SerializeField] float linewidth = 0.5f;
    [SerializeField] EdgeCollider2D edgeCollider2D;
    bool initialDraw = false;
    [HideInInspector]
    public int outputIndex, inputIndex;
    [HideInInspector]
    public Transform outputNode, inputNode;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = line.endWidth = linewidth;
        line.startColor = line.endColor = Color.black;
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    public void SetUp(IOutputComponent outputComponent, int oIndex, IInputComponent inputComponent, int iIndex)
    {
        this.outputComponent = outputComponent;
        outputIndex = oIndex;
        this.inputComponent = inputComponent;
        inputIndex = iIndex;
    }
    
    public void SetTransforms(Transform a, Transform b)
    {
        outputNode = a;
        inputNode = b;
    }

    void Update()
    {
        if (outputNode == null || inputNode == null)
        {
            Simulator.connectionLines.Remove(this);
            Destroy(gameObject);
        }
        bool output = outputComponent.GetOutput(outputIndex);
        inputComponent.SetInput(inputIndex, output);
        line.startColor = line.endColor = output ? Color.red : Color.black;
        if (!initialDraw)
        {
            DrawLine();
            initialDraw = true;
        }
        if(Simulator.dragging)
        {
            points[0] = outputNode.position;
            points[points.Count-1] = inputNode.position;
            DrawLine();
        }
    }

    void OnDestroy()
    {
        Simulator.connectionLines.Remove(this);
        if(inputNode != null)
        {
            inputNode.gameObject.name = inputNode.gameObject.name.Split('.')[0];
            if(inputComponent != null)inputComponent.SetInput(inputIndex,false);
        }
    }

    void DrawLine()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
        SetEdgeCollider(line);
    }

    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();
        Vector3 dir = (lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0)).normalized;
        edges.Add(lineRenderer.GetPosition(0) + dir * 0.5f);

        for(int point = 1; point<lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(lineRendererPoint);
        }
        edgeCollider2D.SetPoints(edges);
    }

    public void SetConnectionPoints(List<Vector3> points)
    {
        this.points = new List<Vector3>();
        for(int i = 0;i < points.Count-1;i++)
        {
            this.points.Add(points[i]);
        }
        this.points[this.points.Count - 1] = inputNode.position;
        DrawLine();
    }

    public void LoadConnectionPoints(List<SerializableVector> pathPoints)
    {
        points = new List<Vector3>
        {
            outputNode.position
        };
        foreach(SerializableVector pathPoint in pathPoints)
        {
            points.Add(new Vector3(pathPoint.x, pathPoint.y));
        }
        points.Add(inputNode.position);
    }

    public List<SerializableVector> GetConnectionPoints()
    {
        List<SerializableVector> points = new List<SerializableVector>();
        for(int i = 1;i < this.points.Count-1;i++)
        {
            points.Add(new SerializableVector(this.points[i].x,this.points[i].y));
        }
        return points;
    }
}
