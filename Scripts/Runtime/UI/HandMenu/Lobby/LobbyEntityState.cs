using System;
using Unity.Netcode;

public struct LobbyEntityState : INetworkSerializable, IEquatable<LobbyEntityState>
{
    public ulong ClientId;
    public bool IsHost;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref IsHost);
    }

    public bool Equals(LobbyEntityState other)
    {
        return ClientId == other.ClientId &&
            IsHost == other.IsHost;
    }
}
