using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.Util
{ }
/// <summary>
/// Utility methods for locating component instances.
/// Dan's note: Same as the one from XR Interaction Toolkit but accessible.
/// </summary>
/// <typeparam name="T">The component type.</typeparam>
static class ComponentLocatorUtility<T> where T : Component
{
    /// <summary>
    /// Cached reference to a found component of type <see cref="T"/>.
    /// </summary>
    static T s_ComponentCache;

    /// <summary>
    /// Cached reference to a found component of type <see cref="T"/>.
    /// </summary>
    internal static T ComponentCache => s_ComponentCache;

    public static T FindOrCreateComponent()
    {
        if (s_ComponentCache == null)
        {
            s_ComponentCache = Object.FindFirstObjectByType<T>();

            if (s_ComponentCache == null)
                s_ComponentCache = new GameObject(typeof(T).Name, typeof(T)).GetComponent<T>();
        }

        return s_ComponentCache;
    }

    public static T FindComponent()
    {
        TryFindComponent(out var component);
        return component;
    }

    public static bool TryFindComponent(out T component)
    {
        if (s_ComponentCache != null)
        {
            component = s_ComponentCache;
            return true;
        }

        s_ComponentCache = Object.FindFirstObjectByType<T>();
        component = s_ComponentCache;
        return component != null;
    }
}
