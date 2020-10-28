namespace Shamdev.TOA.BLL.Infrastructure
{
    /// <summary>
    /// Типы CRUD
    /// </summary>
    public class ExecuteTypeConstCRUD
    {
        public static ExecuteTypeConstCRUD ADD { get; } = new ExecuteTypeConstCRUD(1);
        public static ExecuteTypeConstCRUD EDIT { get; } = new ExecuteTypeConstCRUD(2);
        public static ExecuteTypeConstCRUD DELETE { get; } = new ExecuteTypeConstCRUD(3);

        public byte Value { get; private set; }
        private ExecuteTypeConstCRUD(byte value)
        {
            Value = value;
        }
    }
}
