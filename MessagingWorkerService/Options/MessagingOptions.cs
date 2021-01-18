namespace MessagingWorkerService.Options
{
    public class MessagingOptions
    {
        public const string Messaging = "Messaging";

        public string ServiceBusConnectionString { get; set; }
        public string TopicNameToListen { get; set; }
        public string TopicNameToSend { get; set; }
        public string SubscriptionName { get; set; }
    }
}
