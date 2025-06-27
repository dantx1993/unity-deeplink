using System;

using TheArchitect.Attributes;

using UnityEngine;

namespace TheArchitect.Unity
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance = default(T);                // _instance
        private static object _lock = new object();             // object for lock
        protected static bool _applicationIsQuitting = false;   // flag to check application is quitting
        private const string PREF_NAME = "[Singleton]";         // Prefix name

        public static bool IsAvailable => _instance != null;    // check _instance is not null

        public static T Instance
        {
            get
            {
                // if _applicatinIsQuitting => Warning For This Case
                if (_applicationIsQuitting)
                {
                    Type type = typeof(T);
                    Debug.LogWarning($"{PREF_NAME} Instance '{(type != null ? type.ToString() : null)}' already destroyed. Recreate again");
                }
                // Make sure only one thread is called at a time - Because it is in initialization
                lock (_lock)
                {
                    if (!IsAvailable)
                    {
                        // Find Object in Scene First
                        _instance = FindObjectOfType<T>();
                        if (IsAvailable)
                        {
                            return _instance;
                        }
                        GameObject go = null;
                        bool isDontDestroyOnLoad = false;
                        string nameSingleton = $"{PREF_NAME}{typeof(T).Name}";
                        // Check: Has Resource Attribute???
                        if (Attribute.IsDefined(typeof(T), typeof(ResourceAttribute)))
                        {
                            // If Has Resource Attribute 
                            // => Instantiate Prefab from Resource
                            // => Set DontDestroyOnLoad to Resource Attribute's DontDestroyOnLoad Value
                            ResourceAttribute resourceAttribute = (ResourceAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ResourceAttribute));
                            isDontDestroyOnLoad = resourceAttribute.IsDontDestroyOnLoad;
                            string name = resourceAttribute.Name;
                            try
                            {
                                go = Instantiate(Resources.Load<GameObject>(string.IsNullOrEmpty(name) ? nameSingleton : name));
                            }
                            catch (Exception exc)
                            {
                                if (!string.IsNullOrEmpty(name))
                                {
                                    Debug.LogError($"Could not instantiate prefab even though prefab attribute was set: {exc.Message}\n{exc.StackTrace}");
                                }
                            }
                        }
                        // Check: Hasn't Resource Attribute => Create a new Object
                        if (go == null)
                        {
                            go = new GameObject();
                        }
                        // Set DontDestroyOnLoad For This
                        if (isDontDestroyOnLoad)
                        {
                            DontDestroyOnLoad(go);
                        }
                        // Get or Set T for GO
                        go.name = nameSingleton;
                        _instance = go.GetComponent<T>();
                        if (!IsAvailable)
                        {
                            _instance = go.AddComponent<T>();
                        }
                        Singleton<T> instance = _instance as Singleton<T>;
                        if (instance != null)
                        {
                            // Set Parrent - Define by user
                            if (instance.GetParent() != null)
                            {
                                go.transform.SetParent(instance.GetParent());
                            }
                            // Call Init() - Define by user
                            instance.Init();
                        }
                    }
                }
                return _instance;
            }
        }

        #region MonoBehaviour Method
        protected virtual void OnApplicationQuit() => _applicationIsQuitting = true;
        #endregion

        #region Singleton Base Method
        /// <summary>
        /// Preload a Singleton
        /// </summary>
        /// <param name="parent">Set Parent for Singleton</param>
        public void Load(GameObject parent = null) => SetParent(parent);
        /// <summary>
        /// Overide to code run after create a Singleton, before return Singleton in first time
        /// </summary>
        protected virtual void Init() { }
        /// <summary>
        /// Destroy a static var _instance for free this Singleton
        /// </summary>        
        protected void ResetSingleton(bool isAutoDestroy)
        {
            if (isAutoDestroy && IsAvailable)
            {
                DestroyImmediate(_instance.gameObject);  
            }
            _instance = default(T);
        }
        /// <summary>
        /// Overide for set parent of Singleton on Create
        /// </summary>
        /// <returns>A transform is parent of this Singleton</returns>
        protected virtual Transform GetParent()
        {
            return null;
        }
        /// <summary>
        /// Set Parent for this Singleton
        /// </summary>
        /// <param name="parent">GameObject is parent of this Singleton (you want to)</param>
        protected virtual void SetParent(GameObject parent)
        {
            if (parent == null)
            {
                return;
            }
            this.transform.SetParent(parent.transform);
        }
        #endregion
    }
}