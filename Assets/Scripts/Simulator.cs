using System;
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
    Vector2 screenTopLeft, screenBottomRight;
    public static List<ConnectionLine> connectionLines;
    public static Simulator simulator;
    List<string> inputNames, outputNames;
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
    [SerializeField] Color guideLineColor, guideLineHitColor;
    [SerializeField] LayerMask inputNodeLayer;
    Coroutine coroutine;

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
        CheckScreenBounds();
    }

    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            mainCam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 3f, 20f);
            CheckScreenBounds();
        }
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
                    point = col.transform.position;
                    currConnectionPoints[currConnectionPoints.Count - 1] = point;
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
                        print("Setting");
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
            CheckScreenBounds();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toolsOpened = !toolsOpened;
            componentMenu.SetActive(toolsOpened);
            PlayerPrefs.SetInt("BarOpened",toolsOpened ? 1 : 0);
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Destroy(selectedObject);
            selectGraphic.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeMenu.SetActive(true);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            selectedObject.transform.Rotate(new Vector3(0f,0f,90f));
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
                Vector2 ctrlPoint, start, end;
                if(Mathf.Abs(point.x - lastPoint.x) > Mathf.Abs(point.y-lastPoint.y))
                {
                    ctrlPoint = new Vector2(point.x, lastPoint.y);
                    currConnectionPoints[currConnectionPoints.Count-1] = ctrlPoint;  
                    start = new Vector3(ctrlPoint.x, screenBottomRight.y);
                    end = new Vector3(ctrlPoint.x, screenTopLeft.y);
                }
                else
                {
                    ctrlPoint = new Vector2(lastPoint.x, point.y);
                    currConnectionPoints[currConnectionPoints.Count-1] = ctrlPoint;  
                    start = new Vector3(screenBottomRight.x, ctrlPoint.y);
                    end = new Vector3(screenTopLeft.x, ctrlPoint.y);
                }
                guideLine.SetPositions(new Vector3[] {start, end});
                RaycastHit2D hit = Physics2D.Raycast(start, end - start, Mathf.Infinity, inputNodeLayer);
                SetGuideLineColor(hit);
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
                    (ushort)connectionLine.inputIndex,
                    connectionLine.GetConnectionPoints()
                )
            );
        }
        Renamable[] renamables = FindObjectsOfType<Renamable>();
        foreach(Renamable renamable in renamables)
        {
            circuitData.renamedObjects.Add(circuitObjects.IndexOf(renamable.transform)+"^"+renamable.text.text);
        }
        LogicSwitch[] switches = FindObjectsOfType<LogicSwitch>();
        circuitData.camX = mainCam.transform.position.x;
        circuitData.camY = mainCam.transform.position.y;
        circuitData.size = mainCam.orthographicSize;
        SaveLoadSystem.Save(circuitData, currentFile);
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
        foreach(Connection connection in circuitData.connections)
        {
            GameObject newConnector = Instantiate(connector);
            ConnectionLine connectionLine = newConnector.GetComponent<ConnectionLine>();
            connectionLine.SetUp(circuitObjects[connection.outputObj].GetComponent<IOutputComponent>(), connection.outPutIndex,
            circuitObjects[connection.inputObject].GetComponent<IInputComponent>(), connection.inputIndex);
            Transform outputTransform = circuitObjects[connection.outputObj];
            connectionLine.SetTransforms(outputTransform.GetChild(outputTransform.childCount - connection.outPutIndex-1),
            circuitObjects[connection.inputObject].GetChild(connection.inputIndex));
            connectionLine.LoadConnectionPoints(connection.pathPoints);
            connectionLines.Add(connectionLine);
        }
        mainCam.transform.position = new Vector3(circuitData.camX, circuitData.camY, -10f);
        mainCam.orthographicSize = circuitData.size;
    }

    void SetGuideLineColor(bool detected)
    {
        guideLine.startColor = guideLine.endColor = detected ? guideLineHitColor : guideLineColor;
    }

    void CheckScreenBounds()
    {
        screenTopLeft = mainCam.ScreenToWorldPoint(new Vector3(0f, 0f));
        screenBottomRight = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
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
        Vector2 randomPos = new Vector2(mainCam.transform.position.x, mainCam.transform.position.y) + UnityEngine.Random.insideUnitCircle * 3f;
        GameObject spawned = Instantiate(prefabs[index], randomPos, Quaternion.identity);
        circuitObjects.Add(spawned.transform);
        ids.Add((ushort)index);
    }

    public void SpawnCustomComponent(string blockName)
    {
        Vector2 randomPos = new Vector2(mainCam.transform.position.x, mainCam.transform.position.y) + UnityEngine.Random.insideUnitCircle * 3f;
        GameObject spawned = Instantiate(prefabs[21], randomPos, Quaternion.identity);
        CustomComponent customComponent = spawned.GetComponent<CustomComponent>();
        BlockData loadedBlock = SaveLoadSystem.LoadBlock(blockName);
        customComponent.BuildBlock(loadedBlock);
        circuitObjects.Add(spawned.transform);
        ids.Add(21);
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

    public void SpawnCustomComponent(string name, float x,float y)
    {
        GameObject spawned = Instantiate(prefabs[21], new Vector3(x, y, 0f), Quaternion.identity);
        circuitObjects.Add(spawned.transform);
        ids.Add(21);
    }

    public void SynthesizeBlock()
    {
        LogicSwitch[] switches = FindObjectsOfType<LogicSwitch>();
        OutPutBulb[] outPutBulbs = FindObjectsOfType<OutPutBulb>();
        Array.Sort(outPutBulbs, (a,b) => b.transform.position.y.CompareTo(a.transform.position.y));
        Array.Sort(switches, (a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        coroutine = StartCoroutine(FindMinTerms(switches.Length,switches,outPutBulbs));
    }

    IEnumerator FindMinTerms(int bits, LogicSwitch[] switches, OutPutBulb[] outPutBulbs)
    {
        int term = 0;
        
        int final = (1 << bits) - 1; 
        
        List<List<int>> minTerms = new List<List<int>>();
        for (int i = 0; i < outPutBulbs.Length; i++) 
        {
            minTerms.Add(new List<int>());
        }

        while (term <= final)
        {
            for (int i = 0; i < switches.Length; i++)
            {
                bool bitState = ((term >> i) & 1) == 1;
                switches[i].SetState(bitState);
            }

            yield return null;
            yield return null;

            for (int i = 0; i < outPutBulbs.Length; i++)
            {
                if (outPutBulbs[i].GetState())
                {
                    minTerms[i].Add(term);
                }
            }
            term++;
        }

        BlockData blockData = new BlockData();
        string fileName = PlayerPrefs.GetString("CurrentFile");
        print(fileName);
        string blockName = fileName.Split(new char[] {'\\','.','/'})[1];
        blockData.origin = fileName;
        foreach(LogicSwitch logicSwitch in switches)
        {
            blockData.inputs.Add(logicSwitch.GetComponent<Renamable>().text.text);
        }
        foreach(OutPutBulb outPutBulb in outPutBulbs)
        {
            blockData.outputs.Add(outPutBulb.GetComponent<Renamable>().text.text);
        }
        foreach(List<int> minTerm in minTerms)
        {
            blockData.minTerms.Add(minTerm);
        }
        SaveLoadSystem.SaveBlockData(blockData,"Blocks/"+blockName+".block");
        StopCoroutine(coroutine);
        coroutine = null;
    }

    public static bool[] IntToBinaryLSB(int number, int bitCount)
    {
        // Handle invalid bit count inputs gracefully
        if (bitCount <= 0) return new bool[0];

        bool[] bits = new bool[bitCount];

        for (int i = 0; i < bitCount; i++)
        {
            if (i < 32) bits[i] = ((number >> i) & 1) == 1;
            else bits[i] = number < 0;
        }

        return bits;
    }

    IEnumerator ClockToggle()
    {
        while(true)
        {
            clockPulse = !clockPulse;

            float toggleTime = 1/(clockFrequency*2f);
            yield return new WaitForSeconds(toggleTime);
        }
    }
}
