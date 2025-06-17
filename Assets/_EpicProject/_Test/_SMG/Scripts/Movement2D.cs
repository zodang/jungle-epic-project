using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    public Vector2 MoveDir { get; set; }

    private Rigidbody2D _rigidbody2D;
    private float _speed = 5f;

    [Header("Bridge")]
    public bool CheckGround;
    public bool nextIsBirdge;
    public bool nextIsGround;

    private Collider2D _collider;

    public float testdist;

    public Tilemap GroundTilemap;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider = GetComponentInChildren<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (CheckGround)
        {
            testdist = _speed * Time.fixedDeltaTime;
            MoveDir = CheckBridgePathBeforeMove(MoveDir, testdist);
        }

        _rigidbody2D.linearVelocity = MoveDir * _speed;
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
            //nextIsGround = IsBoxCheckLayer(nextPoint, boxSize, LayerMask.GetMask("Ground"));
            nextIsGround = GroundTilemap.HasTile(GroundTilemap.WorldToCell(nextPoint));

            //nextIsBirdge = IsTwoPointsCheckLayer(nextPoint, bounds.size.x / 2f * 0.9f, 0f, LayerMask.GetMask("Bridge"), true);
            //nextIsGround = IsTwoPointsCheckLayer(nextPoint, bounds.size.x / 2f * 0.9f, 0f, LayerMask.GetMask("Ground"), true); 
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
            //nextIsGround = IsBoxCheckLayer(nextPoint, boxSize, LayerMask.GetMask("Ground"));
            nextIsGround = GroundTilemap.HasTile(GroundTilemap.WorldToCell(nextPoint));

            //nextIsBirdge = IsTwoPointsCheckLayer(nextPoint, 0f, bounds.size.y / 2f * 0.9f, LayerMask.GetMask("Bridge"), true);
            //nextIsGround = IsTwoPointsCheckLayer(nextPoint, 0f, bounds.size.y / 2f * 0.9f, LayerMask.GetMask("Ground"), true);
            if (!(nextIsBirdge || nextIsGround))
            {
                moveDir = new Vector2(moveDir.x, 0f);
            }
        }

        return moveDir;
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
}
