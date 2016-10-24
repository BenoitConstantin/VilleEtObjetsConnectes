using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
//[ExecuteInEditMode]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
#if EQUILIBRE_GAMES_DEBUG
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                 "' already destroyed on application quit." +
                                 " Won't create again - returning null.");
#endif
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    CheckMultipleInstance();
                    if (_instance != null)
                    {
                        //Debug.Log("[Singleton] Using instance already created: " + _instance.gameObject.name); 
                        return _instance;
                    }

                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "_" + typeof(T).ToString() + "(singleton)";

                    DontDestroyOnLoad(singleton);
                    applicationIsQuitting = false;
#if EQUILIBRE_GAMES_DEBUG
                    Debug.Log("[Singleton] An instance of " + typeof(T) +
                              " is needed in the scene, so '" + singleton +
                              "' was created with DontDestroyOnLoad.");
#endif
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    /// <summary>
    /// To prevent singleton returning null when game object has also a
    /// DontDestroyOnLoad script, we change the boolean when script is enabled back.
    /// </summary>
    public void OnEnable()
    {
        applicationIsQuitting = false;
        CheckMultipleInstance();
    }

    static void CheckMultipleInstance()
    {
        Object[] objectList = FindObjectsOfType(typeof(T));
        if (_instance == null && objectList.Length > 0)
            _instance = (T)objectList[0];

        if (objectList.Length > 1)
        {


#if UNITY_EDITOR
            // Remove every other instance of EditorOnly singleton
            for (int i = objectList.Length - 1; i >= 0; i--)
            {
                T component = objectList[i] as T;
                if (objectList[i] != _instance && component.gameObject.CompareTag("EditorOnly"))
                    Destroy(component.gameObject);
            }
#else
#if EQUILIBRE_GAMES_DEBUG
						        Debug.LogError("[Singleton<" + typeof(T) + ">] Something went really wrong " +
						                       " - there should never be more than 1 singleton!" +
						                       " Reopenning the scene might fix it.");
#endif
#endif
        }
    }
}