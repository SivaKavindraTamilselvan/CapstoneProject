namespace Ecommerce.DTOs;

public class ResponsePayment
{
    public int PaymentId {get;set;}
    public int OrderId {get;set;}
    public int ModeOfPaymentId {get;set;}
    public string PaymentGatewayOrderId {get;set;} = string.Empty;
    public string PaymentGatewayTransactionId {get;set;} = string.Empty;
    public int PaymentStatusId {get;set;}
    public DateTime? PaymentDate {get;set;}
    public string? FailureReason {get;set;}
    public decimal Amount {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.Now;
}


public class RequestCreatePaymentDTO
{
    public int OrderId { get; set; }
    public int ModeOfPaymentId { get; set; }
}


public class ResponseCreatePaymentDTO
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public bool RequiresOnlinePayment { get; set; }

    public string RazorpayOrderId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int Amount { get; set; } // paise
    public string Currency { get; set; } = "INR";

    public string Message { get; set; } = string.Empty;
}


public class RequestPaymentFailedDTO
{
    public int OrderId { get; set; }
    public string RazorpayOrderId { get; set; } = string.Empty;

    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorDescription { get; set; } = string.Empty;
    public string ErrorSource { get; set; } = string.Empty;
    public string ErrorStep { get; set; } = string.Empty;
    public string ErrorReason { get; set; } = string.Empty;
    public string ErrorField { get; set; } = string.Empty;
}