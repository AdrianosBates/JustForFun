using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class MouseDrawing : MonoBehaviour {

	public GameObject objectPrefab;
	public GameObject pointPrefab;
	public GameObject boxPrefab;
	public CompositeCollider2D currentCollider;
	public AdditiveObject currentObject;
	public GameObject fakePointer;

	public float interval = 0.5f;
	public float minDistance = 0.1f;
	public float minBoxDistance = 1f;
	public bool isDrawing = false;
	public bool lastDrawing = false;
	private ActionableTimer drawingIntervalTimer;
	private Vector3 currentPos;
	private Vector3 lastPos;
	


	// Start is called before the first frame update
	void Start() {
		drawingIntervalTimer = new ActionableTimer();
	}

	// Update is called once per frame
	void Update() {
		currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currentPos.z = 0;

		if (Input.GetMouseButton(0)) {
			lastDrawing = isDrawing;
			isDrawing = true;
		} else {
			lastDrawing = isDrawing;
			isDrawing = false;
		}

		if (!lastDrawing && isDrawing) {
			if (CanStartDrawing()) {
				StartDrawingTick();
			} else {
				isDrawing = false;
			}
		}

		if (isDrawing) {
			drawingIntervalTimer.Update(DrawingTick, interval, Time.deltaTime);
		}

		if (lastDrawing && !isDrawing) {
			StopDrawingTick();
			drawingIntervalTimer.Reset();
		}
	}

	private bool CanStartDrawing() {
		LayerMask mask = LayerMask.GetMask("DoneDrawing");

		if (Physics2D.OverlapCircleAll(currentPos, 0.5f, mask).Length > 0) {
			print("Cant Draw");
			return false;
		}
		return true;
	}

	private bool CanDrawLine(Vector2 a, Vector2 b) {
		LayerMask mask = LayerMask.GetMask("DoneDrawing");

		var distance = Vector2.Distance(a, b);
		var between = Vector2.Lerp(lastPos, currentPos, 0.5f);
		var angle = Quaternion.FromToRotation(Vector3.right, a - b).eulerAngles.z;

		if (Physics2D.OverlapBoxAll(between, new Vector2(distance, 1.05f), angle, mask).Length > 0) {
			print("Cant Draw");
			return false;
		}
		return true;
	}

	private void StartDrawingTick() {
		print("Start Drawing");

		var obj = Instantiate(objectPrefab, currentPos, Quaternion.identity, this.transform);
		currentCollider = obj.GetComponent<CompositeCollider2D>();
		currentObject = obj.GetComponent<AdditiveObject>();

		GameObject.Instantiate(pointPrefab, currentPos, Quaternion.identity, currentCollider.transform);
		lastPos = currentPos;


		fakePointer = GameObject.Instantiate(pointPrefab, currentPos, Quaternion.identity, currentCollider.transform);
		fakePointer.GetComponent<PolygonCollider2D>().enabled = false;
		// DrawingTick();
	}

	private void DrawingTick() {
		var distanceFromLast = Vector2.Distance(currentPos, lastPos);
		var between = Vector2.Lerp(lastPos, currentPos, 0.5f);

		if (distanceFromLast >= minDistance && CanStartDrawing() && CanDrawLine(currentPos, lastPos)) {

			if (distanceFromLast >= minBoxDistance) {
				var connectingBox = GameObject.Instantiate(boxPrefab, currentPos, Quaternion.identity, currentCollider.transform);
				var newScale = connectingBox.transform.localScale;
				var newRot = connectingBox.transform.rotation;
				var newPos = connectingBox.transform.position;
				newRot = Quaternion.FromToRotation(Vector3.right, lastPos - newPos);
				newScale.x = distanceFromLast;
				newPos = between;
				connectingBox.transform.localScale = newScale;
				connectingBox.transform.rotation = newRot;
				connectingBox.transform.position = newPos;

				GameObject.Instantiate(pointPrefab, currentPos, Quaternion.identity, currentCollider.transform);
			} else {
				GameObject.Instantiate(pointPrefab, currentPos, Quaternion.identity, currentCollider.transform);
			}

			fakePointer.transform.position = currentPos;
			lastPos = currentPos;
		}
	}

	private void StopDrawingTick() {
		print("Stop Drawing");

		GameObject.Instantiate(pointPrefab, lastPos, Quaternion.identity, currentCollider.transform);

		Destroy(fakePointer);

		currentCollider.GenerateGeometry();
		currentCollider.attachedRigidbody.gravityScale = 1;
		currentCollider.attachedRigidbody.mass = currentCollider.transform.childCount * 0.05f;
		currentCollider.gameObject.layer = 7;

		currentObject.CondenseObject();
	}
}