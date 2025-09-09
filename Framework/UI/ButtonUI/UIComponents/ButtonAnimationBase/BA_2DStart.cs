using DG.Tweening;
using UnityEngine;

namespace Txx.Base
{
    public class BA_2DStart : ButtonAnimationBase
    {
        public Transform targetTransform; // 要执行动画的目标物体的Transform组件

        [Range(0, 3)] public float selcetScale = 1.1f;
        [Range(0, 1)] public float selcetDuration = 0.2f; // 动画持续时间

        public bool original;

        [Range(0, 1)] public float newScale = 0.2f; // 不采用初始缩放

        private float originalScale; // 原始位置

        void Start()
        {
            if (targetTransform == null)
            {
                //if (transform.childCount != 0)
                //{
                //    targetTransform = transform.GetChild(0);
                //}
                //else
                //{
                targetTransform = transform;
                //}
            }

            if (!original)
                originalScale = targetTransform.localScale.x;
            else
                originalScale = newScale;
        }

        public override void SelcetAnimation()
        {
            targetTransform.DOScale(selcetScale, selcetDuration);
        }

        public override void PressAnimation()
        {
        }

        public override void LeaveAnimation()
        {
            Debug.Log(originalScale);
            targetTransform.DOScale(originalScale, selcetDuration);
        }
    }
}