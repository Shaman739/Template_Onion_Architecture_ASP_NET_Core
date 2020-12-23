using System;
using System.Collections.Generic;
using System.Text;

namespace Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question
{
    public abstract class Question
    {
        public Question()
        {
            Buttons = new List<ButtonQuestion>();
        }
        public int Id { get; set; }
        public string Message { get; set; }
        public List<ButtonQuestion> Buttons { get; set; }

    }
    public class ButtonQuestion
    {
        private ButtonQuestion()
        {

        }
        public ButtonQuestion(string label)
        {
            Label = label;
        }
        public String Label { get; private set; }

    }
    public class WarningQuestion : Question
    {
        public WarningQuestion() : base()
        {
            Buttons.Add(new ButtonQuestion("ОК"));
        }
    }

}
