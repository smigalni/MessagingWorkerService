using System;

namespace MessagingWorkerService.DtoModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTimeOffset OrderDate { get; set; }
    }
}