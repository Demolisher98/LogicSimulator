using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Simulator : MonoBehaviour
{
    public static bool dragging = false;
    string currentFile;
    bool connecting = false, toolsOpened = true;
    Vector2 point, offset;
    Transform dragComponent, connectComponent;
    LineRenderer line;
    GameObject selectedObject;
    TextMeshProUGUI currentEditingText;
    IOutputComponent outputComponent;
    IInputComponent inputComponent;
    List<Vector3> currConnectionPoints;
    List<Transform> circuitObjects;
    List<ushort> ids;
    int conOutPutIndex;
    public static List<ConnectionLine> connectionLines;
    public static Simulator simulator;
    [HideInInspector]public bool clockPulse = false;
    [SerializeField]Transform selectGraphic;
    [SerializeField] string version = "1.0";
    [SerializeField] TextMeshProUGUI camPositionText;
    [SerializeField] Camera mainCam;
    [SerializeField] float linewidth = 0.1f, sensitivity = 1f;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] GameObject connector, componentMenu, escapeMenu, editor;
    [SerializeField] float clockFrequency = 1f, guideLineWidth = 0.01f;
    [SerializeField] LineRenderer guideLine;
    [SerializeField] Color guideLineColor;

    void Start()
    {
        currConnectionPoints = new List<Vector3>();
        if(simulator == null)
        {
            simulator = this;
        }
        else Destroy(gameObject);
        circuitObjects = new List<Transform>();
        connectionLines = new List<ConnectionLine>();
        ids = new List<ushort>();
        line = GetComponent<LineRenderer>();
        currentFile = PlayerPrefs.GetString("CurrentFile");
        Load(currentFile);
        Application.targetFrameRate = 60;
        line.startWidth = line.endWidth = linewidth;
        line.startColor = line.endColor = Color.black;
        guideLine.startWidth = guideLine.endWidth = guideLineWidth;
        guideLine.startColor = guideLine.endColor = guideLineColor;
        camPositionText.text = "<"+mainCam.transform.position.x.ToString("0")+","+mainCam.transform.position.y.ToString("0")+">";
        componentMenu.SetActive(PlayerPrefs.GetInt("BarOpened",1) == 1);
        StartCoroutine(ClockToggle());
    }

    void Update()
    {
        mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 3f, 20f);
        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            selectedObject = null;
            selectGraphic.gameObject.SetActive(false);
            point = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapCircle(point, 0.1f);
            if (col != null)
            {
                if (col.gameObject.CompareTag("Component"))
                {
                    line.enabled = true;
                    dragging = true;
                    offset = (Vector2)col.transform.position - point;
                    selectedObject = col.gameObject;
                    selectGraphic.transform.position = col.transform.position;
                    Vector3 scale;
                    scale = Vector3.Max(col.gameObject.GetComponent<BoxCollider2D>().size,col.transform.localScale);
                    selectGraphic.transform.localScale = scale + scale.normalized * 0.1f;
                    selectGraphic.gameObject.SetActive(true);
                    dragComponent = col.transform;
                }
                else if(col.gameObject.CompareTag("Connection"))
                {
                    selectedObject = col.gameObject;
                }
                else if (col.gameObject.CompareTag("OutputNode"))
                {
                    connectComponent = col.transform;
                    outputComponent = col.transform.parent.GetComponent<IOutputComponent>();
                    conOutPutIndex = int.Parse(col.gameObject.name);
                    line.positionCount = 1;
                    line.enabled = true;
                    point = col.transform.position;
                    currConnectionPoints.Add(point);
                    print(currConnectionPoints);
                    connecting = true;
                    print("connecting");
                }
                else if (col.gameObject.CompareTag("Switch"))
                {
                    LogicSwitch logicSwitch = col.transform.parent.GetComponent<LogicSwitch>();
                    logicSwitch.ToggleState();
                }
            }
            if(connecting)
            {
                currConnectionPoints.Add(point);
                
                line.positionCount = currConnectionPoints.Count;
                line.SetPositions(currConnectionPoints.ToArray());

                if (col != null && col.transform.parent != null && col.gameObject.CompareTag("InputNode"))
                {
                    if(col.gameObject.name.EndsWith('.'))
                    {
                        print("Connected to smt else");
                        return;
                    }
                    col.gameObject.name += ".";
                    inputComponent = col.transform.parent.GetComponent<IInputComponent>();
                    if (inputComponent != null)
                    {
                        connecting = false;
                        GameObject newConnector = Instantiate(connector);
                        ConnectionLine connectionLine = newConnector.GetComponent<ConnectionLine>();
                        connectionLine.SetUp(outputComponent, conOutPutIndex, inputComponent, int.Parse(col.gameObject.name.Split('.')[0]));
                        connectionLine.SetTransforms(connectComponent, col.transform);
                        connectionLine.SetConnectionPoints(currConnectionPoints);
                        connectionLines.Add(connectionLine);
                    }
                    connecting = false;
                    line.positionCount = 0;
                    line.enabled = false;
                    guideLine.enabled = false;
                    currConnectionPoints.Clear();
                }
            }
        }

        // Left Drag
        if (Input.GetMouseButton(0))
        {
            if (dragging)
            {
                Vector2 currentPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                dragComponent.position = currentPos + offset;
                if(Input.GetKey(KeyCode.LeftControl))
                {
                    dragComponent.position = new Vector3(Mathf.Round(dragComponent.position.x),Mathf.Round(dragComponent.position.y),0f);
                }
                selectGraphic.position = dragComponent.position;
            }
        }

        // Left Release
        if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                dragComponent = null;
                dragging = false;
            }
        }

        //Right Click
        if (Input.GetMouseButtonDown(1))
        {
            point = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapCircle(point, 0.1f);
            if (col != null)
            {
                if (col.GetComponent<Renamable>() != null)
                {
                    editor.SetActive(true);
                    currentEditingText = col.GetComponent<Renamable>().text;
                }
            }
            else editor.SetActive(false);

            if(connecting)
            {
                connecting = false;
                line.enabled = false;
                line.positionCount = 0;
                currConnectionPoints.Clear();
            }
        }

        //Right Drag
        if (Input.GetMouseButton(1))
        {
            mainCam.transform.position -= new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sensitivity;
            camPositionText.text = "<"+mainCam.transform.position.x.ToString("0")+","+mainCam.transform.position.y.ToString("0")+">";
        }

        //EnableMenu
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toolsOpened = !toolsOpened;
            componentMenu.SetActive(toolsOpened);
            PlayerPrefs.SetInt("BarOpened",toolsOpened ? 1 : 0);
        }

        //Press Delete
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(selectedObject);
            selectGraphic.gameObject.SetActive(false);
        }

        //Press Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeMenu.SetActive(true);
        }

        if(connecting)
        {
            point = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 lastPoint = currConnectionPoints[currConnectionPoints.Count-2];
            currConnectionPoints[currConnectionPoints.Count-1] = point;

            // Check whether mouse has moved more in x direction or y direction
            if(Input.GetKey(KeyCode.LeftControl))
            {
                guideLine.enabled = true;
                Vector2 ctrlPoint;
                if(Mathf.Abs(point.x - lastPoint.x) > Mathf.Abs(point.y-lastPoint.y))
                {
                    ctrlPoint = new Vector2(point.x, lastPoint.y);
                    currConnectionPoints[currConnectionPoints.Count-1] = ctrlPoint;  
                    guideLine.SetPositions(new Vector3[] {ctrlPoint + Vector2.down * 5f, ctrlPoint + Vector2.up * 5f});
                }
                else
                {
                    ctrlPoint = new Vector2(lastPoint.x, point.y);
                    currConnectionPoints[currConnectionPoints.Count-1] = ctrlPoint;  
                    guideLine.SetPositions(new Vector3[] {ctrlPoint + Vector2.left * 5f, ctrlPoint + Vector2.right * 5f}); 
                }
            }
            else guideLine.enabled = false;

            line.positionCount = currConnectionPoints.Count;
            line.SetPositions(currConnectionPoints.ToArray());
        }
    }

    public void Save()
    {
        CircuitData circuitData = new CircuitData();
        int i = 0;
        foreach (Transform obj in circuitObjects)
        {
            circuitData.objectIDs.Add(ids[i]);
            circuitData.xPos.Add(obj.position.x);
            circuitData.yPos.Add(obj.position.y);
            i++;
        }
        foreach(ConnectionLine connectionLine in connectionLines)
        {
            circuitData.connections.Add(
                new Connection(
                    (ushort)circuitObjects.IndexOf(connectionLine.outputNode.parent),
                    (ushort)connectionLine.outputIndex,
                    (ushort)circuitObjects.IndexOf(connectionLine.inputNode.parent),
                    (ushort)connectionLine.inputIndex
                )
            );
        }
        Renamable[] renamables = FindObjectsOfType<Renamable>();
        foreach(Renamable renamable in renamables)
        {
            circuitData.renamedObjects.Add(circuitObjects.IndexOf(renamable.transform)+"^"+renamable.text.text);
        }
        LogicSwitch[] switches = FindObjectsOfType<LogicSwitch>();
        foreach(LogicSwitch logicSwitch in switches)
        {
            if(logicSwitch.GetOutput(0))
            {
                circuitData.switchesOn.Add(circuitObjects.IndexOf(logicSwitch.transform));
            }
        }
        circuitData.camX = mainCam.transform.position.x;
        circuitData.camY = mainCam.transform.position.y;
        circuitData.size = mainCam.orthographicSize;
        SaveLoadSystem.Save(circuitData, currentFile);
        print("saved");
    }

    void Load(string fileName)
    {
        CircuitData circuitData = SaveLoadSystem.Load(fileName);
        for (int i = 0; i < circuitData.objectIDs.Count; i++)
        {
            SpawnItem(circuitData.objectIDs[i], circuitData.xPos[i], circuitData.yPos[i]);
        }
        foreach(string name in circuitData.renamedObjects)
        {
            string[] stripped = name.Split('^');
            int index = int.Parse(stripped[0]);
            circuitObjects[index].GetComponent<Renamable>().text.text = stripped[1];
        }
        foreach(int onIndex in circuitData.switchesOn)
        {
            circuitObjects[onIndex].GetComponent<LogicSwitch>().SetState(true);
        }
        foreach(Connection connection in circuitData.connections)
        {
            GameObject newConnector = Instantiate(connector);
            ConnectionLine connectionLine = newConnector.GetComponent<ConnectionLine>();
            connectionLine.SetUp(circuitObjects[connection.outputObj].GetComponent<IOutputComponent>(), connection.outPutIndex,
            circuitObjects[connection.inputObject].GetComponent<IInputComponent>(), connection.inputIndex);
            Transform outputTransform = circuitObjects[connection.outputObj];
            connectionLine.SetTransforms(outputTransform.GetChild(outputTransform.childCount - connection.outPutIndex-1),
            circuitObjects[connection.inputObject].GetChild(connection.inputIndex));
            connectionLines.Add(connectionLine);
        }
        mainCam.transform.position = new Vector3(circuitData.camX, circuitData.camY, -10f);
        mainCam.orthographicSize = circuitData.size;
    }

    public void Menu()
    {
        Save();
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        escapeMenu.SetActive(false);
    }

    public void SetText(string text)
    {
        currentEditingText.text = text;
    }

    public void SpawnItem(int index)
    {
        Vector2 randomPos = new Vector2(mainCam.transform.position.x, mainCam.transform.position.y) + Random.insideUnitCircle * 3f;
        GameObject spawned = Instantiate(prefabs[index], randomPos, Quaternion.identity);
        circuitObjects.Add(spawned.transform);
        ids.Add((ushort)index);
    }

    public void RemoveTransform(Transform obj)
    {
        int index = circuitObjects.IndexOf(obj);
        circuitObjects.RemoveAt(index);
        ids.RemoveAt(index);
    }

    public void SpawnItem(int index, float x, float y)
    {
        GameObject spawned = Instantiate(prefabs[index], new Vector3(x, y, 0f), Quaternion.identity);
        circuitObjects.Add(spawned.transform);
        ids.Add((ushort)index);
    }

    IEnumerator ClockToggle()
    {
        while(true && clockFrequency >= 1f)
        {
            clockPulse = !clockPulse;

            float toggleTime = 1/(clockFrequency*2f);
            yield return new WaitForSeconds(toggleTime);
        }
    }
}
