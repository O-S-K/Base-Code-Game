using System;
using UnityEngine;

public class PushObject : MonoBehaviour
{
	private float pushPower = 2f;
	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
		if (attachedRigidbody == null || attachedRigidbody.isKinematic)
		{
			return;
		}
		Vector3 a = hit.normal * Vector3.Dot(hit.normal, hit.moveDirection);
		attachedRigidbody.velocity = a * this.pushPower;
	}

}
