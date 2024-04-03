using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.UnderWriterModel;
using AutoMapper;

namespace Adroit_v8.Models
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Login, LoginRootBase> ();
         //   CreateMap<RegularLoanChargePost, RegularLoanCharge> ();
            CreateMap<StaffLoanCreateModel, StaffLoanModel> ();
            CreateMap<StaffLoanInterestRatenCreateModel, StaffLoanInterestRateModel> ();
           // CreateMap<RegularLoanInterestRatePost, RegularLoanInterestRate> ();
        }
    }
}
