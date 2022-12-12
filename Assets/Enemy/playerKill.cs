using TMPro;
using UnityEngine;

public class playerKill : MonoBehaviour {
	public ReStart restart;

	public TextMeshProUGUI end;

	// Start is called before the first frame update
	private void Start() {
		restart.enabled = false;
		restart = GameObject.Find("Canvas/Restart").GetComponent<ReStart>();
	}

	// Update is called once per frame
	private void Update() { }

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			end.text = "You Lose! Click On Menu Button To Return!";
			restart.enabled = true;
		}
	}
}