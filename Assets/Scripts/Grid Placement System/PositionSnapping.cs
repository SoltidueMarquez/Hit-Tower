using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grid_Placement_System
{
    [ExecuteInEditMode]
    public class PositionSnapping : MonoBehaviour
    {
        [Tooltip("每个轴向上的对齐步长")] 
        private readonly Vector2 m_SnapStep = new Vector2(ConstManager.k_GridSize, ConstManager.k_GridSize);
        [LabelText("高度偏差值")] private const float k_PreviewYOffset = ConstManager.k_PreviewYOffset;

        private Vector3 m_LastPosition;
        
        [Header("网格可视化设置")]
        [SerializeField] private bool showGrid = true;
        [SerializeField] private Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        [SerializeField] private int gridSize = 10; // 网格显示范围（从中心向四周扩展的格子数）

        [Header("防重叠设置")]
        [SerializeField] private bool enableAntiOverlap = true;
        [SerializeField] private LayerMask overlapCheckLayer = -1; // 检查哪些层的物体
        [SerializeField] private Vector3 objectSize = Vector3.one; // 物体占用的大小

#if UNITY_EDITOR
        private void OnEnable()
        {
            // 订阅编辑器更新事件
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            // 取消订阅，防止内存泄漏
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (!Application.isPlaying && transform.position != m_LastPosition)
            {
                SnapPosition();
                m_LastPosition = transform.position;
            }
        }
#endif

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGrid) return;
            
            // 保存原来的Gizmos设置
            Color originalColor = Gizmos.color;
            Matrix4x4 originalMatrix = Gizmos.matrix;
            
            // 设置网格颜色
            Gizmos.color = gridColor;
            
            // 以当前物体位置为中心绘制网格，并偏移半个网格大小
            Vector3 center = transform.position;
            float gridSizeX = m_SnapStep.x;
            float gridSizeZ = m_SnapStep.y;
            
            // 添加半个网格的偏移量，使地块中心位于网格中心而非交叉点
            Vector3 offsetCenter = new Vector3(
                center.x - gridSizeX * 0.5f,
                center.y,
                center.z - gridSizeZ * 0.5f
            );
            
            // 计算网格绘制范围
            float startX = offsetCenter.x - gridSize * gridSizeX;
            float endX = offsetCenter.x + gridSize * gridSizeX;
            float startZ = offsetCenter.z - gridSize * gridSizeZ;
            float endZ = offsetCenter.z + gridSize * gridSizeZ;
            
            // 绘制X方向的网格线（平行于Z轴）
            for (int i = -gridSize; i <= gridSize; i++)
            {
                float x = offsetCenter.x + i * gridSizeX;
                Vector3 start = new Vector3(x, 0.02f, startZ);
                Vector3 end = new Vector3(x, 0.02f, endZ);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制Z方向的网格线（平行于X轴）
            for (int i = -gridSize; i <= gridSize; i++)
            {
                float z = offsetCenter.z + i * gridSizeZ;
                Vector3 start = new Vector3(startX, 0.02f, z);
                Vector3 end = new Vector3(endX, 0.02f, z);
                Gizmos.DrawLine(start, end);
            }
            
            // 绘制当前所在的格子（高亮显示）
            Gizmos.color = Color.green;
            
            // 修正高亮格子的位置计算，使其与偏移后的网格对齐
            Vector3 snappedPos = new Vector3(
                Mathf.Round(center.x / gridSizeX) * gridSizeX - gridSizeX * 0.5f,
                0.02f,
                Mathf.Round(center.z / gridSizeZ) * gridSizeZ - gridSizeZ * 0.5f
            );
            
            // 绘制当前格子的边框
            Vector3[] corners = new Vector3[]
            {
                new Vector3(snappedPos.x, 0.02f, snappedPos.z),
                new Vector3(snappedPos.x + gridSizeX, 0.02f, snappedPos.z),
                new Vector3(snappedPos.x + gridSizeX, 0.02f, snappedPos.z + gridSizeZ),
                new Vector3(snappedPos.x, 0.02f, snappedPos.z + gridSizeZ)
            };
            
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }

            // 绘制物体占用区域的Gizmo
            if (enableAntiOverlap)
            {
                Gizmos.color = new Color(1, 0, 0, 0.3f);
                Gizmos.DrawWireCube(transform.position + Vector3.up * objectSize.y * 0.5f, objectSize);
            }
            
            // 恢复原来的Gizmos设置
            Gizmos.color = originalColor;
            Gizmos.matrix = originalMatrix;
        }
