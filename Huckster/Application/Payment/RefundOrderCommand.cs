using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Payment;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Application.Payment
{
    public class RefundOrderCommand: ICommand
    {
        public int OrderId { get; set; }
        public Guid OrderAggregateRootId { get; set; }
    }

    public class RefundOrderCommandHandler : AdoCommandHandler<RefundOrderCommand>
    {
        private readonly PaypalService _paypalService;
        private readonly StripeService _stripeService;

        public RefundOrderCommandHandler(AdoContext adoContext, PaypalService paypalService, StripeService stripeService) : base(adoContext)
        {
            _paypalService = paypalService;
            _stripeService = stripeService;
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, RefundOrderCommand command)
        {
            var paymentEvents = context.Query<PaymentEvent>("Select * from [dbo].[PaymentEvent] where ParentAggregateId = @AggregateRootId", 
                new { AggregateRootId = command.OrderAggregateRootId }).ToList();

            if (!paymentEvents.Any())
            {
                throw new ApplicationException($"No Payment Events for Order {command.OrderId} {command.OrderAggregateRootId}");
            }

            PaymentEvent refunfPayementEvent = null;
            var gateway = paymentEvents.FirstOrDefault().Gateway;
            if (gateway.Equals("Paypal"))
            {
                refunfPayementEvent = await _paypalService.Refund(paymentEvents.FirstOrDefault().ExternalId);
            }
            else if (gateway.Equals("Stripe"))
            {
                refunfPayementEvent = await _stripeService.Refund(paymentEvents.FirstOrDefault().ExternalId);
            }

            refunfPayementEvent.ParentAggregateId = command.OrderAggregateRootId;
            context.Insert(refunfPayementEvent);

        }
    }
}