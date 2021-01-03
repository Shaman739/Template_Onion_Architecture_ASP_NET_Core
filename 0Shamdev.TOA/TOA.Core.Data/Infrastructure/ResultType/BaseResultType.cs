using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Shamdev.TOA.Core.Data.Infrastructure.ResultType
{
    public class BaseResultType<T> : BaseResultType
        where T : class, new()
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
            Status = ResultStatus.Success;
        }
        public ResultStatus Status { get; set; }

        public string Message { get; set; }
        public List<Question> Question { get; private set; }

        /// <summary>
        /// Merge двух результатов. Если у одного результат IsSuccess=false, то в итоге общий результат будет IsSuccess = false, но сообщения соединятся.
        /// </summary>
        /// <param name="resultValidate"></param>
        public void Merge(BaseResultType resultValidate)
        {
            AddMessage(resultValidate.Message);
            Status = MergeStatus(Status, resultValidate.Status);
            if (Question == null)
                Question = new List<Question>();
            if (resultValidate.Question != null)
                Question.AddRange(resultValidate.Question);
        }
        private ResultStatus MergeStatus(ResultStatus target, ResultStatus source)
        {
            if (source == ResultStatus.Fail || target == ResultStatus.Fail)
                return ResultStatus.Fail;
            else if (source == ResultStatus.Question || target == ResultStatus.Question)
                return ResultStatus.Question;
            else if (source == ResultStatus.Success && target == ResultStatus.Success)
                return ResultStatus.Success;
            else
                return ResultStatus.Fail;

        }
        public void AddError(string newMessage)
        {
            this.Status = ResultStatus.Fail;
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

        public void AddWarring(WarningQuestion question)
        {
            if (Question == null)
                Question = new List<Question>();
            if (!Question.Any(x => x is QuestionYesNo))
                Question.Add(question);

        }

        public void AddQuestion(QuestionYesNo question)
        {
            if (Question == null)
                Question = new List<Question>();
            if (Question.Any(x => x is QuestionYesNo))

                Question = Question.Where(x => x is QuestionYesNo).ToList();

            Status = ResultStatus.Question;
            Question.Add(question);

        }


    }

    public enum ResultStatus
    {
        [EnumMember(Value = "Success")]
        Success,
        [EnumMember(Value = "Fail")]
        Fail,
        [EnumMember(Value = "Question")]
        Question

    }

}
