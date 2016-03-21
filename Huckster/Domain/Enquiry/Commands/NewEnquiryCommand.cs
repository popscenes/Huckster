using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Enquiry.Commands
{
    public class NewEnquiryCommand: ICommand
    {
        public Enquiry Enquiry { get; set; }
    }

    public class NewEnquiryCommandHandler : AdoCommandHandler<NewEnquiryCommand>
    {
        public NewEnquiryCommandHandler(AdoContext adoContext) : base(adoContext)
        {

        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, NewEnquiryCommand command)
        {
            context.Insert(command.Enquiry);
        }
    }
}