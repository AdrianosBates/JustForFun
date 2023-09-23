using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDrawing3D : MonoBehaviour {
    public GameObject meshObjectPrefab;
    public GameObject meshGhostPrefab;

    public LineToMesh.ColorMode colorMode;
    public Color color;

    private List<Vector3> currentLine;

    private GameObject currentObject;
    private DrawnObject currentDrawnObject;

    private GameObject currentGhostObject;
    private DrawnObjectMeshOnly currentGhostDrawnObject;

    public float minDistance = 0.1f;
    public float lineThickness = 0.2f;

    private float zLayer = 10f;
    private float epsilon = 0.000001f;
    private int objectId = 0;

    // Awake is called when the script instance is being loaded
    void Awake() {
        currentLine = new List<Vector3>();
        currentGhostObject = Instantiate(meshGhostPrefab);
        currentGhostDrawnObject = currentGhostObject.GetComponent<DrawnObjectMeshOnly>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonDown(0)) {
                if (CanStartDrawing()) {
                    CreateNewObject();
                    AddLinePoint();
                } else {
                    DrawGhost();
                }
            }
        }

        if (currentObject != null) {
            if (Input.GetMouseButton(0)) {
                if (!AddLinePoint()) {
                    DrawGhost();
                } else {
                    ClearGhost();
                }
            } else if (Input.GetMouseButtonUp(0)) {
                AddLinePoint();
                ReleaseObject();
                ClearGhost();
            }
        } else {
            if (Input.GetMouseButtonUp(0)) {
                ClearGhost();
            }
        }
    }

    void CreateNewObject() {
        //currentLine = new List<Vector3>();

        currentObject = Instantiate(meshObjectPrefab, GetMouseWorldPoint(zLayer), Quaternion.identity, transform);
        //currentObject = Instantiate(meshObjectPrefab, Vector3.zero, Quaternion.identity, transform);
        currentObject.name = "Object " + objectId;
        objectId += 1;
        currentDrawnObject = currentObject.GetComponent<DrawnObject>();
        currentDrawnObject.InitObject(lineThickness);
        zLayer -= epsilon;
    }

    void ReleaseObject() {
        currentDrawnObject.ReleaseObject();
        currentObject = null;
        currentLine.Clear();
    }

    bool AddLinePoint() {
        var worldPos = GetMouseWorldPoint(zLayer);
        var center = GetLineCenterPoint(currentLine);
        //worldPos -= currentDrawnObject.transform.position;

        if (currentLine.Count > 0) {
            if (CanDrawLine(currentLine[currentLine.Count - 1], worldPos)) {
                var distance = Vector3.Distance(currentLine[currentLine.Count - 1], worldPos);
                if (distance > minDistance) {
                    currentLine.Add(worldPos);


                    var smoothedLine = SmoothLine(currentLine);
                    var shiftedLine = ShiftLine(smoothedLine, center);

                    currentDrawnObject.gameObject.transform.position = center;
                    currentDrawnObject.RegenerateObject(shiftedLine, smoothedLine, colorMode, color);
                }
            } else {
                return false;
            }
        } else {
            currentLine.Add(worldPos);

            var smoothedLine = SmoothLine(currentLine);
            var shiftedLine = ShiftLine(smoothedLine, currentDrawnObject.transform.position);
            
            currentDrawnObject.RegenerateObject(shiftedLine, smoothedLine, colorMode, color);
        }

        return true;
    }

    void DrawGhost() {
        currentGhostDrawnObject.InitObject(lineThickness);

        if (currentLine.Count == 0) {
            List<Vector3> points = new List<Vector3>();
            points.Add(GetMouseWorldPoint(zLayer - epsilon));

            currentGhostDrawnObject.RegenerateObject(points, points, colorMode, color);
        } else {
            List<Vector3> points = new List<Vector3>();
            points.Add(currentLine[currentLine.Count - 1]);
            points.Add(GetMouseWorldPoint(zLayer - epsilon));

            currentGhostDrawnObject.RegenerateObject(points, points, colorMode, color);
        }
    }

    void ClearGhost() {
        currentGhostDrawnObject.ClearMesh();
    }

    private bool CanStartDrawing() {
        LayerMask mask = LayerMask.GetMask("DoneDrawing");

        if (Physics2D.OverlapCircleAll(GetMouseWorldPoint(zLayer), lineThickness, mask).Length > 0) {
            //print("Cant Draw");
            return false;
        }
        return true;
    }

    private bool CanDrawLine(Vector2 a, Vector2 b) {
        LayerMask mask = LayerMask.GetMask("DoneDrawing");

        var distance = Vector2.Distance(a, b);
        var between = Vector2.Lerp(a, b, 0.5f);
        var angle = Quaternion.FromToRotation(Vector3.right, a - b).eulerAngles.z;

        if (Physics2D.OverlapBoxAll(between, new Vector2(distance, lineThickness * 2f), angle, mask).Length > 0) {
            //print("Cant Draw");
            return false;
        }
        return true;
    }

    List<Vector3> SmoothLine(List<Vector3> line) {
        var displacementPercentage = 1f;
        int spacingForAverage = 3;

        if (line.Count > 2) {
            List<Vector3> newLine = new List<Vector3>();

            if (line.Count < spacingForAverage * 2) {
                spacingForAverage = line.Count / 2;
            }

            newLine.Add(line[0]);
            for (int i = spacingForAverage; i < line.Count - spacingForAverage; i++) {

                var avgPoint = line[i];
                for (int k = 1; k < spacingForAverage + 1; k++) {
                    avgPoint += line[i + k];
                    avgPoint += line[i - k];
                }

                avgPoint /= spacingForAverage * 2 + 1;
                //var distanceFromOriginalPoint = Vector3.Distance(avgPoint, line[i]);
                var direction = (avgPoint - line[i]);
                newLine.Add(line[i] + displacementPercentage * direction);
            }
            newLine.Add(line[line.Count - 1]);

            return newLine;
        } else {
            return line;
        }
    }

    List<Vector3> ShiftLine(List<Vector3> line, Vector2 shift) {
        List<Vector3> newLine = new List<Vector3>();

        for (int i = 0; i < line.Count; i++) {
            newLine.Add(new Vector3(line[i].x - shift.x, line[i].y - shift.y, line[i].z));
        }

        return newLine;
    }

    Vector3 GetMouseWorldPoint(float zLayer) {
        var screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zLayer);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        return worldPos;
    }

    public void SetColorFromUI(Color color) {
        this.color = color;
    }

    public void SetColorModeFromUI(LineToMesh.ColorMode colorMode) {
        this.colorMode = colorMode;
    }

    private Vector3 GetLineCenterPoint(List<Vector3> line) {
        float avgX = 0, avgY = 0;

        for (int i = 0; i < line.Count; i++) {
            avgX += line[i].x;
            avgY += line[i].y;
        }

        avgX /= line.Count;
        avgY /= line.Count;

        return new Vector3(avgX, avgY, zLayer);
    }
}
