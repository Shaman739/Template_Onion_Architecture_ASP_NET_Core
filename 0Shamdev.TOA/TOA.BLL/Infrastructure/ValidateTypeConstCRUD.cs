namespace Shamdev.TOA.BLL.Infrastructure
{
    /// <summary>
    /// Типы валидации при CRUD операциях
    /// </summary>
    public class ValidateTypeConstCRUD
    {
        public static ValidateTypeConstCRUD ADD_OR_EDIT { get; } = new ValidateTypeConstCRUD(1);
        public static ValidateTypeConstCRUD DELETE { get; } = new ValidateTypeConstCRUD(3);

        public byte Value { get; private set; }
        private ValidateTypeConstCRUD(byte value)
        {
            Value = value;
        }
    }
}
