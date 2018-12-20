using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos() {
        for (int i = 0; i < 36; i++) {
            var rotation = (((float) i) / 36)*Mathf.PI*2;

            var x = Mathf.Cos(rotation);
            var z = Mathf.Sin(rotation);

            var rot = new Vector2(transform.eulerAngles.z * Mathf.Deg2Rad,
                                  transform.eulerAngles.x * Mathf.Deg2Rad);
            
            var currentAngleX = rot.x;
            var currentAngleZ = rot.y;

            var cosX = Mathf.Cos(currentAngleX);
            var sinX = Mathf.Sin(currentAngleX);
            var cosZ = Mathf.Cos(currentAngleZ);
            var sinZ = Mathf.Sin(currentAngleZ);

            var Axz = sinX * sinZ;
            var Ayz = -cosX * sinZ;
            var Azz = cosZ;

            var projectX = cosX * x + Axz * z;
            var projectY = sinX * x + Ayz * z;
            var projectZ = Azz * z;

            Gizmos.DrawSphere(transform.localPosition + new Vector3(projectX, projectY, projectZ), .1f);

        }

    }

}