#endif

        private void SnapPosition()
        {
            if (m_SnapStep.x <= 0 || m_SnapStep.y <= 0)
            {
                Debug.LogWarning("对齐步长必须大于0", this);
                return;
            }

            var currentPosition = transform.position;
            var snappedPosition = new Vector3(
                Mathf.Round(currentPosition.x / m_SnapStep.x) * m_SnapStep.x,
                k_PreviewYOffset,
                Mathf.Round(currentPosition.z / m_SnapStep.y) * m_SnapStep.y
            );

            // 如果启用了防重叠功能，检查并处理重叠
            if (enableAntiOverlap)
            {
                snappedPosition = FindNonOverlappingPosition(snappedPosition);
            }

            if (currentPosition == snappedPosition) return;
            
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(transform, "Snap Position");
#endif
            transform.position = snappedPosition;
        }

        /// <summary>
        /// 寻找不重叠的位置
        /// </summary>
        private Vector3 FindNonOverlappingPosition(Vector3 targetPosition)
        {
            Vector3 checkPosition = targetPosition;
            int maxAttempts = 10; // 最大尝试次数，防止无限循环
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                if (!IsOverlapping(checkPosition))
                {
                    return checkPosition; // 找到不重叠的位置
                }
                
                // 向右边偏移一格（X轴减少一个网格步长）
                checkPosition.x += m_SnapStep.x;
                
                // 可选：如果向左找不到，也可以尝试其他方向
                // 比如：checkPosition.z += m_SnapStep.y; // 向上偏移
                // 或者实现更复杂的搜索逻辑
            }
            
            // 如果所有尝试都失败，返回原始位置并显示警告
            Debug.LogWarning($"无法为 {gameObject.name} 找到不重叠的位置，已尝试 {maxAttempts} 次", this);
            return targetPosition;
        }

        /// <summary>
        /// 检查指定位置是否会与其他同脚本物体重叠
        /// </summary>
        private bool IsOverlapping(Vector3 position)
        {
            // 方法1：使用物理碰撞检测
            Collider[] colliders = Physics.OverlapBox(
                position + Vector3.up * objectSize.y * 0.5f,
                objectSize * 0.5f,
                Quaternion.identity,
                overlapCheckLayer
            );

            if ((from collider in colliders 
                    where collider.gameObject != gameObject 
                    select collider.GetComponent<PositionSnapping>()).Any(otherSnapping => otherSnapping != null))
            {
                return true; // 发现重叠的同脚本物体
            }

            // 方法2：直接查找所有同脚本物体进行位置比较（备用方法）
            PositionSnapping[] allSnappingObjects = FindObjectsOfType<PositionSnapping>();
            return (from snappingObj in allSnappingObjects 
                where snappingObj != this 
                select Vector3.Distance(snappingObj.transform.position, position))
                .Any(distance => distance < Mathf.Min(m_SnapStep.x, m_SnapStep.y) * 0.9f);
        }

        /// <summary>
        /// 手动触发防重叠检查（可以在需要时调用）
        /// </summary>
        public void CheckAndResolveOverlap()
        {
            if (enableAntiOverlap)
            {
                Vector3 newPosition = FindNonOverlappingPosition(transform.position);
                if (newPosition != transform.position)
                {
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, "Resolve Overlap");
#endif
                    transform.position = newPosition;
                }
            }
        }
    }
}