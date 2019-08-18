using UnityEngine;

public class RotateBehaviour : MonoBehaviour
{
    public Vector3 RotationAmount;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(RotationAmount * Time.deltaTime);
    }
}