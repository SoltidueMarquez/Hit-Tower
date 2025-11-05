using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 协程辅助器 - 让非MonoBehaviour类也能使用协程
    /// </summary>
    public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper _instance;
        private static bool _isApplicationQuitting = false;
    
        /// <summary>
        /// 单例实例
        /// </summary>
        public static CoroutineHelper Instance
        {
            get
            {
                if (_isApplicationQuitting) 
                    return null;
                
                if (_instance == null)
                {
                    Initialize();
                }
                return _instance;
            }
        }
    
        // 存储所有运行的协程，便于管理
        private Dictionary<string, Coroutine> _runningCoroutines = new Dictionary<string, Coroutine>();
        private int _coroutineCounter = 0;
    
        /// <summary>
        /// 初始化协程辅助器
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance != null || _isApplicationQuitting) return;
        
            GameObject helperObject = new GameObject("CoroutineHelper");
            helperObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            _instance = helperObject.AddComponent<CoroutineHelper>();
            DontDestroyOnLoad(helperObject);
        }
    
        private void OnApplicationQuit()
        {
            _isApplicationQuitting = true;
            StopAllCoroutines();
            _runningCoroutines.Clear();
        }
    
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    
        /// <summary>
        /// 启动协程（自动生成唯一ID）
        /// </summary>
        public static Coroutine Start(IEnumerator coroutine)
        {
            if (Instance == null) return null;
            return Instance.StartCoroutine(coroutine);
        }
    
        /// <summary>
        /// 启动协程并指定ID，便于后续管理
        /// </summary>
        public static string Start(IEnumerator coroutine, string coroutineId)
        {
            if (Instance == null) return null;
        
            // 如果已存在相同ID的协程，先停止
            if (Instance._runningCoroutines.ContainsKey(coroutineId))
            {
                Instance.StopCoroutine(Instance._runningCoroutines[coroutineId]);
                Instance._runningCoroutines.Remove(coroutineId);
            }
        
            var newCoroutine = Instance.StartCoroutine(Instance.WrapCoroutine(coroutine, coroutineId));
            Instance._runningCoroutines[coroutineId] = newCoroutine;
            return coroutineId;
        }
    
        /// <summary>
        /// 启动协程并返回自动生成的ID
        /// </summary>
        public static string StartWithId(IEnumerator coroutine)
        {
            string id = $"Coroutine_{Instance._coroutineCounter++}";
            return Start(coroutine, id);
        }
    
        /// <summary>
        /// 停止指定ID的协程
        /// </summary>
        public static void Stop(string coroutineId)
        {
            if (Instance == null || !Instance._runningCoroutines.ContainsKey(coroutineId)) return;
        
            Instance.StopCoroutine(Instance._runningCoroutines[coroutineId]);
            Instance._runningCoroutines.Remove(coroutineId);
        }
    
        /// <summary>
        /// 停止所有由本辅助器启动的协程
        /// </summary>
        public static void StopAll()
        {
            if (Instance == null) return;
        
            foreach (var coroutine in Instance._runningCoroutines.Values)
            {
                if (coroutine != null)
                    Instance.StopCoroutine(coroutine);
            }
            Instance._runningCoroutines.Clear();
            Instance.StopAllCoroutines();
        }
    
        /// <summary>
        /// 检查指定ID的协程是否在运行
        /// </summary>
        public static bool IsRunning(string coroutineId)
        {
            return Instance != null && Instance._runningCoroutines.ContainsKey(coroutineId);
        }
    
        /// <summary>
        /// 获取所有运行中的协程ID
        /// </summary>
        public static List<string> GetRunningCoroutineIds()
        {
            if (Instance == null) return new List<string>();
            return new List<string>(Instance._runningCoroutines.Keys);
        }
    
        /// <summary>
        /// 包装协程，用于自动清理字典
        /// </summary>
        private IEnumerator WrapCoroutine(IEnumerator coroutine, string coroutineId)
        {
            yield return StartCoroutine(coroutine);
        
            // 协程完成后自动清理
            if (_runningCoroutines.ContainsKey(coroutineId))
            {
                _runningCoroutines.Remove(coroutineId);
            }
        }
    }

    /// <summary>
    /// 协程工具类 - 提供便捷的静态方法
    /// </summary>
    public static class CoroutineUtility
    {
        /// <summary>
        /// 延迟执行
        /// </summary>
        public static string Delay(float seconds, Action callback)
        {
            return CoroutineHelper.StartWithId(DelayRoutine(seconds, callback));
        }
    
        /// <summary>
        /// 按帧延迟执行
        /// </summary>
        public static string DelayFrames(int frames, Action callback)
        {
            return CoroutineHelper.StartWithId(DelayFramesRoutine(frames, callback));
        }
    
        /// <summary>
        /// 等待直到条件满足
        /// </summary>
        public static string WaitUntil(Func<bool> condition, Action callback)
        {
            return CoroutineHelper.StartWithId(WaitUntilRoutine(condition, callback));
        }
    
        /// <summary>
        /// 循环执行（可设置间隔）
        /// </summary>
        public static string Loop(float interval, Action callback, int loopCount = -1)
        {
            return CoroutineHelper.StartWithId(LoopRoutine(interval, callback, loopCount));
        }
    
        /// <summary>
        /// 渐变效果
        /// </summary>
        public static string Lerp(float duration, Action<float> onUpdate, Action onComplete = null)
        {
            return CoroutineHelper.StartWithId(LerpRoutine(duration, onUpdate, onComplete));
        }
    
        private static IEnumerator DelayRoutine(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback?.Invoke();
        }
    
        private static IEnumerator DelayFramesRoutine(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            callback?.Invoke();
        }
    
        private static IEnumerator WaitUntilRoutine(Func<bool> condition, Action callback)
        {
            yield return new WaitUntil(condition);
            callback?.Invoke();
        }
    
        private static IEnumerator LoopRoutine(float interval, Action callback, int loopCount)
        {
            int count = 0;
            while (loopCount == -1 || count < loopCount)
            {
                callback?.Invoke();
                yield return new WaitForSeconds(interval);
                count++;
            }
        }
    
        private static IEnumerator LerpRoutine(float duration, Action<float> onUpdate, Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                onUpdate?.Invoke(progress);
                elapsed += Time.deltaTime;
                yield return null;
            }
            onUpdate?.Invoke(1f);
            onComplete?.Invoke();
        }
    }
}