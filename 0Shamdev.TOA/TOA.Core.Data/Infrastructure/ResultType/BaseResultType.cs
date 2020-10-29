using System;

namespace Shamdev.TOA.Core.Data.Infrastructure.ResultType
{
    public class BaseResultType<T> : BaseResultType
        where T : class,new()
    {
        public BaseResultType()
        {
            Data = new T();
        }
        public T Data { get; set; }
    }
    /// <summary>
    /// Базовый класс для возврата результата обработки какой-то 
    /// </summary>
    public class BaseResultType
    {
        public BaseResultType()
        {
            IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Merge двух результатов. Если у одного результат IsSuccess=false, то в итоге общий результат будет IsSuccess = false, но сообщения соединятся.
        /// </summary>
        /// <param name="resultValidate"></param>
        public void Merge(BaseResultType resultValidate)
        {
            AddMessage(resultValidate.Message);
            this.IsSuccess &= resultValidate.IsSuccess;
        }

        public void AddError(string newMessage)
        {
            this.IsSuccess = false;
            AddMessage(newMessage);
        }

        public void AddMessage(string newMessage)
        {
            Message = AddMessageWithNewLine(Message, newMessage);
        }

        private string AddMessageWithNewLine(string sourceMessage, string newMessage)
        {
            //Если строка не пустая, то добавляем перенос строки, что было
            // не "Message1Message2"
            // а "Message1
            //    Message2"
            if (!String.IsNullOrWhiteSpace(sourceMessage))
                sourceMessage += Environment.NewLine;
            sourceMessage += newMessage;

            return sourceMessage;
        }
    }
}
