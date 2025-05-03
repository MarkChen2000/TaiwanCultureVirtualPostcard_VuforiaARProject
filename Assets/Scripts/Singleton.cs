using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null) {
                _instance = (T)FindAnyObjectByType(typeof(T));
                if (_instance == null) Debug.LogError("There is no instance of type:" + typeof(T) + "Please check it out!");
            }
            return _instance;
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

}
