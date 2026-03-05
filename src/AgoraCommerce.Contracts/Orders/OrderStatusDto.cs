namespace AgoraCommerce.Contracts.Orders;

public enum OrderStatusDto
{
    PendingPayment = 0,
    Paid = 1,
    Shipped = 2,
    Completed = 3,
    Canceled = 4
}
