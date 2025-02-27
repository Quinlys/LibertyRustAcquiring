namespace LibertyRustAcquiring.Order.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<UpdateOrderStatusResult>
    {
        public string OrderId { get; }
        public string Status { get; }
        public UpdateOrderStatusCommand(string orderId, string status)
        {
            OrderId = orderId;
            Status = status;
        }
    }
}
