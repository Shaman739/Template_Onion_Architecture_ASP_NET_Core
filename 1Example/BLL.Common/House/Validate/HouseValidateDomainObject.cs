using Shamdev.ERP.Core.Data.Infrastructure.ResultType.Question;
using Shamdev.TOA.BLL.Validate.Interface;
using Shamdev.TOA.Core.Data.Infrastructure.ResultType;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Common.House.Validate
{
    public class HouseValidateDomainObject : IValidateDomainObject<Core.Data.Domain.House>
    {
        public BaseResultType Validate(Core.Data.Domain.House item)
        {
            BaseResultType baseResultType = new BaseResultType() { IsSuccess = true };
            if (item.CountOfEntrance == null)
            {
                WarningQuestion question = new WarningQuestion()
                {
                    Id = 1,
                    Message = "Отсутствует количество подъездов"
                };
                baseResultType.AddQuestion(question);
            }
            if (item.CountOfFloor == null)
            {
                WarningQuestion question = new WarningQuestion()
                {
                    Id = 1,
                    Message = "Отсутствует количество этажей"
                };
                baseResultType.AddQuestion(question);
            }
            return baseResultType;
        }
    }

}
