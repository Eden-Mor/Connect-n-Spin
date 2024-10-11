using UnityEngine;

public class RigidBodyChildManager : MonoBehaviour
{
    public void SetChildState(bool enable)
    {
        foreach (Transform child in transform)
            if (child.TryGetComponent<Rigidbody2D>(out var rb))
                rb.isKinematic = !enable;
        // This disables the gravity on a game object so that it wont move
    }

    public void RotateChildren(int degrees)
    {
        foreach (Transform child in transform)
        {
            Vector3 currentRotation = child.localEulerAngles;
            currentRotation.z += degrees;
            child.localEulerAngles = currentRotation;
        }
    }

    public void DeleteChildren()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}
