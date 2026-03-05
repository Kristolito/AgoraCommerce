using AgoraCommerce.Domain.Enums;

namespace AgoraCommerce.Domain.Entities;

public class CheckoutRequest
{
    private CheckoutRequest()
    {
    }

    public Guid Id { get; private set; }

    public CheckoutOwnerType OwnerType { get; private set; }

    public Guid? AnonymousId { get; private set; }

    public Guid? UserId { get; private set; }

    public string IdempotencyKey { get; private set; } = string.Empty;

    public CheckoutRequestStatus Status { get; private set; }

    public Guid? OrderId { get; private set; }

    public DateTimeOffset ReceivedAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public string? Error { get; private set; }

    public static CheckoutRequest Create(CheckoutOwnerType ownerType, Guid? anonymousId, Guid? userId, string idempotencyKey)
    {
        return new CheckoutRequest
        {
            Id = Guid.NewGuid(),
            OwnerType = ownerType,
            AnonymousId = anonymousId,
            UserId = userId,
            IdempotencyKey = idempotencyKey.Trim(),
            Status = CheckoutRequestStatus.Received,
            ReceivedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkProcessed(Guid orderId)
    {
        Status = CheckoutRequestStatus.Processed;
        OrderId = orderId;
        ProcessedAt = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void MarkFailed(string? error)
    {
        Status = CheckoutRequestStatus.Failed;
        Error = error;
    }
}
