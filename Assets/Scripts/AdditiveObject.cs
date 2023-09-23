using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveObject : MonoBehaviour {
	//public float updateInterval = 1f;
	private ActionableTimer updateTimer;
	private CompositeCollider2D compositeCollider2D;

	public GameObject MeshObjectPrefab;
	// Start is called before the first frame update
	void Start() {
		updateTimer = new ActionableTimer();
		compositeCollider2D = GetComponent<CompositeCollider2D>();
	}

	// Update is called once per frame
	void Update() {
		// updateTimer.Update(CondenseObject, updateInterval, Time.deltaTime);
	}

	public void CondenseObject() {
		foreach(Transform child in transform) {
			Destroy(child.gameObject);
		}

		Mesh newObjectMesh = compositeCollider2D.CreateMesh(true, true);
		print("Test");
		
		var obj = Instantiate(MeshObjectPrefab, transform, true);
		obj.GetComponent<MeshFilter>().mesh = newObjectMesh;
	}

	public void SplitObject(){
		for (int i = 0; i < compositeCollider2D.shapeCount; i++){
			
		}
	}
}
