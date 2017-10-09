using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {

	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.down * Time.deltaTime);
    }
}
