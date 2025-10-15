using UnityEngine;

namespace FDIM.Framework
{
    public static class TransformExtensions
    {
        public static Transform SetActiveEx(this Transform t, bool active)
        {
            t.gameObject.SetActive(active);
            return t;
        }

        public static Transform SetNameEx(this Transform t, string name)
        {
            t.gameObject.name = name;
            return t;
        }

        public static Transform SetParentEx(this Transform t, Transform parent, bool worldPositionStays = true)
        {
            t.SetParent(parent, worldPositionStays);
            return t;
        }

        public static Transform SetLayerEx(this Transform t, int layer)
        {
            t.gameObject.layer = layer;
            return t;
        }

        public static Transform SetTagEx(this Transform t, string tag)
        {
            t.gameObject.tag = tag;
            return t;
        }

        public static Transform SetPositionEx(this Transform t, Vector3 pos)
        {
            t.position = pos;
            return t;
        }


        public static Transform SetPositionX(this Transform t, float x)
        {
            Vector3 pos = t.position;
            pos.x = x;
            t.position = pos;
            return t;
        }

        public static Transform SetPositionY(this Transform t, float y)
        {
            Vector3 pos = t.position;
            pos.y = y;
            t.position = pos;
            return t;
        }

        public static Transform SetPositionZ(this Transform t, float z)
        {
            Vector3 pos = t.position;
            pos.z = z;
            t.position = pos;
            return t;
        }

        public static Transform SetLocalPositionEx(this Transform t, Vector3 pos)
        {
            t.localPosition = pos;
            return t;
        }



        public static Transform SetLocalPositioX(this Transform t, float x)
        {
            Vector3 localPos = t.localPosition;
            localPos.x = x;
            t.localPosition = localPos;
            return t;
        }

        public static Transform SetLocalPositioY(this Transform t, float y)
        {
            Vector3 localPos = t.localPosition;
            localPos.y = y;
            t.localPosition = localPos;
            return t;
        }

        public static Transform SetLocalPositioZ(this Transform t, float z)
        {
            Vector3 localPos = t.localPosition;
            localPos.z = z;
            t.localPosition = localPos;
            return t;
        }

        public static Transform SetRotationEx(this Transform t, Quaternion rot)
        {
            t.rotation = rot;
            return t;
        }

        public static Transform SetEulerAngles(this Transform t, Vector3 eulerAngles)
        {
            t.eulerAngles = eulerAngles;
            return t;
        }

        public static Transform SetEulerAnglesX(this Transform t, float x)
        {
            Vector3 eulerAngles = t.eulerAngles;
            eulerAngles.x = x;
            t.eulerAngles = eulerAngles;
            return t;
        }

        public static Transform SetEulerAnglesY(this Transform t, float y)
        {
            Vector3 eulerAngles = t.eulerAngles;
            eulerAngles.y = y;
            t.eulerAngles = eulerAngles;
            return t;
        }

        public static Transform SetEulerAnglesZ(this Transform t, float z)
        {
            Vector3 eulerAngles = t.eulerAngles;
            eulerAngles.z = z;
            t.eulerAngles = eulerAngles;
            return t;
        }

        public static Transform SetLocalRotationEx(this Transform t, Quaternion rot)
        {
            t.localRotation = rot;
            return t;
        }

        public static Transform SetLocalEulerAngles(this Transform t, Vector3 eulerAngles)
        {
            t.localEulerAngles = eulerAngles;
            return t;
        }

        public static Transform SetLocalEulerAnglesX(this Transform t, float x)
        {
            Vector3 localEulerAngles = t.localEulerAngles;
            localEulerAngles.x = x;
            t.localEulerAngles = localEulerAngles;
            return t;
        }

        public static Transform SetLocalEulerAnglesY(this Transform t, float y)
        {
            Vector3 localEulerAngles = t.localEulerAngles;
            localEulerAngles.y = y;
            t.localEulerAngles = localEulerAngles;
            return t;
        }

        public static Transform SetLocalEulerAnglesZ(this Transform t, float z)
        {
            Vector3 localEulerAngles = t.localEulerAngles;
            localEulerAngles.z = z;
            t.localEulerAngles = localEulerAngles;
            return t;
        }

        public static Transform SetScaleEx(this Transform t, Vector3 scale)
        {
            t.localScale = scale;
            return t;
        }
    }
}