using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    public static class Extensions
    {
        public static bool TryExtractInterface<T>(this GameObject interfaceOwner, out T i)
            where T : class
        {
            i = null;
            var monos = interfaceOwner.GetComponents<MonoBehaviour>();
            foreach (var mono in monos)
            {
                if (mono is T tMono)
                {
                    i = tMono;
                    return true;
                }
            }
            return false;
        }
    }
}
