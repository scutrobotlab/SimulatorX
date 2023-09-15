namespace Gameplay.Embedded.CustomizeUI
{
    /// <summary>
    /// 删除自定义图形操作
    /// </summary>
    public enum DeleteCustomGraphicOperation
    {
        Null = 0,
        DeleteLayer = 1,
        DeleteAll = 2
    }

    /// <summary>
    /// 删除自定义图形
    /// </summary>
    public class DeleteCustomGraphicData : InteractiveData
    {
        public DeleteCustomGraphicOperation Operation
        {
            get => (DeleteCustomGraphicOperation)Data[0];
            set => Data[0] = (byte)value;
        }

        public int Layer
        {
            get => Data[1];
            set => Data[1] = (byte)value;
        }

        public DeleteCustomGraphicData(byte[] bytes) : base(bytes)
        {
        }

        public DeleteCustomGraphicData(InteractiveHeader header, byte[] data) : base(header, data)
        {
        }

        public override DataCommandId GetCommandId() => DataCommandId.DeleteCustomGraphic;
    }
}