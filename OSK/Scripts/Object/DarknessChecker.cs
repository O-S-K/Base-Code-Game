using UnityEngine;

public class DarknessChecker : MonoBehaviour
{
    [SerializeField] float sunCheckDistance = 100f;
    [SerializeField] float groundCheckDistance = 2f;
    [SerializeField] Vector3 sunCheckCentre = Vector3.zero;
    [SerializeField] Vector3 groundCheckCentre = Vector3.up;
    [SerializeField] LayerMask sunCheckLayerMask;
    [SerializeField] LayerMask groundCheckLayerMask;
    [SerializeField] Transform sunTransform;

    // if (!IsInDarkness(player.position)) Die

    public bool IsInDarkness(Vector3 position)
    {
        if (Physics.Raycast(position + sunCheckCentre, sunTransform.transform.rotation * Vector3.back, 
            out RaycastHit raycastHit, sunCheckDistance, sunCheckLayerMask))
        {
            Debug.DrawRay(position, raycastHit.point - position, Color.yellow);
            return true;
        }
        return Physics.Raycast(position + this.groundCheckCentre, Vector3.down, 
            out RaycastHit raycastHit2, this.groundCheckDistance, this.groundCheckLayerMask);
    }
}
