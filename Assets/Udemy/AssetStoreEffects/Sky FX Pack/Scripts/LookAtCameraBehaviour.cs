using UnityEngine;

public class LookAtCameraBehaviour : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Camera.current == null) return;
        transform.LookAt(new Vector3(Camera.current.transform.position.x, transform.position.y,
            Camera.current.transform.position.z));
    }
}