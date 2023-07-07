using UnityEngine;

namespace Utils
{
    public static class MathExtensions
    {
        public static void ClampMagnitude(this ref Vector3 value, float min, float max)
        {
            if (value.magnitude < min)
                value = value.normalized * min;
            else if (value.magnitude > max)
                value = value.normalized * max;
        }
    }
}