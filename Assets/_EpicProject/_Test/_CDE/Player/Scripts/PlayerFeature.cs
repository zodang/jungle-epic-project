using UnityEngine;

public class PlayerFeature : MonoBehaviour, IControllable, IScalable
{
    public bool _enableMove;
    private float _speed = 5f;
    public float scaleMin = 0.5f, scaleMax = 2.0f;
    
    private void Update()
    {
        if (!_enableMove) return;
        Move();
    }

    #region Control
    public void EnableControl()
    {
        _enableMove = true;
    }

    public void DisableControl()
    {
        _enableMove = false;
    }
    private void Move()
    {
        Vector2 moveInput = InputManager.Instance.MoveInput;
        transform.Translate(moveInput * (_speed * Time.deltaTime));
    }
    #endregion

    #region Scale
    float IScalable.GetMinValue() => scaleMin;
    float IScalable.GetMaxValue() => scaleMax;
    float IScalable.GetCurrentValue() => transform.localScale.x;
    void IScalable.SetValue(float v)
    {
        transform.localScale = new Vector3(v, v, 1f);
    }
    #endregion
    

}
