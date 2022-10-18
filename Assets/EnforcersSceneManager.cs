using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnforcersSceneManager: MonoBehaviour
{
    private static string currentScene = "";

    public static void changeScene(string sceneName)
    {
        currentScene = sceneName;
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    public static void restartScene()
    {
        SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
    }

    public static string getSceneName()
    {
        return currentScene;
    }
}
