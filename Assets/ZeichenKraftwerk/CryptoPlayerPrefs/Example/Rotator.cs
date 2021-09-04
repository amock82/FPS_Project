using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
	public Vector3 eulersPerSecond = new Vector3(0, 0, 1);
	
	Transform myTransform;
	public void Start() {
		myTransform = transform;
	}
	
	void Update () {
		myTransform.Rotate(eulersPerSecond * Time.deltaTime);
	}
}
