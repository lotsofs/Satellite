using UnityEngine;
using System.Collections;

public class SatDebris : Satellite {

    Vector3 rotationAngle = Vector3.zero;

	// Use this for initialization
	public override void Start () {
        base.Start();
        rotationAngle.x += Random.Range(-2f, 2f);
        rotationAngle.y += Random.Range(-2f, 2f);
        rotationAngle.z += Random.Range(-2f, 2f);
    }

    // Update is called once per frame
    public override void Update () {
        base.Update();
        transform.Rotate(rotationAngle * Time.deltaTime);
	}
}
