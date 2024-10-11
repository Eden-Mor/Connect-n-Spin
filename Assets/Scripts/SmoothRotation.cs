using UnityEngine;
using System.Collections;
using System;

public class SmoothRotation : MonoBehaviour
{
    public bool IsRotating { get; private set; } = false;
    
    public float rotationDuration = 1f;  // Time for the rotation animation (seconds)

    public void RotateDegrees(float degrees, Action onRotationComplete = null)
    {
        if (IsRotating)  // Only start rotating if it's not already rotating
            return;

        StartCoroutine(RotateCoroutine(degrees, onRotationComplete));
    }

    private IEnumerator RotateCoroutine(float degrees, Action onRotationComplete = null)
    {
        IsRotating = true;
        Quaternion startRotation = transform.rotation;  // Store the initial rotation
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 0, -degrees);  // Calculate the target rotation

        float timeElapsed = 0f;

        // Rotate over time
        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / rotationDuration;

            // Lerp between start and target rotations
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            // This yield returns control until the next frame
            yield return null;
        }

        // Ensure the rotation is exactly the target rotation at the end
        transform.rotation = targetRotation;

        onRotationComplete?.Invoke();

        IsRotating = false;
    }
}
