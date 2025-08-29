using System;
using System.Collections.Generic;
using System.Text;

namespace SpecialEducationPlanning
.Business.IService
{
    public interface IPostCodeServiceFactory
    {

        IPostCodeService GetService(string countryCode);
    }
}
