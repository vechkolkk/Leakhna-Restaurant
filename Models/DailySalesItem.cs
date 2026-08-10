namespace RestaurantApp.Models;

public class DailySalesItem
{
    public DateTime Date { get; set; }

    public int ReceiptCount { get; set; }

    public decimal Sales { get; set; }

    public decimal Tax { get; set; }
}
