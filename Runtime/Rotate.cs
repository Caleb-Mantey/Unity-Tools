using UnityEngine;

//
// Copyright (c) Relu Interactives. All right's reserved
//

namespace Relu.Utils
{
	public class Rotate : MonoBehaviour {
	
		public float speed = 10.0f;
	
		public enum WhichWayToRotate {AroundX, AroundY, AroundZ}

		public WhichWayToRotate way = WhichWayToRotate.AroundX;
		
		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		private void Update () {
			switch(way)
			{
				case WhichWayToRotate.AroundX:
					transform.Rotate(Vector3.right * (Time.deltaTime * speed));
					break;
				case WhichWayToRotate.AroundY:
					transform.Rotate(Vector3.up * (Time.deltaTime * speed));
					break;
				case WhichWayToRotate.AroundZ:
					transform.Rotate(Vector3.forward * (Time.deltaTime * speed));
					break;
			}	
		}
	}
}