using System;
using UnityEngine;

namespace Utils
{
    public class FaceCamera : MonoBehaviour
    {
        [Header("摄像机设置")]
        [Tooltip("如果为空，将自动使用主摄像机")]
        public Transform targetCamera;
    
        [Header("朝向选项")]
        [Tooltip("启用后物体会平滑旋转朝向摄像机")]
        public bool smoothRotation = true;
        [Tooltip("旋转速度（仅在平滑旋转启用时有效）")]
        public float rotationSpeed = 5f;
    
        [Tooltip("锁定X轴旋转")]
        public bool lockXAxis = true;
        [Tooltip("锁定Z轴旋转")]
        public bool lockZAxis = true;

        private void Start()
        {
            // 自动获取主摄像机
            if (targetCamera == null && Camera.main != null)
            {
                targetCamera = Camera.main.transform;
            }
        }

        private void FixedUpdate()
        {
            FacingCamera();
        }

        private void FacingCamera()
        {
            if (targetCamera == null) return;

            // 计算目标朝向
            Vector3 directionToCamera = targetCamera.position - transform.position;
        
            // 在2D环境中，通常只考虑Z轴旋转
            if (lockXAxis) directionToCamera.x = 0;
            if (lockZAxis) directionToCamera.z = 0;
        
            // 计算目标旋转
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
        
            // 应用旋转
            if (smoothRotation)
            {
                // 平滑旋转
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                    rotationSpeed * Time.deltaTime);
            }
            else
            {
                // 直接旋转
                transform.rotation = targetRotation;
            }
        }
    
        // 在编辑器模式下实时预览效果
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;
        
            Transform cam = targetCamera;
            if (cam == null && Camera.main != null)
                cam = Camera.main.transform;
            
            if (cam != null)
            {
                Vector3 dir = cam.position - transform.position;
                if (lockXAxis) dir.x = 0;
                if (lockZAxis) dir.z = 0;
            
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, dir.normalized * 2f);
            }
        }
    }
}