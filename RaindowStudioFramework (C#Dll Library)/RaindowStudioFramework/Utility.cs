using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace RaindowStudio.Utility
{
    public static class Utility
    {
        public static string DynamicCodeFolderName;

        #region Transform utilities
        public static void PositionX(this Transform self, float x)
        {
            Vector3 position = self.position;
            position.x = x;
            self.position = position;
        }

        public static void PositionY(this Transform self, float y)
        {
            Vector3 position = self.position;
            position.y = y;
            self.position = position;
        }

        public static void PositionZ(this Transform self, float z)
        {
            Vector3 position = self.position;
            position.z = z;
            self.position = position;
        }

        public static void LocalPositionX(this Transform self, float x)
        {
            Vector3 position = self.localPosition;
            position.x = x;
            self.localPosition = position;
        }

        public static void LocalPositionY(this Transform self, float y)
        {
            Vector3 position = self.localPosition;
            position.y = y;
            self.localPosition = position;
        }

        public static void LocalPositionZ(this Transform self, float z)
        {
            Vector3 position = self.localPosition;
            position.z = z;
            self.localPosition = position;
        }
        #endregion

        #region Char & string Utilities
        public static string GenerateUUID()
        {
            string hexDigits = "0123456789abcdef";
            System.Random random = new System.Random();
            string uuid = "";

            for (int i = 0; i < 32; i++)
            {
                uuid += hexDigits[random.Next(0, hexDigits.Length)];
            }

            // Insert hyphens at specific positions
            uuid = uuid.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            return uuid;
        }

        public static bool IsAlphabet(this char self)
        {
            return self >= 'A' && self <= 'z';
        }

        public static bool IsNumber(this char self)
        {
            return self >= '0' &&  self <= '9';
        }

        /// <summary>
        /// Check stirng could be used as script define naming. Judge with is start with alphabet and without special character except'_'.
        /// </summary>
        public static bool IsScriptableNaming(this string self)
        {
            if (!self[0].IsAlphabet())
            {
                return false;
            }
            for (int i = 0; i < self.Length; ++i)
            {
                if (!(self[i].IsNumber() || self[i].IsAlphabet() || self[i] == '_'))
                {
                    return false;
                }
            }
            return true;
        }

        public static string FormatAsTimeString(this float second) => TimeSpan.FromSeconds(second).ToString(@"mm\:ss\.ff");

        public static string FormatAsMillisecondString(this float second, int roundAmount = 2) => TimeSpan.FromSeconds((float)Math.Round(second, roundAmount)).TotalMilliseconds.ToString();

        public static int FormatAsMillisecond(this float second, int roundAmount = 2) => (int)((float)Math.Round(second, roundAmount) * Mathf.Pow(10, roundAmount));

        public static string FormatAsTimeString(this int millisecond) => TimeSpan.FromMilliseconds(millisecond).ToString(@"mm\:ss\.ff");
        #endregion

        public static List<int> ToDigitList(this int self)
        {
            int number = self;
            List<int> digits = new List<int>();
            while (number > 0)
            {
                digits.Add(number % 10);
                number /= 10;
            }
            digits.Reverse();
            return digits;
        }

        public static void UpdateEnumInDynamicCodeScript(string enumName, string[] enums, Action<Dictionary<string, int>> resortedAction = null) => UpdateEnumInScript(Path.Combine(Application.dataPath, @"RaindowStudio\DynamicCode.cs"), enumName, enums, resortedAction);

        public static void UpdateEnumInScript(string scriptPath, string enumName, string[] enums, Action<Dictionary<string, int>> resortedAction = null)
        {
            string script = File.ReadAllText(scriptPath);
            string enumDefination = $"enum {enumName}";
            int indexOfDefinationStart;
            Dictionary<string, int> enumIndexes = new Dictionary<string, int>();
            int indexTemp = 0;
            int index = 0;
            if (script.Contains(enumDefination))
            {
                indexOfDefinationStart = script.IndexOf(enumDefination);
                int indexofDefinationEnd = script.IndexOf('}', indexOfDefinationStart);
                string enumDefineString = script.Substring(indexOfDefinationStart, indexofDefinationEnd - indexOfDefinationStart + 1);
                enumDefineString = enumDefineString.Remove(0, enumDefineString.IndexOf('{') + 1).Replace("}", string.Empty);
                index = 0;
                foreach (string e in enumDefineString.Split(','))
                {
                    string en = e.Trim();

                    if (string.IsNullOrWhiteSpace(en))
                        continue;

                    if (en.Contains('='))
                    {
                        var enu = en.Split('=');
                        if (int.TryParse(enu[1], out indexTemp))
                        {
                            enumIndexes[enu[0].Trim()] = indexTemp;
                            index = indexTemp;
                        }
                        else
                        {
                            enumIndexes[enu[0].Trim()] = index;
                        }
                    }
                    else
                    {
                        enumIndexes[en] = index;
                    }
                    index++;
                }
                script = script.Remove(indexOfDefinationStart, indexofDefinationEnd - indexOfDefinationStart + 1);
            }
            else
            {
                script += "\n";
                indexOfDefinationStart = script.Length;
            }
            Dictionary<string, int> newEnumIndexes = new Dictionary<string, int>();
            string newEnumDefination = $"enum {enumName}\n" + '{';
            for (int i = 0; i < enums.Length; ++i)
            {
                enumName = enums[i];
                index = i;
                if (enumName.IsScriptableNaming())
                {
                    if (!enumIndexes.ContainsKey(enumName))
                    {
                        while (enumIndexes.ContainsValue(index))
                        {
                            index++;
                        }
                        newEnumIndexes[enumName] = index;
                    }
                    else
                    {
                        newEnumIndexes[enumName] = enumIndexes[enumName];
                    }
                }
                else
                {
                    Debug.LogError($"Enum name is illegal. Please remove special character or check is start with Alphabet or not : {enums[i]}");
                }
            }
            newEnumIndexes = newEnumIndexes.OrderBy(t => t.Value).ToDictionary(entry => entry.Key, entry => entry.Value);
            var list = newEnumIndexes.ToList();
            for (int i = 0; i < list.Count; ++i)
            {
                var kvp = list[i];
                newEnumIndexes[kvp.Key] = i;
                newEnumDefination += $"\n\t{kvp.Key}={i},";
            }

            newEnumDefination += "\n}";
            script = script.Insert(indexOfDefinationStart, newEnumDefination);
            File.WriteAllText(scriptPath, script);

            resortedAction?.Invoke(newEnumIndexes);
        }

        public static Vector2 SwapXY(this Vector2 self)
        {
            return new Vector2(self.y, self.x);
        }

        private static Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();

        /// <summary>
        /// Start a Coroutine for Delay action.
        /// </summary>
        /// <param name="delaySecond">How much delay will be to Invoke the action.</param>
        /// <param name="toDo">Action that will Invoke after delay.</param>
        public static Coroutine DelayToDo(this MonoBehaviour self, float delaySecond, Action toDo)
        {
            return self.StartCoroutine(DelayToDo(delaySecond, toDo));
        }

        /// <summary>
        /// Loop a action until a condition is true.
        /// </summary>
        /// <param name="condition">The condition of keep looping.</param>
        /// <param name="toDo">Action that will loop per 'loopInterval' while condition is true.</param>
        /// <param name="loopInterval">Interval second of each loop.</param>
        /// <param name="endAction">Action that will be invoke untill condition false.</param>
        public static Coroutine LoopUntil(this MonoBehaviour self, Func<bool> condition, Action toDo, float loopInterval = 0.0f, Action endAction = null)
        {
            return self.StartCoroutine(LoopUntil(condition, toDo, loopInterval, endAction));
        }

        public static Coroutine WaitUntilToDo(this MonoBehaviour self, Func<bool> condition, Action toDo)
        {
            return self.StartCoroutine(WaitUntilToDo(condition, toDo));
        }

        public static Coroutine WaitUntilToDoWithTimeout(this MonoBehaviour self, Func<bool> condition, Action toDo, float timeout = 2.0f)
        {
            float timer = Time.time;
            return self.StartCoroutine(WaitUntilToDo(() => condition.Invoke() || Time.time - timer >= timeout, toDo));
        }

        public static T TryAddComponent<T>(this GameObject self) where T : MonoBehaviour
        {
            if (self.GetComponent<T>() == null)
            {
                self.AddComponent<T>();
            }
            return self.GetComponent<T>();
        }

        /// <summary>
        /// Convert Texture2D to Sprite.
        /// </summary>
        public static Sprite ToSprite(this Texture2D self)
        {
            Sprite spriteTemp = Sprite.Create(self, new Rect(0, 0, self.width, self.height), new Vector2(0.5f, 0.5f));
            spriteTemp.name = self.name;
            return spriteTemp;
        }

        /// <summary>
        /// Get a WaitForSecond instance. To avoid GC memory uses.
        /// </summary>
        public static WaitForSeconds GetWaitForSecond(float time)
        {
            if (!waitForSeconds.ContainsKey(time))
            {
                waitForSeconds.Add(time, new WaitForSeconds(time));
            }
            return waitForSeconds[time];
        }

        private static IEnumerator WaitUntilToDo(Func<bool> condition, Action toDo)
        {
            yield return new WaitUntil(condition);
            toDo();
        }

        private static IEnumerator DelayToDo(float delaySecond, Action toDo)
        {
            yield return GetWaitForSecond(delaySecond);
            toDo();
        }

        private static IEnumerator LoopUntil(Func<bool> condition, Action toDo, float loopInterval = 0.0f, Action endAction = null)
        {
            while (!condition.Invoke())
            {
                toDo?.Invoke();
                yield return (loopInterval == 0.0f ? null : GetWaitForSecond(loopInterval));
            }
            endAction?.Invoke();
        }
    }
}
