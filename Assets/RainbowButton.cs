using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainbowButton : MonoBehaviour {
    public float speed;
    private float rainbowTimer;
    private Image image;
    // Awake is called when the script instance is being loaded
    void Awake() {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        rainbowTimer = Mathf.Repeat(rainbowTimer + Time.deltaTime * speed, 1.0f);
        image.color = Color.HSVToRGB(Mathf.Lerp(0, 1, rainbowTimer), 1, 1);
    }
}
