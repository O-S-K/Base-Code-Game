using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCameraFollower : MonoBehaviour {
	private Vector2 cameraPreShake;
	private Vector2 shakeVector;
	private Vector2 shakeDirection;
	private int lastDirectionalShake;
	private float shakeTimer;
	private Vector2 Position;

	[Header ("Camera Follow")]
	public Transform m_Target;
	public int m_XOffset = 0;
	public int m_YOffset = 0;
	public Vector2 m_OffsetDir;
	public float m_OffsetAmount = 10f;
	public float m_DampTime = .4f;

	[Header ("Camera Limits/Bounds")]
	public bool m_EnableBounds;
	public int m_MinY, m_MaxY, m_MinX, m_MaxX;

	public static PixelCameraFollower instance = null;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this){
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		Position = new Vector2 (Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
		cameraPreShake = Position;

		// Search for player if there is no target
		if (m_Target == null) {
			var player = GameObject.FindGameObjectWithTag ("Player");
			m_Target = player != null ? player.transform : null;

			if (m_Target == null) {
				Debug.Log ("There is no camera target");
			}
		}
	}

	// Update is called once per frame
	void Update () {
		// Camera Shake
		Position = cameraPreShake;

		if (shakeTimer > 0f)
		{
			if (OnInterval(0.04f))
			{
				int num = (int)Mathf.Ceil((shakeTimer * 10f));
				if (shakeDirection == Vector2.zero)
				{
					shakeVector.x = (float)(-(float)num + new System.Random().Next(num * 2 + 1));
					shakeVector.y = (float)(-(float)num + new System.Random().Next(num * 2 + 1));

					//Fix to white lines between tiles while using tilemap when screenshaking
					shakeVector = -shakeVector * (float)lastDirectionalShake * (float)num;
					if (Mathf.Abs (shakeVector.y) > 1f) {
						shakeVector.y = 1f * Mathf.Sign (shakeVector.y);
					}
				}
				else
				{
					if (lastDirectionalShake == 0)
					{
						lastDirectionalShake = 1;
					}
					else
					{
						lastDirectionalShake *= -1;
					}

					//Fix to white lines between tiles while using tilemap when screenshaking
					shakeVector = -shakeDirection * (float)lastDirectionalShake * (float)num;
					if (Mathf.Abs (shakeVector.y) > 1f) {
						shakeVector.y = 1f * Mathf.Sign (shakeVector.y);
					}
				}


			}
			shakeTimer -= Time.deltaTime;
		}
		else
		{
			shakeVector = Vector2.zero;
		}

		// Camera Follow
		if (m_Target) {
			// Point towards mouse
			var vector = m_OffsetDir * m_OffsetAmount;
			m_XOffset = (int)vector.x;
			m_YOffset = (int)vector.y;

			int targetX = (int)m_Target.position.x + m_XOffset;
			int targetY = (int)m_Target.position.y + m_YOffset;

			// Follow Horizontally
			if (Position.x != targetX) {
				Position.x = (int)Mathf.Lerp (Position.x, targetX, 1/m_DampTime * Time.deltaTime); // Smooth

				// Horizontal Bounds
				if (m_EnableBounds) {
					Position.x = Mathf.Clamp ((int)Position.x, m_MinX, m_MaxX);
				}
			}

			// Follow Vertically
			if (Position.y != targetY) {
				Position.y = (int)Mathf.Lerp (Position.y, targetY, 1/m_DampTime * Time.deltaTime); // Smooth

				// Vertical Bounds
				if (m_EnableBounds) {
					Position.y = Mathf.Clamp ((int)Position.y, m_MinY, m_MaxY);
				}
			}
				
		}

	}

	void LateUpdate () {
		cameraPreShake = Position;
		Position += shakeVector;
		Position = new Vector2( Mathf.Floor(Position.x), Mathf.Floor(Position.y));

		transform.position = new Vector3(Position.x, Position.y, -10f);
	}

	public void DirectionalShake(Vector2 dir, float time = 0.15f)
	{
		shakeDirection = dir.normalized;
		lastDirectionalShake = 0;
		shakeTimer = Mathf.Max(shakeTimer, time);
	}

	public void Shake(float time = 0.45f)
	{
		shakeDirection = Vector2.zero;
		shakeTimer = Mathf.Max(shakeTimer, time);
	}

	public void StopShake()
	{
		shakeTimer = 0f;
	}

	public bool OnInterval(float interval)
	{
		return (int)((Time.time - Time.deltaTime) / interval) < (int)(Time.time / interval);
	}

	public void CameraRecoil (float force) {
		var amount = m_OffsetDir * force;
		amount.x = -(int)amount.x;
		amount.y = -(int)amount.y;

		Position += amount;
		transform.position += new Vector3(amount.x, amount.y, 0);
	}
}
