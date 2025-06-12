using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindFirstObjectByType(typeof(T));

                if (_instance == null)
                {
                    SetupInstance();
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        RemoveDuplicates();
    }

    private static void SetupInstance()
    {
        // lazy instantiation
        _instance = (T)FindFirstObjectByType(typeof(T));

        /*if (_instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            _instance = gameObj.AddComponent<T>();
            DontDestroyOnLoad(gameObj);
        }*/
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;

            // Use DontDestroyOnLoad to make persistent but clean up/dispose manually
            // DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}