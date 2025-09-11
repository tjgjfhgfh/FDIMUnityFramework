using System.Collections;
using UnityEngine;

namespace Txx.Base
{
    public abstract class ButtonAnimationBase : MonoBehaviour
    {
        public abstract void SelcetAnimation();
        public abstract void PressAnimation();

        public virtual void LeaveAnimation()
        {
        }
    }
}