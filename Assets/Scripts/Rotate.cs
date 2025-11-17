using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public GameObject CircValueManip;

	public float XSpeed;
	public float YSpeed;
	public float ZSpeed;

	public bool Rotation=true;
	public bool Manipulation=false;
	public bool CircManip=false;

	// Update is called once per frame
	void Update () {
		if (Rotation) {
			transform.Rotate (XSpeed * Time.deltaTime, YSpeed * Time.deltaTime, ZSpeed * Time.deltaTime);
		}
		if (Manipulation) {
			gameObject.GetComponent<Renderer>().material.SetFloat("_Opacity", (Mathf.Sin(0.1f*Time.time)+1)/2);
		}
		if (CircManip){
			CircValueManip.GetComponent<Renderer>().material.SetFloat("_Value", (Mathf.Sin(0.2f*Time.time)+1)/2);
		}

	
	}
}
