using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public void LoadScene(int SceneID)
    {
        SceneManager.LoadScene(SceneID);
        Debug.Log("Loading scene: " + SceneID.ToString());
    }
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
        Debug.Log("Loading scene: " + SceneName);
    }
    public void Shutdown()
    {
        Application.Quit();
        Debug.Log("Application quitting...");
    }
}
