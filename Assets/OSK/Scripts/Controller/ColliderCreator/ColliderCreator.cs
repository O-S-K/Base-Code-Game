/// add the ColliderCreator component to the collider parent object 
/// hit "Setup" and then start copying and positioning those "Point" objects.
/// duplicate point and move

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ColliderCreator : MonoBehaviour
{
    [Header("Gizmos")]
    public bool visualize = true;
    public Color gizmosColor = Color.green;

    [Header("Generation")]
    public bool generateCapsuleColliders = true;
    public bool generateBoxColliders = true;

    private void OnDrawGizmos()
    {
        if (!visualize)
            return;

        // start at 1 (2nd child), first child is container for box colliders.
        for (int i = 1; i < transform.childCount; i++)
        {
            var target = transform.GetChild(i);

            // render capsule for this child.
            if (generateCapsuleColliders)
            {
                DrawWireCapsule(target.position, target.rotation, target.localScale.x, target.localScale.y);
            }

            // render box for child inbetween this and next child.
            if (generateBoxColliders && i < transform.childCount - 1)
            {
                var other = transform.GetChild(i + 1);

                var position = (target.position + other.position) / 2f;
                var size = new Vector3((target.localScale.x + other.localScale.x) / 2f, (target.localScale.y + other.localScale.y) / 2f, Vector3.Distance(target.position, other.position));

                Matrix4x4 rotationMatrix = Matrix4x4.TRS(position, Quaternion.LookRotation(other.position - target.position), size);
                Gizmos.matrix = rotationMatrix;

                Gizmos.color = gizmosColor * 0.3f;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);

                Gizmos.color = gizmosColor;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }

    private void DrawWireCapsule(Vector3 position, Quaternion rotation, float radius, float height)
    {
        Handles.color = gizmosColor;
        Matrix4x4 angleMatrix = Matrix4x4.TRS(position, rotation, Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            var pointOffset = (height - (radius * 2)) / 2;

            // draw sideways.
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, radius);
            Handles.DrawLine(new Vector3(0, pointOffset, -radius), new Vector3(0, -pointOffset, -radius));
            Handles.DrawLine(new Vector3(0, pointOffset, radius), new Vector3(0, -pointOffset, radius));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, radius);
            // draw frontways.
            Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, radius);
            Handles.DrawLine(new Vector3(-radius, pointOffset, 0), new Vector3(-radius, -pointOffset, 0));
            Handles.DrawLine(new Vector3(radius, pointOffset, 0), new Vector3(radius, -pointOffset, 0));
            Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, radius);
            // draw center.
            Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, radius);
            Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, radius);

        }
    }
}
#endif
