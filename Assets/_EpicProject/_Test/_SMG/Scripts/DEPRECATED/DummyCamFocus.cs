using Unity.Cinemachine;
using UnityEngine;

public class DummyCamFocus : MonoBehaviour
{
    public Color triggerAreaGizmoColor = new Color(0f, 0.7f, 0f, 0.2f);
    public float triggerRangeX = 1f;
    public float triggerRangeY = 1f;
    public float triggerOffsetX = 0f;
    public float triggerOffsetY = 0f;

    private CinemachineTargetGroup _cinemachineTargetGroup;
    private CinemachineGroupFraming _groupFraming;
    private Transform _target;    



    private void Awake()
    {
        _cinemachineTargetGroup = FindAnyObjectByType<CinemachineTargetGroup>();
        _groupFraming = FindAnyObjectByType<CinemachineGroupFraming>();
        _target = _cinemachineTargetGroup.transform;

        isIn = false;
        prevIsIn = isIn;
    }

    bool isIn;
    bool prevIsIn;



    private void Update()
    {
        float distX = _target.position.x - (transform.position.x + triggerOffsetX);
        float distY = _target.position.y - (transform.position.y + triggerOffsetY);

        bool isIn = distX < triggerRangeX && distX > -triggerRangeX
            && distY < triggerRangeY && distY > -triggerRangeY;

        if (isIn == prevIsIn) return;
        prevIsIn = isIn;

        transform.GetChild(0).gameObject.SetActive(isIn);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        bool isFull = collider2D.bounds.Contains(_cinemachineTargetGroup.transform.position);

        //Debug.Log(_cinemachineTargetGroup.BoundingBox);
        //_groupFraming.


        if (isFull)
            transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Collider2D collider2D = GetComponent<Collider2D>();

        bool isFull = collider2D.bounds.Contains(_cinemachineTargetGroup.transform.position);

        if (!isFull)
            transform.GetChild(0).gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerAreaGizmoColor;
        
        Gizmos.DrawCube(new Vector3(
            transform.position.x + triggerOffsetX
            , transform.position.y + triggerOffsetY
            , transform.position.z)
            , new Vector3(
                triggerRangeX * 2
                , triggerRangeY * 2, 0f)
            );
    }
#endif

    //private CinemachineCamera[] _cinemachineCamera;




    //public float LensSize = 9f;
    //void Test(bool isFocus)
    //{
    //    _isFocus = isFocus;
    //    for(int i = 0; i < _cinemachineCamera.Length; i++)
    //    {
    //        _cinemachineCamera[i].Follow = isFocus ? transform : _cinemachineTargetGroup.transform;
    //        if (_isFocus)
    //        {
    //            _cinemachineCamera[i].Lens.OrthographicSize = LensSize;
    //            //Cinemachine 
    //        }
    //    }
    //}
    //bool _isFocus;
    //float _deltaTime;
    //private void Update()
    //{
    //    _deltaTime += Time.deltaTime;
    //    if (_deltaTime > 3f)
    //    {
    //        _deltaTime = 0f;
    //        Test(!_isFocus);
    //    }
    //}

    //[ContextMenu("Test(true)")]
    //void teston()
    //{
    //    Test(true);
    //}

    //[ContextMenu("Test(false)")]
    //void testoff()
    //{
    //    Test(false);
    //}
}
