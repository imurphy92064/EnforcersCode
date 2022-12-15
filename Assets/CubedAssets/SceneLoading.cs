using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading
{
    public static void loadScene(string sceneName)
    {
        Globals.hasRedKey = false;
        Globals.hasGreenKey = false;
        Globals.hasBlueKey = false;

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        Player.LockCursor();
    }
}
