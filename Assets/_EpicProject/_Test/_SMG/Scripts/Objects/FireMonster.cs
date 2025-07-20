using UnityEngine;
using UnityEngine.Events;

public class FireMonster : MonoBehaviour
{
    public GameObject BabyObject;

    public GameObject model;
    //public GameObject modelBaby;
    //public GameObject DetectionRange;

    [SerializeField] float _firePowerBase = 86f;
    float _firePower;

    public UnityEvent OnDefeated;

    private void Start()
    {
        _firePower = _firePowerBase;
    }



    private void Update()
    {
        if (_firePower < 0)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                BabyObject.SetActive(true);
                //model.SetActive(false);
                //modelBaby.SetActive(true);
                //DetectionRange.SetActive(false);
                OnDefeated?.Invoke();
                
                // 로그 시스템
                StageBaseManager.Instance.ChangeStageSection("51_2_pass_monster");
            }

        }
        else
        {
            _firePower += 10f * Time.deltaTime;
            _firePower = Mathf.Clamp(_firePower, 0f, _firePowerBase);
            float alpha = Mathf.InverseLerp(0, _firePowerBase, _firePower);
            float delta = Mathf.Lerp(0.4f, 1f, alpha);
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
