using Unity.Netcode;
using UnityEngine;

namespace Serializable {
    /// <summary>
    /// Used to send teleportation position over network.
    /// </summary>
    public struct PositionsData : INetworkSerializable {
        public Vector3[] Positions;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            // Positions length
            int length = 0;
            
            if (!serializer.IsReader)
                length = Positions.Length;
            
            serializer.SerializeValue(ref length);

            // Positions value
            if (serializer.IsReader)
                Positions = new Vector3[length];
            
            for(int i = 0; i < length; ++i)
                serializer.SerializeValue(ref Positions[i]);
        }
    }
}