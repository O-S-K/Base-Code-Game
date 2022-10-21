using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float minX = -60f;
	public float maxX = 60f;

	public float sensitivity;

	float rotY = 0f;
	float rotX = 0f;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		rotY += Input.GetAxis("Mouse X") * sensitivity;
		rotX += Input.GetAxis("Mouse Y") * sensitivity;

		rotX = Mathf.Clamp(rotX, minX, maxX);

		//transform.localEulerAngles = new Vector3(0, rotY, 0);
		transform.localEulerAngles = new Vector3(-rotX, rotY, 0);

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if (Cursor.visible && Input.GetMouseButtonDown(1))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
