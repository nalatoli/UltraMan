using Unity.Netcode;

public struct InputState : INetworkSerializable
{
    public bool IsMoveLeftKeyDown;
    public bool IsMoveUpKeyDown;
    public bool IsMoveRightKeyDown;
    public bool IsMoveDownKeyDown;
    public bool IsBasicFireKeyPressed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref IsMoveLeftKeyDown);
        serializer.SerializeValue(ref IsMoveUpKeyDown);
        serializer.SerializeValue(ref IsMoveRightKeyDown);
        serializer.SerializeValue(ref IsMoveDownKeyDown);
        serializer.SerializeValue(ref IsBasicFireKeyPressed);
    }
}