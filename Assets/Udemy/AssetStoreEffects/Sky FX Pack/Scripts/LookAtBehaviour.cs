using UnityEngine;

public class LookAtBehaviour : MonoBehaviour
{
    public Transform Target;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Target != null)
        {
            transform.LookAt(Target);
        }
    }
}