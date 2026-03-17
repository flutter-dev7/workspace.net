using System;

namespace Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int WorkSpaceId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}