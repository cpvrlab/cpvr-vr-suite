using Unity.Netcode.Components;

namespace Util {
    /// <summary>
    /// A client authoritative network transform synchronizer.
    /// </summary>
    public class NetworkTransformClient : NetworkTransform {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}
