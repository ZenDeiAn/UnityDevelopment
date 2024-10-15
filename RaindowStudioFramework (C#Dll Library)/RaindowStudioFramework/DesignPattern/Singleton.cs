using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RaindowStudio.DesignPattern
{
    // The Singleton with MonoBehavior & has the DontDestroyOnload().
    #region SingletonUnityEternal

    /// <summary>
    /// Eternal Unity Singleton is the singleton with MonoBehavior & DontDestroyOnLoad() Function.
    /// </summary>
    /// <typeparam name="SingletonUnityEternal"></typeparam>
    public abstract class SingletonUnityEternal<T> : SingletonUnity<T> where T : MonoBehaviour
    {
        protected override void Initialization()
        {
            base.Initialization();
            if (_instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    #endregion

    // The Singleton with MonoBehavior.
    #region SingletonUnity

    /// <summary>
    /// Unity Singleton is the singleton with MonoBehavior.
    /// </summary>
    /// <typeparam name="SingletonUnity"></typeparam>
    [DisallowMultipleComponent]
    public abstract class SingletonUnity<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        /// <summary>
        /// Get instance while there is any GameObject.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<T>();
                _instance?.GetComponent<SingletonUnity<T>>().Initialization();
                return _instance;
            }
        }

        /// <summary>
        /// Get instance. Additionally instantiate new gameObject while Instance is null.
        /// </summary>
        public static T Inwn
        {
            get
            {
                if (Instance == null)
                {
                    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Initialization()
        {
            gameObject.SetActive(Instance == this);
        }

        protected virtual void Awake()
        {
            // Trigger the Initialization function.
            _instance = Instance;
            gameObject.SetActive(Instance == this);
        }
    }

    #endregion

    // Just simple Singleton.
    #region Singleton
    /// <summary>
    /// Normal singleton.
    /// </summary>
    /// <typeparam name="T">Component name.</typeparam>
    public abstract class Singleton<T> where T : class, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

        public static T Instance => lazyInstance.Value;
    }

    #endregion
}
