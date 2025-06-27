using System;

namespace TheArchitect.Attributes
{
    /// <summary>
    /// Attribute For Define Resourse Prefab Name and DontDestroyOnLoad flag
    /// </summary>
    public class ResourceAttribute : Attribute
    {
        // Flag for check Dont Destroy on Load
        private bool _isDontDestroyOnLoad = false;
        public bool IsDontDestroyOnLoad => _isDontDestroyOnLoad;

        // Name of Resource Prefab
        private string _name;
        public string Name => _name;

        // Contructor
        public ResourceAttribute()
        {
            _name = "";
        }
        public ResourceAttribute(string name)
        {
            _name = name;
        }
        public ResourceAttribute(bool isDontDestroyOnLoad) : base()
        {
            _isDontDestroyOnLoad = isDontDestroyOnLoad;
        }
        public ResourceAttribute(string name, bool isDontDestroyOnLoad)
        {
            _name = name;
            _isDontDestroyOnLoad = isDontDestroyOnLoad;
        }
    }
}