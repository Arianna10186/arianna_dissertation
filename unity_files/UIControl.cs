using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public QLearning qlearning;
    public TextMeshProUGUI qTableDisplay;
    // public GameObject uiPanel;
    public GameObject[] uiTabs;
    // notes tab
    public TMP_InputField idInput;
    public TMP_InputField dateInput;
    public TMP_InputField notesInput;
    // client info tab
    public Transform notesListParent;
    public GameObject noteButtonPrefab;
    public TextMeshProUGUI fileContentsDisplay;
    void Start()
    {
        ShowTab(0); // default to tab 1
    }
    public void ShowTab(int tabIndex)
    {
        for (int i = 0; i < uiTabs.Length; i++)
        {
            uiTabs[i].SetActive(i == tabIndex);
        }
    }
    public void SaveNotes()
    {
        string id = idInput.text.Trim();
        string date = dateInput.text.Trim();
        string notes = notesInput.text.Trim();

        string formattedDate = date.Replace("/", "_");
        string filename = $"{id}_{formattedDate}.txt";

        // file content
        string content = $"Client ID: {id}\nDate: {date}\n\nNotes:\n{notes}";

        // save path
        string folerPath = $"C:/Users/arian/My project (1)/Assets/{id}";
        Directory.CreateDirectory(folerPath);
        string savePath = $"{folerPath}/{filename}";

        // save file
        System.IO.File.WriteAllText(savePath, content);
        Debug.Log($"Notes saved to {savePath}");

    }
    public void LoadNotes()
    {
        string id = idInput.text.Trim();

        // load path
        string loadPath = $"C:/Users/arian/My project (1)/Assets/{id}";
        
        // Clear old buttons
        foreach (Transform child in notesListParent)
        {
            Destroy(child.gameObject);
        }

        if (!Directory.Exists(loadPath))
        {
            Debug.LogWarning("No folder found for this ID.");
            return;
        }
        string[] files = Directory.GetFiles(loadPath, "*.txt");
        foreach (string file in files)
        {
            GameObject newButton = Instantiate(noteButtonPrefab, notesListParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileNameWithoutExtension(file);

            Button btn = newButton.GetComponent<Button>();
            string filePath = file;

            btn.onClick.AddListener(() =>
            {
                string content = System.IO.File.ReadAllText(filePath);
                fileContentsDisplay.text = content;
            }
            );}
    }
}
