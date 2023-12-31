using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScreen : MonoBehaviour {
    // Awake is called when the script instance is being loaded
    void Awake() {

    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)) {
            Clear();
        }
    }

    public void Clear() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}
