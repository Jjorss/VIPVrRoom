using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

	// Use this for initialization
	private SteamVR_TrackedObject trackedObj;
	// 1
	private GameObject collidingObject; 
	// 2
	private GameObject objectInHand;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	// Update is called once per frame


	private void SetCollidingObject(Collider col)
	{
		// 1
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}
		// 2
		collidingObject = col.gameObject;
	}

	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject (other);
	}

	public void OnTriggerExit (Collider other)
	{
		if (!collidingObject) {
			return;
		}

		collidingObject = null;
	}

	private void  GrabObject()
	{
		objectInHand = collidingObject;
		collidingObject = null;

		var joint = AddFixedJoint ();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody> ();
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint> ();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

	private void ReleaseObject()
	{
		if (GetComponent<FixedJoint>())
		{
			GetComponent<FixedJoint>().connectedBody=null;
			Destroy (GetComponent<FixedJoint> ());

			objectInHand.GetComponent<Rigidbody>().velocity=Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
		}
		print ("Released Object Called");
		objectInHand=null;
	}



	void Update ()
	{
		if (Controller.GetHairTriggerDown ()) {
			if (collidingObject) {
				GrabObject ();
			}


		}
		if (Controller.GetHairTriggerUp ()) {
			print ("controller released triger");
			if (objectInHand) {
				ReleaseObject ();
			}
		}

	}
}
