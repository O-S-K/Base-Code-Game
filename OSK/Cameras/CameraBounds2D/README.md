# Camera Bounds 2D
A camera bounds visualizer editor tool.

![image](https://github.com/prashant-singh/camera-bounds-2D/blob/master/camera%20bounds1.gif)

> You can check out the art I used in the gif [here](https://0x72.itch.io/dungeontileset-ii).

# Integration

1. Create an empty game object in the hierarchy.
2. Assign our script CameraBounds2D.cs to that object. 

Let's assume you have a script like below that follows the player.

```csharp
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
	Transform player;
	float speed;

	void Update()
	{
		Vector3 currentPosition = transform.position;
		Vector3 targetPosition = player.position;
		transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * speed);
	}
}
```
We will add declare variables.
```csharp
[SerializeField] CameraBounds2D bounds;
Vector2 maxXPositions,maxYPositions;
```
maxXPositions and maxYPositions are the X axes and Y axes limits for the camera.
So for example if themaxXPositions has the value (-13 , 8) which means the minimum x position for the camera is -13 and the maximum will be 8. Same works for the maxYPosition with respect to the Y axes.

Now in add this code in the Awake() method if you already have it.
If not you can create an Awake() function like below.

```csharp
	void Awake()
	{
		bounds.Initialize(GetComponent<Camera>());
        	maxXPositions = bounds.maxXlimit;
        	maxYPositions = bounds.maxYlimit;
	}
``` 
bounds.Initialize(GetComponent<Camera>()); will initialize the bounds script with you current camera.
And the maxXPos and maxYPos will be assigned with the bounds calculated values.

```csharp
	void Update()
	{
		Vector3 currentPosition = transform.position;
	        Vector3 targetPosition = new Vector3(Mathf.Clamp(player.position.x, maxXPositions.x, maxXPositions.y), Mathf.Clamp(player.position.y, maxYPositions.x, maxYPositions.y), currPos.z);
		transform.position = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * speed);
	}
```
Here we will clamp the x and y positions of our camera in the targetPos variable.
Which will prevent our camera to go past the maximum and minimum x and y positions.

Now save the script and go into the unity editor and Drag your GameObject with the CameraBounds.cs script and assign the reference to the bounds object in the CameraFollow.cs script in the scene.

In the CameraBounds.cs script you will see 2 field Bounds X and Bounds Y you can change the value and you will see the rectangle in the scene view changing.
Set it to your need and it's done.

And that's it!

If you get stuck at somewhere let me know.


