using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Enquiry.Quiries
{
    public class GetEnquiriesQuery: IQuery<GetEnquiriesQuery,List<Enquiry>>
    {
    }

    public class GetEnquiriesQueryHandler : AdoQueryHandler<GetEnquiriesQuery, List<Enquiry>>
    {
        public GetEnquiriesQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<List<Enquiry>> HandleSqlCommandAsync(IDbConnection context,
            GetEnquiriesQuery argument)
        {
            var enquiries = context.GetList<Enquiry>();
            return enquiries.ToList();
        }
    }
}