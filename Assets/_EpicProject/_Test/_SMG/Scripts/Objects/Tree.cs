using UnityEngine;

public class Tree : MonoBehaviour
{
    public Sprite StumpSprite;
    public bool IsCuted { get; private set; }
    public bool CutedInStart;

    private GameObject _leaf;
    
    private void Awake()
    {
        _leaf = transform.GetChild(0).gameObject;
        IsCuted = CutedInStart;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Define.Tags.Slash))
        {
            if (IsCuted) return;    // Only Once
            IsCuted = true;

            // Change Sprite to Stump
            if(TryGetComponent<SpriteRenderer>(out SpriteRenderer renderer))
            {
                renderer.sprite = StumpSprite;
            }
            
            // Drop Leaf
            bool hitLeft = transform.position.x - collision.transform.position.x > 0f;
            _leaf.SetActive(true);
            _leaf.transform.localEulerAngles = new Vector3(0f, 0f, -27f * ((hitLeft) ? 1f : -1f));
            _leaf.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f + Vector2.right * ((hitLeft) ? 1f : -1f) * 2f , ForceMode2D.Impulse);
            Invoke("leafSetActiveFalse", 5f);
        }
    }

    void leafSetActiveFalse()
    {
        _leaf.SetActive(false);
    }
}
