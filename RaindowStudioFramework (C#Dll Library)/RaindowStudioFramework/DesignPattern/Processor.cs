using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RaindowStudio.Attribute;
using Unity.IO.LowLevel.Unsafe;

namespace RaindowStudio.DesignPattern
{
    public abstract class Processor<TE> : MonoBehaviour where TE : struct, Enum
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [UneditableField, SerializeField] private TE state;
        [UneditableField, SerializeField] private TE preState;

        public event Action<ProcessorStateTriggerType, TE> StateTriggerEvent;

        private Dictionary<ProcessorStateTriggerType, KeyOperator<TE>> stateTriggerKeyOperator =
            new Dictionary<ProcessorStateTriggerType, KeyOperator<TE>>();

        public TE PreState => preState;

        public TE State
        {
            get => state;
            set
            {
                preState = state;
                state = value;
                StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Deactivate, preState);
                StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Activate, state);
            }
        }

        public void ChangeStateByIndex(int targetStateIndex)
        {
            if (Enum.IsDefined(typeof(TE), targetStateIndex))
            {
                TE targetState = (TE)Enum.ToObject(typeof(TE), targetStateIndex);
                ChangeState(targetState);
                return;
            }

            Debug.LogError($"The parameter '{targetStateIndex}' is not available in type '{typeof(TE)}'.");
        }

        public void ChangeStateByString(string targetStateString)
        {
            if (Enum.TryParse(targetStateString, out TE state))
            {
                ChangeState(state);
                return;
            }

            Debug.LogError($"The parameter '{targetStateString}' is not available in type '{typeof(TE)}'.");
        }

        public void ChangeState(TE targetState) =>
            State = targetState;

        /// <summary>
        /// Initialization that could be defined to called in any where not specific.
        /// </summary>
        private void Initialization()
        {
            string[] triggerTypenames = Enum.GetNames(typeof(ProcessorStateTriggerType));
            List<string> triggerTypenames_LowerCase =
                triggerTypenames.Select(element => element.ToLower()).ToList();

            // Register operations.
            MethodInfo[] methodInfos = GetType().GetMethods(bindingFlags);

            List<string> enumNames = Enum.GetNames(typeof(TE)).ToList();

            for (int i = 0; i < enumNames.Count; ++i)
            {
                enumNames[i] = enumNames[i].ToLower();
            }

            for (int i = 0; i < methodInfos.Length; ++i)
            {
                if (methodInfos[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                {
                    string methodName = methodInfos[i].Name;

                    if (methodName.Count(t => t == '_') == 1)
                    {
                        string[] nameSplitted = methodName.Split('_');

                        for (int j = 0; j < nameSplitted.Length; ++j)
                        {
                            if (!enumNames.Contains(nameSplitted[j].ToLower()))
                                continue;

                            TE @enum = (TE)Enum.Parse(typeof(TE), nameSplitted[j], true);

                            string key = nameSplitted[j == 0 ? 1 : 0].ToLower();
                            int keyIndex = triggerTypenames_LowerCase.IndexOf(key);
                            if (keyIndex > -1)
                            {
                                ProcessorStateTriggerType triggerType =
                                    (ProcessorStateTriggerType)Enum.GetValues(typeof(ProcessorStateTriggerType)).GetValue(keyIndex);
                                if (!stateTriggerKeyOperator.ContainsKey(triggerType))
                                {
                                    stateTriggerKeyOperator[triggerType] = new KeyOperator<TE>();
                                }

                                stateTriggerKeyOperator[triggerType].Register(@enum, Delegate.CreateDelegate(typeof(Action), this, methodInfos[i]) as Action);
                            }
                        }
                    }
                }
            }

            StateTriggerEvent += (pstt, te) => stateTriggerKeyOperator[pstt].Operate(te);
        }

        protected virtual void FixedUpdate()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Update, state);
        }

        protected virtual void Update()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.FixedUpdate, state);
        }

        protected virtual void LateUpdate()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.LateUpdate, state);
        }

        protected virtual void Awake()
        {
            Initialization();
        }
    }

    public abstract class Processor<T, TE> : SingletonUnity<T>
        where T : Processor<T, TE>
        where TE : struct, Enum
    {
        private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        [UneditableField, SerializeField] private TE state;
        [UneditableField, SerializeField] private TE preState;

        public event Action<ProcessorStateTriggerType, TE> StateTriggerEvent;

        private Dictionary<ProcessorStateTriggerType, KeyOperator<TE>> stateTriggerKeyOperator =
            new Dictionary<ProcessorStateTriggerType, KeyOperator<TE>>();

        public TE PreState => preState;

        public TE State
        {
            get => state;
            set
            {
                preState = state;
                state = value;
                StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Deactivate, preState);
                StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Activate, state);
            }
        }

        public void ChangeStateByIndex(int targetStateIndex)
        {
            if (Enum.IsDefined(typeof(TE), targetStateIndex))
            {
                TE targetState = (TE)Enum.ToObject(typeof(TE), targetStateIndex);
                ChangeState(targetState);
                return;
            }

            Debug.LogError($"The parameter '{targetStateIndex}' is not available in type '{typeof(TE)}'.");
        }

        public void ChangeStateByString(string targetStateString)
        {
            if (Enum.TryParse(targetStateString, out TE state))
            {
                ChangeState(state);
                return;
            }

            Debug.LogError($"The parameter '{targetStateString}' is not available in type '{typeof(TE)}'.");
        }

        public void ChangeState(TE targetState) =>
            State = targetState;

        /// <summary>
        /// Initialization that could be defined to called in any where not specific.
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            string[] triggerTypenames = Enum.GetNames(typeof(ProcessorStateTriggerType));
            List<string> triggerTypenames_LowerCase =
                triggerTypenames.Select(element => element.ToLower()).ToList();

            // Register operations.
            MethodInfo[] methodInfos = GetType().GetMethods(bindingFlags);

            List<string> enumNames = Enum.GetNames(typeof(TE)).ToList();

            for (int i = 0; i < enumNames.Count; ++i)
            {
                enumNames[i] = enumNames[i].ToLower();
            }

            for (int i = 0; i < methodInfos.Length; ++i)
            {
                if (methodInfos[i].GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length == 0)
                {
                    string methodName = methodInfos[i].Name;

                    if (methodName.Count(t => t == '_') == 1)
                    {
                        string[] nameSplitted = methodName.Split('_');

                        for (int j = 0; j < nameSplitted.Length; ++j)
                        {
                            if (!enumNames.Contains(nameSplitted[j].ToLower()))
                                continue;

                            TE @enum = (TE)Enum.Parse(typeof(TE), nameSplitted[j], true);

                            string key = nameSplitted[j == 0 ? 1 : 0].ToLower();
                            int keyIndex = triggerTypenames_LowerCase.IndexOf(key);
                            if (keyIndex > -1)
                            {
                                ProcessorStateTriggerType triggerType =
                                    (ProcessorStateTriggerType)Enum.GetValues(typeof(ProcessorStateTriggerType)).GetValue(keyIndex);
                                if (!stateTriggerKeyOperator.ContainsKey(triggerType))
                                {
                                    stateTriggerKeyOperator[triggerType] = new KeyOperator<TE>();
                                }

                                stateTriggerKeyOperator[triggerType].Register(@enum, Delegate.CreateDelegate(typeof(Action), this, methodInfos[i]) as Action);
                            }
                        }
                    }
                }
            }

            StateTriggerEvent += (pstt, te) =>
            {
                if (stateTriggerKeyOperator.ContainsKey(pstt))
                {
                    stateTriggerKeyOperator[pstt].Operate(te);
                }
            };
        }

        protected virtual void FixedUpdate()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.Update, state);
        }

        protected virtual void Update()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.FixedUpdate, state);
        }

        protected virtual void LateUpdate()
        {
            StateTriggerEvent?.Invoke(ProcessorStateTriggerType.LateUpdate, state);
        }
    }

    public abstract class ProcessorEternal<T, E> : Processor<T, E>
        where T : ProcessorEternal<T, E>
        where E : struct, Enum
    {
        protected override void Initialization()
        {
            base.Initialization();
            if (gameObject.activeSelf)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}