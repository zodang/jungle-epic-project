using UnityEngine;

public class FireMonster : MonoBehaviour
{
    public GameObject model;
    public GameObject modelBaby;
    public GameObject DetectionRange;

    float _firePowerBase = 86f;
    float _firePower;

    private void Start()
    {
        _firePower = _firePowerBase;
    }



    private void Update()
    {
        if (_firePower < 0)
        {
            if (model.activeSelf)
            {
                model.SetActive(false);
                modelBaby.SetActive(true);
                DetectionRange.SetActive(false);
            }

        }
        else
        {
            _firePower += 10f * Time.deltaTime;
            _firePower = Mathf.Clamp(_firePower, 0f, _firePowerBase);
            float alpha = Mathf.InverseLerp(0, _firePowerBase, _firePower);
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
