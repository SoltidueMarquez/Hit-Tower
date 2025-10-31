using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace ObjectPool
{
    /// <summary>
    /// 游戏对象池管理器，用于管理和复用GameObject实例
    /// 继承自Singleton<GameObjectPool>，确保全局唯一实例
    /// </summary>
    public class GameObjectPool : Singleton<GameObjectPool>
    {
        // 存储预制体到对象池队列的映射关系
        // Key: 预制体对象，Value: 该预制体对应的可用对象队列
        private readonly Dictionary<GameObject, Queue<GameObject>> m_PrefabToPool = new Dictionary<GameObject, Queue<GameObject>>();
        
        // 存储实例对象到其原始预制体的映射关系
        // Key: 实例对象，Value: 创建该实例的原始预制体
        private readonly Dictionary<GameObject, GameObject> m_InstanceToPrefab = new Dictionary<GameObject, GameObject>();

        /// <summary>
        /// 从对象池中获取一个游戏对象实例
        /// </summary>
        /// <param name="prefab">需要的预制体</param>
        /// <param name="parent">可选的父级变换，如果为null则使用池管理器作为父级</param>
        /// <returns>可用的游戏对象实例，如果prefab为null则返回null</returns>
        public GameObject Get(GameObject prefab, Transform parent = null)
        {
            // 安全检查：确保预制体不为空
            if (prefab == null)
            {
                Debug.LogError("GameObjectPool.Get 失败: prefab 为空");
                return null;
            }

            // 获取或创建该预制体对应的对象池队列
            if (!m_PrefabToPool.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                m_PrefabToPool[prefab] = queue;
            }

            GameObject instance;
            // 优先从对象池队列中获取可复用的实例
            if (queue.Count > 0)
            {
                instance = queue.Dequeue();
            }
            else
            {
                // 如果池中没有可用实例，则创建新的实例
                instance = Instantiate(prefab);
                // 记录新实例与其预制体的映射关系
                m_InstanceToPrefab[instance] = prefab;
            }

            // 设置实例的父级变换
            if (parent != null)
            {
                instance.transform.SetParent(parent, false);
            }

            // 激活实例并返回
            instance.SetActive(true);
            return instance;
        }

        /// <summary>
        /// 将游戏对象实例回收到对象池中
        /// </summary>
        /// <param name="instance">要回收的实例对象</param>
        public void Release(GameObject instance)
        {
            // 安全检查：实例为空时直接返回
            if (instance == null)
            {
                return;
            }

            // 检查实例是否由本对象池管理
            if (!m_InstanceToPrefab.TryGetValue(instance, out var prefab))
            {
                // 如果不是池管理的对象，直接销毁以避免内存泄漏
                Object.Destroy(instance);
                return;
            }

            // 获取或创建该预制体对应的对象池队列
            if (!m_PrefabToPool.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                m_PrefabToPool[prefab] = queue;
            }

            // 停用实例并重置其状态
            instance.SetActive(false);
            // 将实例移回池管理器的子级，避免场景层次结构混乱
            instance.transform.SetParent(transform, false);
            // 将实例重新加入对象池队列，供后续复用
            queue.Enqueue(instance);
        }

        /// <summary>
        /// 预热对象池，预先创建指定数量的实例
        /// </summary>
        /// <param name="prefab">要预热的预制体</param>
        /// <param name="count">预创建的实例数量</param>
        /// <param name="parent">可选的父级变换</param>
        public void Warm(GameObject prefab, int count, Transform parent = null)
        {
            // 参数检查：预制体为空或数量小于等于0时直接返回
            if (prefab == null || count <= 0) return;

            // 获取或创建该预制体对应的对象池队列
            if (!m_PrefabToPool.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                m_PrefabToPool[prefab] = queue;
            }

            // 循环创建指定数量的实例
            for (int i = 0; i < count; i++)
            {
                // 实例化预制体，设置父级为指定父级或池管理器
                var inst = Instantiate(prefab, parent != null ? parent : transform, false);
                // 记录实例与预制体的映射关系
                m_InstanceToPrefab[inst] = prefab;
                // 保持实例为未激活状态
                inst.SetActive(false);
                // 将实例加入对象池队列
                queue.Enqueue(inst);
            }
        }
    }
}