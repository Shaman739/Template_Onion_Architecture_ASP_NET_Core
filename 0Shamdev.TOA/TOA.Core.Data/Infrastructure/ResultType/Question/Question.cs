using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question
{

    public class Question
    {
        public Question()
        {
            Buttons = new List<ButtonQuestion>();
        }
        public string Id { get; set; }
        public string Message { get; set; }

        public List<ButtonQuestion> Buttons { get; set; }

        public String Result { get; set; }

    }
    public class ButtonQuestion
    {
        //private ButtonQuestion()
        //{

        //}
        public ButtonQuestion(string label)
        {
            Label = label;
        }

        public String Label { get;  set; }

    }
    public class ButtonQuestionWithResult : ButtonQuestion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="result">константа из ResultQuestionType</param>
        public ButtonQuestionWithResult(string label,string result) : base(label)
        {
            Result = result;
        }
        public String Result { get; set; }
    }
    public class WarningQuestion : Question
    {
        public WarningQuestion() : base()
        {
            Buttons.Add(new ButtonQuestion("ОК"));
        }
    }

    public class QuestionYesNo : Question
    {
        public QuestionYesNo() : base()
        {
            Buttons.Add(new ButtonQuestionWithResult("Да", ResultQuestionType.YES));
            Buttons.Add(new ButtonQuestionWithResult("Нет", ResultQuestionType.NO));
        }

    }

    public class ResultQuestionType
    {

        public const string YES = "YES";
        public const string NO = "NO";
    }

}
