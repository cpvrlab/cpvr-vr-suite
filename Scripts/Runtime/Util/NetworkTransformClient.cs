using Unity.Netcode.Components;

namespace cpvr_vr_suite.Scripts.Runtime.Util
{
    /// <summary>
    /// A client authoritative network transform synchronizer.
    /// </summary>
    public class NetworkTransformClient : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
