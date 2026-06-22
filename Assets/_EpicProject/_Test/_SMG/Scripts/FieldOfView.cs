using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [Range(0, 360)] public float fov = 90f;     //시야각
    public int rayCount = 90;                   //레이의 수 (높을수록 퀄리티↑퍼포먼스↑)
    public float viewDistance = 10f;            //거리
    public LayerMask layerMask;                 //벽으로 인식할 레이어

    private Camera mainCamera;                  //카메라
    private Mesh mesh;                          //시야각을 그려줄 메쉬

    private MeshRenderer meshRenderer;
    private Color originalMaterialColor;
    public float alpha = 0.2f;

    private void Start()
    {
        mainCamera = Camera.main;
        mesh = new Mesh();
        mesh.name = "FOV_Effect";
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer.sharedMaterial != null)
        {
            originalMaterialColor = meshRenderer.sharedMaterial.color;
        }
    }

    private void OnDestroy()
    {
        if (meshRenderer != null && meshRenderer.sharedMaterial != null)
        {
            meshRenderer.sharedMaterial.color = originalMaterialColor;
        }
    }

    int step = 0;
    private void LateUpdate()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y, 0)));
        DrawFOV();
        if(Input.GetMouseButtonDown(0))
        {
            step++;
            switch(step%3)
            {
                case 0:
                    meshRenderer.sharedMaterial.color = new Color(1f, 1f, 1f, alpha);
                    break;
                case 1:
                    meshRenderer.sharedMaterial.color = new Color(1f, 1f, 0f, alpha);
                    break;
                case 2:
                    meshRenderer.sharedMaterial.color = new Color(1f, 0f, 0f, alpha);
                    break;
            }
            
        }
    }

    private void DrawFOV()
    {
        float angle = fov * 0.5f;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(transform.eulerAngles.z + angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null)
            {
                vertex = GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = GetVectorFromAngle(angle) * (raycastHit2D.distance / viewDistance) * viewDistance;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            ++vertexIndex;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    private int GetAngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        int angle = Mathf.RoundToInt(n);

        return angle;
    }
}
