using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu2 : MonoBehaviour {

	void OnGUI() {
		if(GUI.Button(new Rect(30, 30, 100, 50), "Menu")) {
			Application.LoadLevel(1);
		}
	}
}
