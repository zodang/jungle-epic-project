using UnityEngine;
using UnityEngine.Events;

public class CheckBridge : MonoBehaviour
{
    //public bool IsLeftConnected { get; private set; }
    //public bool IsRightConnected { get; private set; }

    //public bool IsBridgeComplete => IsLeftConnected && IsRightConnected;

    public UnityEvent OnConnect;
    public UnityEvent OnDisconnect;

    private int _leftConnectCnt;
    private int _rightConnectCnt;

    private bool _isBridgeConnect;
    private bool _prevBridgeConnect;

    public void SetLeftConnected(bool connect)
    {
        _leftConnectCnt += (connect) ? 1 : -1;
        if (_leftConnectCnt < 0) _leftConnectCnt = 0;

        CheckConnect();
    }

    public void SetRightConnected(bool connect)
    {
        _rightConnectCnt += (connect) ? 1 : -1;
        if (_rightConnectCnt < 0) _rightConnectCnt = 0;

        CheckConnect();
    }

    void CheckConnect()
    {
        _isBridgeConnect = _leftConnectCnt > 0 && _rightConnectCnt > 0;

        if(_isBridgeConnect != _prevBridgeConnect)
        {
            _prevBridgeConnect = _isBridgeConnect;
            if(_isBridgeConnect)
            {
                OnConnect?.Invoke();
            }
            else
            {
                OnDisconnect?.Invoke();
            }
            StageBaseManager.Instance.FlagManager.SetFlag("NPC_02_Flag01", _isBridgeConnect);
        }    
    }

}
