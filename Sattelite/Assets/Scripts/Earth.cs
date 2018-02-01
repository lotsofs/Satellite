using UnityEngine;
using System.Collections;

public class Earth : MonoBehaviour {

	
	int rotationSpeed = 1;			// I hardcoded this into other things. Be careful when changing.
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
    }
}
