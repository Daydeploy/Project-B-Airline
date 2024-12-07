using System.Collections.Generic;
// ik moet hier  ff typen anders kan ik deze file niet pushen



public class SmallItemsService
{
    public List<SmallItem> FetchItemDetails()
    {
        return SmallItemsDataAccess.GetAvailableItems();
    }

    public void AddToPurchase(int itemId, int bookingId)
    {
        if (SmallItemsDataAccess.CheckItemAvailability(itemId))
        {
            SmallItemsDataAccess.AddItemToBooking(itemId, bookingId);
        }
        else
        {
        }
    }

    public decimal CalculateTotalCost(List<int> selectedItemIds)
    {
        decimal totalCost = 0;
        foreach (var id in selectedItemIds)
        {
            var item = SmallItemsDataAccess.GetAvailableItems().Find(i => i.Id == id);
            if (item != null)
            {
                totalCost += item.Price;
            }
        }
        return totalCost;
    }
} 