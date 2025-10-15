using System;

namespace FDIM.Framework
{
    public static class ClassExtension
    {
        public static T Invoke<T>(this T self, Action<T> action) where T : class
        {
            if (self != null)
                action(self);
            return self;
        }
    }
}