
using Adroit_v8.Models.FormModel;
using Adroit_v8.MongoConnections;
using Adroit_v8.MongoConnections.LoanApplication;
using static Adroit_v8.Config.Helper;

using static Adroit_v8.EnumFile.EnumHelper;
namespace Adroit_v8.Repository
{
    public interface IFilterRepository
    {
        IQueryable<RegularLoan> GetLoanFilter(FilFormModelIn obj);
    }
    public class FilterRepository : IFilterRepository
    {
        private readonly IAdroitRepository<RegularLoan> _repo;
        public FilterRepository(IAdroitRepository<RegularLoan> repo)
        {
            _repo = repo;
        }

        public IQueryable<RegularLoan> GetLoanFilter(FilFormModelIn obj)
        {
            IQueryable<RegularLoan>? neRes = null;
            try
            {
                neRes = obj.FilterDet.ToLower() switch
                {
                    "customer" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Under_Review && (
                    o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    "declined" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Declined && (
                    o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    "adjust" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Adjust && (o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    "review" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Review && (o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    "approved" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Approved && (o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    "disbursement" => _repo.AsQueryable().Where(
                   o => o.Status == (int)AdroitLoanApplicationStatus.Disburse && (o.DateCreated > obj.StartDate
                    && o.DateCreated < obj.EndDate.AddDays(1))
                    ),
                    _ => null
                };

                if (neRes.Any())
                {
                    switch (obj)
                    {
                        case { Channel: not null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                            neRes = neRes.Where(o => o.ApplicationChannel == obj.Channel);
                            break;
                        case { Channel: null, ApplicantName: not null, ApplicationId: null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                            neRes = neRes.Where(o => o.FirstName.Contains(obj.ApplicantName) || o.LastName.Contains(obj.ApplicantName));
                            break;
                        case { Channel: null, ApplicantName: null, ApplicationId: not null, Status: 0, EmailAddress: null, PhoneNumber: null }:
                            neRes = neRes.Where(o => o.LoanApplicationId == obj.ApplicationId);
                            break;
                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: not 0, EmailAddress: null, PhoneNumber: null }:
                            neRes = neRes.Where(o => o.Status == obj.Status);
                            break;
                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress: not null, PhoneNumber: null }:
                            neRes = neRes.Where(o => o.EmployerAddress == obj.EmailAddress);
                            break;
                        case { Channel: null, ApplicantName: null, ApplicationId: null, Status: 0, EmailAddress:  null, PhoneNumber: null, LoanCategory: not null }:
                            neRes = neRes.Where(o => o.LoanCategory == obj.LoanCategory);
                            break;
                        default:
                            Console.WriteLine("Unknown object");
                            break;
                    };
                }

                return neRes;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

}