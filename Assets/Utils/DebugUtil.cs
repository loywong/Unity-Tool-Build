using UnityEngine;

public class DebugUtil : MonoBehaviour {
    // Start is called before the first frame update
    void Start () {

    }

    void OnGUI () {
        if (GameSettings._instance.isBuildPerfTestPackage) {
            Debug.Log ("GameSettings._instance.isBuildPerfTestPackage");
            return;
        }

        GUI.skin.textField.fontSize = 36;
        GUI.skin.button.fontSize = 36;
        GUI.backgroundColor = Color.green;

        // GUI.Label(new Rect(0, 300,  400, 60), "ScreenClickSequenceTrigger");
        if (GUI.Button (new Rect (0, 300, 700, 60), "ScreenClickSequenceTrigger")) { }
    }
}