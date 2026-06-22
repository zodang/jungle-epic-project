using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Define;
public enum AxisLock
{
    /// <summary>None</summary>
    None = 0,
    /// <summary> X: Right, Y: Up </summary>
    Positive = 1,
    /// <summary> X: Left, Y: Down </summary>
    Negative = -1 
}

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    public Vector2 MoveDir { get; set; }

    private Rigidbody2D _rigidbody2D;
    private float _baseSpeed = 5f;
    private float _speed;

    [Header("Bridge")]
    public bool CheckGround;
    public bool nextIsBirdge;
    public bool nextIsGround;

    private Collider2D _collider;

    public float testdist;
    private float _fallDeltaTime = 0f;
    public bool IsFalling;

    //public Tilemap GroundTilemap;
    private List<Tilemap> _groundTilemaps = new List<Tilemap>();

    public List<Transform> FootColliders = new List<Transform>();

    public AxisLock YLock { get; private set; }
    public AxisLock XLock { get; private set; }

    private int _YLock; // 1: Up, -1: down
    private int _XLock; // 1: right, -1: left;

    private string _objectName;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponentInChildren<BoxCollider2D>();

        GameObject[] grounds = GameObject.FindGameObjectsWithTag(Tags.Ground);

        for (int i = 0; i < grounds.Length; i++)
        {
            if (grounds[i].TryGetComponent<Tilemap>(out Tilemap tilemap))
            {
                if (!_groundTilemaps.Contains(tilemap))
                {
                    _groundTilemaps.Add(tilemap);
                }
            }
        }

        _speed = _baseSpeed;

        if (FootColliders.Count < 1)
        {
            FootColliders.Add(transform);
        }

        _objectName = gameObject.name;
    }

    private void FixedUpdate()
    {
        if (CheckGround)
        {
            testdist = _speed * Time.fixedDeltaTime;
            //MoveDir = CheckBridgePathBeforeMove(MoveDir, testdist);
        }
        else
        {
            bool isGround = false;
            for (int i = 0; i < FootColliders.Count; i++)
            {
                if (TilemapsHasTile(_groundTilemaps, FootColliders[i].position))
                {
                    isGround = true;
                    break;
                }
            }
            if (!isGround)
            {
                _fallDeltaTime += Time.deltaTime;
                if (_fallDeltaTime > 0.07f)
                {
                    IsFalling = true;
                    Debug.Log(_objectName + ": Falling");
                }
            }
            else
            {
                _fallDeltaTime = 0f;
            }
        }

        if(_rigidbody2D.bodyType != RigidbodyType2D.Static)
        {
            if (IsFalling)
            {
                _rigidbody2D.linearVelocity = Vector2.down * 20f;
            }
            else
            {
                float xDir = MoveDir.x;
                float yDir = MoveDir.y;

                if ((XLock == AxisLock.Positive && xDir > 0f) || (XLock == AxisLock.Negative && xDir < 0f))
                    xDir = 0;
                if ((YLock == AxisLock.Positive && yDir > 0f) || (YLock == AxisLock.Negative && yDir < 0f))
                    yDir = 0;

                Vector2 moveDir = new Vector2(xDir, yDir);

                _rigidbody2D.linearVelocity = moveDir * _speed;
            }
        }
            
        //_rigidbody2D.MovePosition((Vector2)transform.position + MoveDir * _speed * Time.fixedDeltaTime * Vector2.one);
    }

    Vector2 CheckBridgePathBeforeMove(Vector2 moveDir, float checkDist)
    {
        Bounds bounds = _collider.bounds;

        if (moveDir.x != 0f)
        {
            float xDir = moveDir.x > 0f ? 1f : -1f;
            Vector2 colDir = new Vector2(xDir > 0 ? bounds.max.x : bounds.min.x, bounds.center.y);
            Vector2 nextPoint = colDir + new Vector2(xDir * checkDist, 0f);
            Vector2 boxSize = new Vector2(checkDist / 2f, bounds.size.y);

            nextIsBirdge = IsBoxCheckLayer(nextPoint, boxSize, LayerMask.GetMask("Bridge"));
            nextIsGround = TilemapsHasTile(_groundTilemaps, nextPoint);
            //nextIsGround = GroundTilemap.HasTile(GroundTilemap.WorldToCell(nextPoint));

            if (!(nextIsBirdge || nextIsGround))
            {
                moveDir = new Vector2(0f, moveDir.y);
            }
        }

        if (moveDir.y != 0f)
        {
            float yDir = moveDir.y > 0f ? 1f : -1f;
            Vector2 colDir = new Vector2(bounds.center.x, yDir > 0 ? bounds.max.y : bounds.min.y);
            Vector2 nextPoint = colDir + new Vector2(0f, yDir * checkDist);
            Vector2 boxSize = new Vector2(bounds.size.x, checkDist / 2f);

            nextIsBirdge = IsBoxCheckLayer(nextPoint, boxSize, LayerMask.GetMask("Bridge"));
            nextIsGround = TilemapsHasTile(_groundTilemaps, nextPoint);
            //nextIsGround = GroundTilemap.HasTile(GroundTilemap.WorldToCell(nextPoint));

            if (!(nextIsBirdge || nextIsGround))
            {
                moveDir = new Vector2(moveDir.x, 0f);
            }
        }

        return moveDir;
    }

    bool CheckFallzone(Vector2 moveDir, float checkDist)
    {
        Bounds bounds = _collider.bounds;


        return IsBoxCheckLayer(bounds.center, bounds.size, LayerMask.GetMask("FallZone"));
    }

    public bool IsPointCheckLayer(Vector2 point, LayerMask groundMask)
    {
        return Physics2D.OverlapPoint(point, groundMask);
    }

    bool IsTwoPointsCheckLayer(Vector2 center, float offsetX, float offsetY, LayerMask layerMask, bool checkAnd)
    {
        Vector2 offset = new Vector2(offsetX, offsetY);
        bool result = (checkAnd) 
            ? (IsPointCheckLayer(center + offset, layerMask) && IsPointCheckLayer(center - offset, layerMask)) 
            : (IsPointCheckLayer(center + offset, layerMask) || IsPointCheckLayer(center - offset, layerMask));

        return result;
    }


    public bool IsBoxCheckLayer(Vector2 point, Vector2 size, LayerMask layerMask)
    {
        return Physics2D.OverlapBox(point, size, 0, layerMask);
    }

    public void Respawn(Vector3 position)
    {
        _fallDeltaTime = 0f;
        IsFalling = false;
        _rigidbody2D.linearVelocity = Vector2.zero;
        transform.position = position;
    }

    bool TilemapsHasTile(List<Tilemap> tilemaps, Vector3 worldPosition)
    {
        //Debug.Log("tilemaps.Count: " + tilemaps.Count);
        for(int i = 0; i < tilemaps.Count; i++)
        {
            Tilemap tilemap = tilemaps[i];
            if(tilemap.HasTile(tilemap.WorldToCell(worldPosition)))
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void MultiplySpeed(float multiple)
    {
        _speed = _baseSpeed * multiple;
    }

    /// <summary>
    /// Y축 이동을 제한합니다
    /// </summary>
    public void MoveLockY(AxisLock yLock)
    {
        YLock = yLock;
    }
}
