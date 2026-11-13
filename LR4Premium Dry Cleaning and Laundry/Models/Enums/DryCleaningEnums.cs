namespace DryCleaningApi.Models.Enums;

public enum GarmentCategory
{
    Suit,       
    DownJacket, 
    EveningDress, 
    Shoes,        
    Textile         
}

public enum CleaningType
{
    DryCleaning,  
    AquaCleaning   
}

public enum ReceiptStatus
{
    Accepted,       
    Processing,     
    ReadyForPickup, 
    Closed         
}

public enum QueuePriority
{
    Standard,
    HighExpress
}