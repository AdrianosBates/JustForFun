using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorPanel : MonoBehaviour {
    public MouseDrawing3D mouseDrawing3d;

    // Awake is called when the script instance is being loaded
    void Awake() {
        if (mouseDrawing3d == null) {
            Debug.LogError("No mousedrawing is assinged to UIColorPanel! Did you forget to assign it in the inspector?");
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void SetColorFromUI(GameObject buttonObject) {
        Color color = buttonObject.GetComponent<Image>().color;
        mouseDrawing3d.SetColorFromUI(color);
    }

    public void SetColorModeFromUI(int colorMode) {
        mouseDrawing3d.SetColorModeFromUI((LineToMesh.ColorMode) colorMode);
    }
}
