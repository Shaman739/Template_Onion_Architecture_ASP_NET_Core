using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using Shamdev.TOA.Core.Data;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD
{
    /// <summary>
    /// Параметры запроса для CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DefaultParamOfCRUDOperation<TEntity>
        where TEntity : DomainObject
    {
        /// <summary>
        /// Объект для CRUD
        /// </summary>
        public TEntity Item { get; set; }

        /// <summary>
        /// Вопросы 
        /// </summary>
        public List<Question> Questions { get; set; }

        public bool IsSendAndAnswerQuestion(string query,string result, BaseResultType baseResultType)
        {
            Question question = Questions?.FirstOrDefault(x => x.Id == Item.CustomIdentity);
            if (question != null && question.Result == result)
            {
                //Ответ получили, но он не устраивает для проверки.
                return false;
            }
            else
            {
                if (question == null)
                {
                    //Добавляем воспрос
                   // Item.CustomIdentity = Guid.NewGuid().ToString();
                    question = new QuestionYesNo()
                    {
                        Id = Item.CustomIdentity,
                        Message = query
                    };
                    baseResultType.AddQuestion((QuestionYesNo)question);
                }
                return true;
            }
        }
    }
}
