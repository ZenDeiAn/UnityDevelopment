using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor;

namespace RaindowStudio.DesignPattern
{
    public class KeyOperator<T>
    {
        private Dictionary<T, Action> actions = new Dictionary<T, Action>();

        public List<T> Keys => actions.Keys.ToList();

        public void Register(T _enum, Action _action)
        {
            if (actions.ContainsKey(_enum))
            {
                actions[_enum] = _action;
            }
            else
            {
                actions.Add(_enum, _action);
            }
        }

        public void Operate(T _enum)
        {
            if (actions.ContainsKey(_enum))
            {
                actions[_enum]?.Invoke();
            }
        }
    }
}

namespace RaindowStudio.Language
{
    [Serializable]
    public class LanguageTextData
    {
        public string text;
        public TMP_FontAsset font;
    }
}

namespace RaindowStudio.Utility
{
    [Serializable]
    public class EnumPairList<TE, T> : ISerializationCallbackReceiver
    {
        public List<TE> keys;
        public List<T> values = new List<T>();

        public T this[TE key]
        {
            get
            {
                if (keys == null || keys.Count == 0)
                {
                    InitializeKeys();
                }
                return values[keys.IndexOf(key)];
            }
        }

        public EnumPairList()
        {
            InitializeKeys();
        }

        private void InitializeKeys()
        {
            keys = Enum.GetValues(typeof(TE)).Cast<TE>().ToList();
        }

        // This will be called by Unity after deserialization
        public void OnAfterDeserialize()
        {
            if (keys == null || keys.Count == 0)
            {
                InitializeKeys();
            }
        }

        public void OnBeforeSerialize()
        {
            // Ensure keys are always up-to-date during serialization
            if (keys == null || keys.Count == 0)
            {
                InitializeKeys();
            }
        }
    }
}