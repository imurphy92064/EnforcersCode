using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public int buttonWidth;
    public int buttonHeight;
    private int origin_x;
    private int origin_y;

    // Use this for initialization
    void Start()
    {
        Player.UnlockCursor();
        buttonWidth = 200;
        buttonHeight = 50;
        origin_x = Screen.width / 2 - buttonWidth / 2;
        origin_y = Screen.height / 2 - buttonHeight * 2;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(origin_x, origin_y, buttonWidth, buttonHeight), "Start Campaign"))
        {
            SceneLoading.loadScene("Level1");
        }
        if (GUI.Button(new Rect(origin_x, origin_y + buttonHeight + 10, buttonWidth, buttonHeight), "Exit"))
        {
            Quit();
        }
        if (GUI.Button(new Rect(origin_x, origin_y + buttonHeight * 2 + 20, buttonWidth, buttonHeight), "Level 1"))
        {
            SceneLoading.loadScene("Level1");
        }
        if (GUI.Button(new Rect(origin_x, origin_y + buttonHeight * 3 + 30, buttonWidth, buttonHeight), "Level 2"))
        {
            SceneLoading.loadScene("Level2");
        }
        if (GUI.Button(new Rect(origin_x, origin_y + buttonHeight * 4 + 40, buttonWidth, buttonHeight), "Level 3"))
        {
            SceneLoading.loadScene("Level3");
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}










