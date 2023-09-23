using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawnObject : MonoBehaviour {
    public float minColliderStretchDistance = 0.2f;

    public GameObject circlePrefab;
    public GameObject boxPrefab;

    public Material doneDrawingMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private CompositeCollider2D compositeCollider;
    private Rigidbody2D body;

    private float lineThickness;

    // Awake is called when the script instance is being loaded
    void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        compositeCollider = GetComponent<CompositeCollider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void InitObject(float lineThickness) {
        this.lineThickness = lineThickness;
    }

    public void RegenerateObject(List<Vector3> line, List<Vector3> nonShiftedLine,LineToMesh.ColorMode colorMode, Color color) {
        UpdateMesh(line, nonShiftedLine, colorMode, color);
        InstantiateColliders(line);
    }

    public void ReleaseObject() {
        int childCount = transform.childCount;
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        body.gravityScale = 1;
        body.constraints = RigidbodyConstraints2D.None;
        //body.mass = childCount;

        var newLayer = LayerMask.NameToLayer("DoneDrawing");
        gameObject.layer = newLayer;

        meshRenderer.material = doneDrawingMaterial;
    }

    private void UpdateMesh(List<Vector3> line, List<Vector3> nonShiftedLine, LineToMesh.ColorMode colorMode, Color color) {
        var newMesh = LineToMesh.CreateMeshFromLine(line, nonShiftedLine, lineThickness, colorMode, color);
        meshFilter.mesh = newMesh;
    }


    private void InstantiateColliders(List<Vector3> line) {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        var colliderScaling = lineThickness * 2 * 0.925f;

        circlePrefab.transform.localScale = new Vector3(colliderScaling, colliderScaling, 1);
        boxPrefab.transform.localScale = new Vector3(colliderScaling, colliderScaling, 1);

        //for (int i = 0; i < line.Count; i++) {
        //    circlePrefab.transform.localScale = new Vector3(lineThickness * 2, lineThickness * 2, 1);
        //    var circ = Instantiate(circlePrefab, line[i], Quaternion.identity, this.transform);
        //}
        //compositeCollider.GenerateGeometry();

        var minBoxDistance = minColliderStretchDistance;
        Instantiate(circlePrefab, line[0] + transform.position, Quaternion.identity, this.transform);

        for (int i = 1; i < line.Count; i++) {
            var currentPos = line[i] + transform.position;
            var lastPos = line[i - 1] + transform.position;

            var distanceFromLast = Vector2.Distance(currentPos, lastPos);
            var between = Vector2.Lerp(lastPos, currentPos, 0.5f);

            if (distanceFromLast >= minColliderStretchDistance) { //&& CanStartDrawing() && CanDrawLine(currentPos, lastPos)) {

                if (distanceFromLast >= minBoxDistance) {
                    var connectingBox = Instantiate(boxPrefab, currentPos, Quaternion.identity, this.transform);
                    var newScale = connectingBox.transform.localScale;
                    var newRot = connectingBox.transform.rotation;
                    var newPos = connectingBox.transform.position;
                    newRot = Quaternion.FromToRotation(Vector3.right, lastPos - newPos);
                    newScale.x = distanceFromLast;
                    newPos = between;
                    connectingBox.transform.localScale = newScale;
                    connectingBox.transform.rotation = newRot;
                    connectingBox.transform.position = newPos;

                    Instantiate(circlePrefab, currentPos, Quaternion.identity, this.transform);
                } else {
                    Instantiate(circlePrefab, currentPos, Quaternion.identity, this.transform);
                }
            }
        }

        Physics2D.SyncTransforms();
        compositeCollider.GenerateGeometry();
    }
}
