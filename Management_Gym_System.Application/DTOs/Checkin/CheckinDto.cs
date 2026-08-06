public class CheckinDto
{
    public long? checkinId { get; set; }
    public DateTime? checkinTime { get; set; }
    public long CardID { get; set; }
    public string? fullName { get; set; }
    public string? avatar { get; set; }
    public DateTime? startDate { get; set; }
    public DateTime? endDate { get; set; }

    public string? rfidUid { get; set; }
    public string? cardStatus { get; set; }
}

public class CardInfo
{
    public long? ID { get; set; }
    public string? RfidUid { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? CardStatus { get; set; }
}