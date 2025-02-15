using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LVManager : Singleton<LVManager>
{
   // Start is called before the first frame update
    // Start is called before the first frame update
    public List<string> sceneNames = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        LoadSavedScenes();

    }

    public void SaveGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (!sceneNames.Contains(currentSceneName))
        {
            sceneNames.Add(currentSceneName);
            SaveScenesToPrefs();

        }
    }

    private void SaveScenesToPrefs()
    {
        // Chuyển danh sách thành chuỗi phân cách bởi dấu phẩy
        string sceneNamesString = string.Join(",", sceneNames);
        PlayerPrefs.SetString("SavedScenes", sceneNamesString);
        PlayerPrefs.Save();
    }

    private void LoadSavedScenes()
    {
        string savedScenes = PlayerPrefs.GetString("SavedScenes", string.Empty);
        if (!string.IsNullOrEmpty(savedScenes))
        {
            sceneNames = new List<string>(savedScenes.Split(','));
            Debug.Log("Loaded saved scenes: " + string.Join(", ", sceneNames));
        }
    }
}

[System.Serializable]
public class SceneListWrapper
{
    public List<string> scenes;
}