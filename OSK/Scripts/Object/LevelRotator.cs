using System;
using UnityEngine;

public class LevelRotator : MonoBehaviour
{
	[SerializeField] float rotateAcceleration = 100f;
	[SerializeField] float maxRotateSpeed = 100f;
	[SerializeField] Transform gear;
	[SerializeField] float gearSpeed = -3f;
	[SerializeField] AudioSource gridingAudioSource;
	private float rotateSpeed;
	private float maxGrindVolume;

	private void Awake()
	{
		if (this.gridingAudioSource)
		{
			this.maxGrindVolume = this.gridingAudioSource.volume;
			this.gridingAudioSource.volume = 0f;
		}
	}

	public void RotateLevel(bool inputEnabled)
	{
		float num = (inputEnabled ? (Input.GetAxisRaw("Rotate") * this.maxRotateSpeed) : 0f) - this.rotateSpeed;
		num = Mathf.Clamp(num, -this.rotateAcceleration * Time.deltaTime, this.rotateAcceleration * Time.deltaTime);
		this.rotateSpeed += num;
		base.transform.localRotation *= Quaternion.Euler(0f, -this.rotateSpeed * Time.deltaTime, 0f);
		if (this.gear)
		{
			this.gear.localRotation *= Quaternion.Euler(0f, -this.rotateSpeed * Time.deltaTime * this.gearSpeed, 0f);
		}
		if (this.gridingAudioSource)
		{
			this.gridingAudioSource.volume = this.maxGrindVolume * Mathf.Abs(this.rotateSpeed) / this.maxRotateSpeed;
		}
	}

}
