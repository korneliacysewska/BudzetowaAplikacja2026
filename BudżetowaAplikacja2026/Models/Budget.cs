public class Budget
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public decimal TotalAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - SpentAmount;
    public DateTime CreatedAt { get; set; }

    
    public User? User { get; set; }
}
