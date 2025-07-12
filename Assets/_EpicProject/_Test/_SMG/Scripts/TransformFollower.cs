using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    public Transform FollowTransform;

    //public bool FollowPisition;
    public bool IsFollowRotation = true;
    public bool IsFollowScale = true;

    private void LateUpdate()
    {
        if(IsFollowRotation)
        {
            transform.localEulerAngles = FollowTransform.localEulerAngles;
        }    

        if(IsFollowScale)
        {
            transform.localScale = FollowTransform.localScale;
        }
    }
}
