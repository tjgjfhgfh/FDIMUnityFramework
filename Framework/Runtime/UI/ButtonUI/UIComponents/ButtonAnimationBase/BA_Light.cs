using DG.Tweening;
using UnityEngine;

namespace Txx.Base
{
    public class BA_Light : ButtonAnimationBase
    {
        public Transform targetTransform; // 要执行动画的目标物体的Transform组件

        public float pressSpeed = 0.3f; // 按下的速度
        public float releaseSpeed = 0.2f; // 弹起的速度

        public int passAudioId = 9004;
        public int upAudioId = 9005;

        private AudioClip _passClip, _upClip;

        // public float pressDepth = 0.1f; // 按钮被认为按下的阈值
        private Sequence _sequence;

        void Start()
        {
            //targetTransform.gameObject.SetActive(false);
            //if (_passClip == null)
            //{
            //    AudioInfo audioInfo =TxxBase.Data.GetAudioInfoByID(passAudioId.ToString());
            //    _passClip = TxxBase.Resource.LoadResource<AudioClip>(audioInfo.path,assetBundle:"base");
            //}
            //if (_upClip == null)
            //{
            //    AudioInfo audioInfo =TxxBase.Data.GetAudioInfoByID(upAudioId.ToString());
            //    _upClip = TxxBase.Resource.LoadResource<AudioClip>(audioInfo.path,assetBundle:"base");
            //}
        }

        public override void SelcetAnimation()
        {
            targetTransform.gameObject.SetActive(true);
        }

        public override void PressAnimation()
        {
            if (_sequence != null)
                _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(targetTransform.DOLocalMoveZ(targetTransform.localScale.z, pressSpeed));
            _sequence.Append(targetTransform.DOLocalMoveZ(0, releaseSpeed));
        }

        public override void LeaveAnimation()
        {
            targetTransform.gameObject.SetActive(false);
        }
    }
}