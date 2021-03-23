using UnityEngine;
using System.Collections;

public class FallPlat : MonoBehaviour
{
	public float fallTime = 0.5f;

	void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);
			//if (collision.gameObject.CompareTag("Player"))
			//{
				StartCoroutine(Fall(fallTime));
			//}
		}
	}

	IEnumerator Fall(float time)
	{
		yield return new WaitForSeconds(time);
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().isKinematic = false;
	}
}
