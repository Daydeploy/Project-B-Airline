// namespace Testing
// {
//     [TestClass]
//     public class TestAirportLogic
//     {
//         // Test that GetAvailableAirports() only returns airports with a country code of NL.
//         [TestMethod]
//         public void GetAvailableAirports_OnlyReturnsAirportsFromNL()
//         {
//             // Arrange: Create a list of airports with the required constructor parameters.
//             List<AirportModel> airports = new List<AirportModel>
//             {
//                 new AirportModel(1, "Netherlands", "RTM", "Rotterdam The Hague Airport", "NL", "010-4463444", "Rotterdam Airportplein 60, 3045 AP Rotterdam"),
//                 new AirportModel(1, "Netherlands", "AMS", "Amsterdam Airport Schiphol", "NL", "020-7940800", "Evert van de Beekstraat 202, 1118 CP Schiphol"),
//                 new AirportModel(2, "United Kingdom", "LHR", "London Heathrow Airport", "UK", "0844 335 1801", "Longford, Hounslow TW6")
//             };
//
//             // Act: Filter airports with country code NL.
//             var availableAirports = airports.Where(a => a.AirportID == 1).ToList();
//
//             // Assert: Check if only airports with NL as country code are returned.
//             Assert.AreEqual(2, availableAirports.Count);
//             Assert.IsTrue(availableAirports.All(a => a.Country == "Netherlands"));
//         }
//
//         // Test that FilterAirportsByName() returns airports with the specified name.
//         [TestMethod]
//         [DataRow("Rotterdam The Hague Airport")]
//         [DataRow("Amsterdam Airport Schiphol")]
//         public void FilterAirportsByName_ReturnsAirportsWithName(string airportName)
//         {
//             // Arrange: Create a list of airports with the required constructor parameters.
//             List<AirportModel> airports = new List<AirportModel>
//             {
//                 new AirportModel(1, "Netherlands", "RTM", "Rotterdam The Hague Airport", "NL", "010-4463444", "Rotterdam Airportplein 60, 3045 AP Rotterdam"),
//                 new AirportModel(2, "Netherlands", "AMS", "Amsterdam Airport Schiphol", "NL", "020-7940800", "Evert van de Beekstraat 202, 1118 CP Schiphol"),
//                 new AirportModel(3, "United Kingdom", "LHR", "London Heathrow Airport", "UK", "0844 335 1801", "Longford, Hounslow TW6")
//             };
//
//             // Act: Filter airports by name.
//             var filteredAirports = airports.Where(a => a.Name == airportName).ToList();
//
//             // Assert: Check if the airports have the specified name.
//             Assert.AreEqual(1, filteredAirports.Count);
//             Assert.IsTrue(filteredAirports.All(a => a.Name == airportName));
//         }
//     }
// }