using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnObjectMeshOnly : MonoBehaviour {

    private MeshFilter meshFilter;

    private float lineThickness;

    // Awake is called when the script instance is being loaded
    void Awake() {
        meshFilter = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void ClearMesh() {
        meshFilter.mesh = null;
    }

    public void InitObject(float lineThickness) {
        this.lineThickness = lineThickness;
    }

    public void RegenerateObject(List<Vector3> line, List<Vector3> nonShiftedLine, LineToMesh.ColorMode colorMode, Color color) {
        UpdateMesh(line, nonShiftedLine, colorMode, color);
    }

    private void UpdateMesh(List<Vector3> line, List<Vector3> nonShiftedLine, LineToMesh.ColorMode colorMode, Color color) {
        var newMesh = LineToMesh.CreateMeshFromLine(line, nonShiftedLine, lineThickness, colorMode, color);
        meshFilter.mesh = newMesh;
    }
}
