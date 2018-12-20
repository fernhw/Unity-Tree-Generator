/** 
 * Copyright (c) 2018 Fernando Holguin Weber - All Rights Reserved
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of the Unity Tree Generator 
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute,
 * sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
 * NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Fernando Holguín Weber, <contact@fernhw.com>,<http://fernhw.com>,<@fern_hw>
 * 
 */



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
