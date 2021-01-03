using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using Shamdev.TOA.BLL.Infrastructure.ParamOfCRUD;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House.Validate
{
    public class HouseValidateDomainObject : IValidateDomainObject<Core.Data.Domain.House>
    {
        public BaseResultType Validate(DefaultParamOfCRUDOperation<Core.Data.Domain.House> item)
        {
            BaseResultType baseResultType = new BaseResultType() { Status = ResultStatus.Success };
            if (item.Item.CountOfEntrance == null)
            {
                //WarningQuestion question = new WarningQuestion()
                //{
                //    Id = "1",
                //    Message = "Отсутствует количество подъездов"
                //};
                //baseResultType.AddWarring(question);
                if(!item.IsSendAndAnswerQuestion("Отсутствует количество подъездов.Продолжить?", ResultQuestionType.NO, baseResultType))
                 baseResultType.AddError(" Отмена");
            }
            if (item.Item.CountOfFloor == null)
            {
                WarningQuestion question = new WarningQuestion()
                {
                    Id = "1",
                    Message = "Отсутствует количество этажей"
                };
                baseResultType.AddWarring(question);
            }
            return baseResultType;
        }
    }

}
