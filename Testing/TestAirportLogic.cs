using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Testing
{
    [TestClass]
    public class TestAirportLogic
    {
        private AirportLogic _airportLogic;
        private List<AirportModel> _testAirports;

        [TestInitialize]
        public void Setup()
        {
            _testAirports = new List<AirportModel>
            {
                new AirportModel(1, "Netherlands", "Rotterdam", "Rotterdam Airport", "RTM", "Public", "+31 10 123 4567", "Test Address 1"),
                new AirportModel(2, "Ireland", "Dublin", "Dublin Airport", "DUB", "Public", "+353 1 814 1111", "Test Address 2"),
                new AirportModel(3, "United Kingdom", "London", "London City Airport", "LCY", "Public", "+44 20 7646 0088", "Test Address 3")
            };

            _airportLogic = new AirportLogic();
        }

        [TestMethod]
        public void GetAllAirports_ReturnsAllAirports()
        {

            var result = _airportLogic.GetAllAirports();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public void GetAirportById_ExistingId_ReturnsCorrectAirport()
        {
            int testId = 1;

            var result = _airportLogic.GetAirportById(testId);

            Assert.IsNotNull(result);
            Assert.AreEqual(testId, result.AirportID);
        }

        [TestMethod]
        public void GetAirportById_NonExistingId_ReturnsNull()
        {
            int nonExistingId = 999;

            var result = _airportLogic.GetAirportById(nonExistingId);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void UpdateAirport_ExistingAirport_ReturnsTrue()
        {
            var existingAirport = _airportLogic.GetAirportById(1);
            existingAirport.Name = "Updated Airport Name";

            bool result = _airportLogic.UpdateAirport(existingAirport);

            Assert.IsTrue(result);
            var updatedAirport = _airportLogic.GetAirportById(1);
            Assert.AreEqual("Updated Airport Name", updatedAirport.Name);
        }

        [TestMethod]
        public void UpdateAirport_NonExistingAirport_ReturnsFalse()
        {
            var nonExistingAirport = new AirportModel(
                999, 
                "Test Country", 
                "Test City", 
                "Test Airport", 
                "TST", 
                "Public",
                "+1 234 567 8900",
                "Test Address"
            );

            bool result = _airportLogic.UpdateAirport(nonExistingAirport);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddAirport_NewAirport_AddsSuccessfully()
        {
            var newAirport = new AirportModel(
                8, 
                "Germany", 
                "Berlin", 
                "Berlin Airport", 
                "BER", 
                "Public",
                "+49 30 609 11150",
                "Test Address"
            );

            _airportLogic.AddAirport(newAirport);
            var result = _airportLogic.GetAirportById(8);

            Assert.IsNotNull(result);
            Assert.AreEqual(newAirport.Name, result.Name);
            Assert.AreEqual(newAirport.Code, result.Code);
        }
    }
}