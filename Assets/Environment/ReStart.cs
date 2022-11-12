using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReStart : MonoBehaviour
{
    private int origin_x;
	private int origin_y;
    public int buttonWidth;
	public int buttonHeight;

    // Start is called before the first frame update
    void Start()
    {
        buttonWidth = 200;
		buttonHeight = 50;
        origin_x = Screen.width / 2;
		origin_y = Screen.height / 2 ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI(){
        if(GUI.Button(new Rect(origin_x-120, origin_y, 200, 50), "Restart")){
           SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    public void restartGame(string name)
    {
        Application.LoadLevel(name);
    }



    }
