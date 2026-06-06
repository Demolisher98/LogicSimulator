using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO; // Highly recommended for clean text UI

public class SidebarUpdater : MonoBehaviour
{

    [System.Serializable]
    public struct TrayData
    {
        public string trayName;
        public List<string> buttons;
    }
    [SerializeField] Simulator simulator;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject trayPrefab;   // Panel with Black Background and Header Text
    [SerializeField] private GameObject trayContainerPrefab;
    [SerializeField] private GameObject buttonPrefab; // Standard Button Prefab

    [Header("Layout Settings")]
    [SerializeField] private Transform sidebarContentContainer; // The Scroll View Content or Vertical Layout Group
    [SerializeField] private Transform scrollContent;

    [Header("Sidebar Configuration")]
    [Tooltip("Add, remove, or reorder your trays and buttons right here!")]
    [SerializeField] private List<TrayData> sidebarTrays = new List<TrayData>();
    string[] files;
    float size = 100f;
    int index;
    public void BuildSidebar()
    {
        foreach(TrayData trayData in sidebarTrays)
        {
            GameObject spawnedStrip = Instantiate(trayPrefab, sidebarContentContainer);
            spawnedStrip.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = trayData.trayName;
            GameObject buttonContainer = Instantiate(trayContainerPrefab, sidebarContentContainer);
            float containerSize = 30f * Mathf.Ceil(trayData.buttons.Count / 3f) + 10f;
            RectTransform rect = buttonContainer.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.localScale.x, containerSize);
            foreach(string buttonName in trayData.buttons)
            {
                GameObject button = Instantiate(buttonPrefab, buttonContainer.transform);
                SpawnerBtn spawnerBtn = button.GetComponent<SpawnerBtn>();
                spawnerBtn.index = index++;
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buttonName;
                if(index >= 22)
                {
                    Button btn = button.GetComponent<Button>();
                    spawnerBtn.isCustom = true;
                    btn.onClick.AddListener(spawnerBtn.SpawnCustomComponent);
                }
            }
            size += containerSize;
        }
        RectTransform scrollRect = scrollContent.GetComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(scrollRect.sizeDelta.x, size + 100f);
    }

    private void Start()
    {
        files = Directory.GetFiles("Blocks");
        foreach(string file in files)
        {
            print(file);
            sidebarTrays[sidebarTrays.Count-1].buttons.Add(file.Split(new char[] {'\\','.'})[1]);
        }
        BuildSidebar();
    }
}