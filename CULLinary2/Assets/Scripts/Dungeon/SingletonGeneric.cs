using UnityEngine;

//copied from https://gamedevtoday.com/singleton-unity-c-code/
public class SingletonGeneric<T> : MonoBehaviour where T : SingletonGeneric<T>
{
    public static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    instance = singleton.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static bool CheckIfInstanceExists()
    {
        return instance != null;
    }
}