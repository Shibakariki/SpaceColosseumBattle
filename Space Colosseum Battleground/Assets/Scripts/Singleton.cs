using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T>
{
    #region Fields

    private static T _instance;
    // ReSharper disable once StaticMemberInGenericType
    private static bool _hasInstance;

#if UNITY_EDITOR
    // ReSharper disable once StaticMemberInGenericType
    private static int _destroyedFrameCount = -1;
#endif

    #endregion

    #region Properties

    public static bool HasInstance => _hasInstance
#if UNITY_EDITOR
        || !Application.isPlaying && FindObjectOfType<T>()
#endif
        ;

    public static T Instance
    {
        get
        {
            if (_hasInstance) return _instance;

            var singletonOptionsAttributes = typeof(T).GetCustomAttributes(typeof(SingletonOptionsAttribute), true);
            var singletonOptionsAttribute = (SingletonOptionsAttribute)(singletonOptionsAttributes.Length > 0 ? singletonOptionsAttributes[0] : null);

            var name = !string.IsNullOrEmpty(singletonOptionsAttribute?.Name) ? singletonOptionsAttribute.Name.ToUpper() : typeof(T).Name.ToUpper();
            if (singletonOptionsAttribute != null && singletonOptionsAttribute.IsPrefab)
            {
                name = $"Prefabs/[{name}]";
            }
            
            var asset = FindObjectOfType<T>();
            if (asset != null)
            {
                if (Application.isPlaying)
                {
                    asset.Awake();
                }
                else
                {
                    _instance = asset;
                    _hasInstance = true;
                }
                
                return _instance;
            }

            GameObject go;
            if (singletonOptionsAttribute != null && singletonOptionsAttribute.IsPrefab)
            {
#if UNITY_EDITOR
                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>($"Assets/Resources/{name}.prefab");
                go = asset.gameObject;
#else
                go = (GameObject)Resources.Load(name);
                asset = go.GetComponent<T>();
#endif
                if (asset.DontInstantiate)
                {
                    asset.Awake();
                }
                else asset = Instantiate(go).GetComponent<T>();

                _instance = asset;
                _hasInstance = true;
            }
            else
            {
                go = new GameObject($"[{name.ToUpperInvariant()}]");

                _instance = go.GetComponent<T>();

                if (_instance == null)
                {
                    _instance = go.AddComponent<T>();
                    _hasInstance = true;
                }
            }

            return _instance;
        }
    }

    public virtual bool UseDontDestroyOnLoad => false;

    public virtual bool DontInstantiate => false;

    protected bool IsNotTheSingletonInstance => _instance != null && _instance != this;

    #endregion

    #region Protected Methods

    protected virtual void OnAwake() { }

    #endregion

    #region Unity Event Functions

    protected void Awake()
    {
        // For [ExecuteInEditMode] objects
        if (!Application.isPlaying) return;

        if (_instance != null)
        {
            if (_instance != this)
            {
                DestroyImmediate(gameObject);
            }
            
            return;
        }

        _instance = (T)this;
        _hasInstance = true;
        
#if UNITY_EDITOR
        if (_destroyedFrameCount == Time.frameCount)
        {
            Debug.LogWarningFormat(this, "Singleton '{0}' destroyed and instantiated at the same frame. It might be a cleanup issue. Check the callstack.", typeof(T));
        }
#endif

        if (UseDontDestroyOnLoad && !DontInstantiate) DontDestroyOnLoad(gameObject);

        OnAwake();
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _hasInstance = false;
#if UNITY_EDITOR
            _destroyedFrameCount = Time.frameCount;
#endif
        }
    }

    #endregion
}

[AttributeUsage(AttributeTargets.Class)]
public class SingletonOptionsAttribute : Attribute
{
    public SingletonOptionsAttribute(string name, bool isPrefab = false)
    {
        Name = name;
        IsPrefab = isPrefab;
    }

    public string Name { get; }

    public bool IsPrefab { get; }
}