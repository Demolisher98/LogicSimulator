using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuScript : MonoBehaviour
{
    string[] files;
    [SerializeField] string version;
    [SerializeField] GameObject openFilesCanvas, openCompsCanvas, filesList, fileNameStrip, backBtn, createFileCanvas;
    [SerializeField] TMP_InputField newFileField;
    List<GameObject> strips;

    void Start()
    {
        strips = new List<GameObject>();
        MakeFilesList();
    }

    public void OpenFiles()
    {
        openFilesCanvas.SetActive(true);
        backBtn.SetActive(true);
    }

    public void OpenComponents()
    {
        openFilesCanvas.SetActive(true);
        backBtn.SetActive(true);
    }

    public void Back()
    {
        openFilesCanvas.SetActive(false);
        openCompsCanvas.SetActive(false);
        backBtn.SetActive(false);
    }

    public void OpenCreateFileMenu()
    {
        createFileCanvas.SetActive(true);
    }
    
    public void CreateNewFile()
    {
        if (!File.Exists("Circuits/" + newFileField.text + ".ckt"))
        {
            CircuitData circuitData = new CircuitData();
            SaveLoadSystem.Save(circuitData, "Circuits/" + newFileField.text + ".ckt");
        }
        else
        {
            print("already exists");
            return;
        }
        StartSimulation("Circuits/" + newFileField.text + ".ckt");
    }

    public void DeleteFile(string fileName)
    {
        print("Deleting "+fileName);
        File.Delete("Circuits/" + fileName + ".ckt");
        MakeFilesList();
    }

    void MakeFilesList()
    {
        foreach(GameObject strip in strips)
        {
            Destroy(strip);
        }
        files = Directory.GetFiles("Circuits");
        foreach (string fileName in files)
        {
            GameObject newStrip = Instantiate(fileNameStrip);
            newStrip.transform.SetParent(filesList.transform, false);
            newStrip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = fileName.Split('\\', '.')[1];
            strips.Add(newStrip);
            newStrip.GetComponent<Button>().onClick.AddListener(
                () => StartSimulation(fileName)
            );
            newStrip.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(
                () => DeleteFile(fileName.Split('\\','.')[1])
            );
        }
    }
    
    void StartSimulation(string fileName)
    {
        PlayerPrefs.SetString("CurrentFile", fileName);
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
