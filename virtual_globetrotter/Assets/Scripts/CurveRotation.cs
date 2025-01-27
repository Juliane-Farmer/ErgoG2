using System.Collections;
using UnityEngine;

public class CurveRotation : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve rotationCurve; // The curve for rotation. Editable in the Unity editor.

    [SerializeField]
    private float rotationDuration = 2.0f; // Duration of the rotation using the curve.

    private Quaternion originalRotation;
    private bool isRotating = false;

    private void OnEnable()
    {
        // Save the original rotation of the GameObject.
        originalRotation = transform.rotation;
        StartCoroutine(WaitAndRotate(5.5f));
    }

    private IEnumerator WaitAndRotate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Start the rotation process.
        yield return StartCoroutine(RotateByCurve());

        // Return to the original rotation.
        transform.rotation = originalRotation;
    }

    private IEnumerator RotateByCurve()
    {
        isRotating = true;
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            // Get the normalized time (0 to 1).
            float normalizedTime = elapsedTime / rotationDuration;

            // Evaluate the curve to get the rotation value at this point in time.
            float curveValue = rotationCurve.Evaluate(normalizedTime);

            // Apply the rotation around the Y-axis (or any axis you prefer).
            transform.rotation = originalRotation * Quaternion.Euler(0, curveValue * 360, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is set to the end of the curve.
        transform.rotation = originalRotation * Quaternion.Euler(0, rotationCurve.Evaluate(1f) * 360, 0);

        isRotating = false;
    }

    private void OnValidate()
    {
        // Ensure the rotation duration is always positive.
        if (rotationDuration < 0f)
        {
            rotationDuration = 0f;
        }
    }
}
