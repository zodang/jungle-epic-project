using UnityEngine;

public class FireMonster : MonoBehaviour
{
    public GameObject model;
    public GameObject modelBaby;

    [SerializeField] float _firePower = 320f;
    


    private void Update()
    {
        if (_firePower < 0)
        {
            if (model.activeSelf)
            {
                model.SetActive(false);
                modelBaby.SetActive(true);
            }

        }
        else
        {
            _firePower += 10f * Time.deltaTime;
            _firePower = Mathf.Clamp(_firePower, 0f, 320f);
            float alpha = Mathf.InverseLerp(0, 320f, _firePower);
            float delta = Mathf.Lerp(0.36f, 1f, alpha);
            model.transform.localScale = new Vector3(delta, delta, 1f);
            foreach (var spirte in model.GetComponentsInChildren<SpriteRenderer>())
            {
                Color color = spirte.color;
                color.a = alpha;
                spirte.color = color;
            }
        }
    }

    public void ApplyWind(float strength)
    {
        _firePower -= strength;
    }
}
