// using Microsoft.VisualStudio.TestTools.UnitTesting;
//
// [TestClass]
// public class TestPetService
// {
//     private PetServiceLogic _petService;
//
//     [TestInitialize]
//     public void Setup()
//     {
//         _petService = new PetServiceLogic();
//     }
//
//     [TestMethod]
//     public void TestAddPetToBooking()
//     {
//         var pet = new PetModel { Type = "Dog", Size = "Medium", SeatingLocation = "Seat" };
//         int bookingId = 1;
//
//         _petService.AddPetToBooking(bookingId, pet);
//
//         var pets = PetDataAccess.LoadPetReservations(bookingId);
//         Assert.IsTrue(pets.Contains(pet));
//     }
//
//     [TestMethod]
//     public void TestCalculatePetFees()
//     {
//         var pet = new PetModel { Type = "Dog", SeatingLocation = "Seat" };
//
//         decimal fee = _petService.CalculatePetFees(pet);
//
//         Assert.AreEqual(50, fee); // heb fee ff op 50 gezet
//     }
//
//     [TestMethod]
//     public void TestValidatePetBooking()
//     {
//         var pet = new PetModel { Type = "Cat", SeatingLocation = "Seat" };
//
//         Assert.ThrowsException<InvalidOperationException>(() => _petService.ValidatePetBooking(pet));
//     }
// }