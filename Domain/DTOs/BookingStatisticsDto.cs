using System;

namespace Domain.DTOs;

public class BookingStatisticsDto
{
    public int BookingCount { get; set; }
    public decimal TotalPrice { get; set; }
    public int CompanyCount { get; set; }
}
