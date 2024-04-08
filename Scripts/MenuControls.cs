using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public void PlayPressed()
    {
        //SceneManager.UnloadSceneAsync("Menu", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        SceneManager.LoadScene("Scene2 2"
            //, LoadSceneMode.Additive
            );
        //SceneManager.UnloadScene("Menu");
        //EditorSceneManager.CloseScene(SceneManager.GetSceneByName("Menu"), true);
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit pressed!");
    }
}
