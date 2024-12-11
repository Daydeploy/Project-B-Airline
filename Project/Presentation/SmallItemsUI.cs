// using System;
// using System.Collections.Generic;
// // ik moet hier  ff typen anders kan ik deze file niet pushen


// public class SmallItemsUI
// {
//     private SmallItemsServiceLogic _smallItemsServiceLogic = new SmallItemsServiceLogic();

//     public void RenderItemCatalog()
//     {
//         List<SmallItem> items = _smallItemsServiceLogic.FetchItemDetails();
//         Console.WriteLine("Available Small Items:");
//         foreach (var item in items)
//         {
//             Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Price: {item.Price:C}, Description: {item.Description}, Availability: {item.Availability}");
//         }
//     }

//     public void AddItemToBooking(int itemId, int bookingId)
//     {
//         _smallItemsServiceLogic.AddToPurchase(itemId, bookingId);
//         Console.WriteLine("Item added to your booking.");
//     }
// } 