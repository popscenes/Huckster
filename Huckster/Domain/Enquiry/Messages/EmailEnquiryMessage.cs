using infrastructure.Messaging;

namespace Domain.Enquiry.Messages
{
    public class EmailEnquiryMessage : IMessage
    {
        public Enquiry Enquiry { get; set; }
        public string QueueName {
            get { return "emailenquirymessage"; }
            set { }
        }
    }
}