using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu2 : MonoBehaviour {

	void OnGUI() {
		if(GUI.Button(new Rect(0, 500, 100, 50), "Menu")) {
			SceneLoading.loadScene("Menu");
		}
	}
}
