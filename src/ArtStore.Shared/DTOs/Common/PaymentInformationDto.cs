namespace ArtStore.Shared.DTOs.Common;

public class PaymentInformationDto
{
    public string CardholderName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public string ExpiryDate { get; set; } = "";
    public string Cvv { get; set; } = "";
}