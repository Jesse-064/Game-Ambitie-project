using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -6f);
    [SerializeField] private float lookAhead = 6f;

    void LateUpdate()
    {
        if (player == null) return;

        transform.position = player.TransformPoint(offset);
        Vector3 lookTarget = player.position + player.forward * lookAhead;
        transform.LookAt(lookTarget);
    }
}
