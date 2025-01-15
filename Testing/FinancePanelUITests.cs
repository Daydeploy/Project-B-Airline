using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FinancePanelUITests
{
    [TestClass]
    public class FinancePanelUITests
    {
        [TestMethod]
        public void ShowYearlyDataUI_ValidYear_ReturnsMetrics()
        {
            // Arrange
            int validYear = 2022;
            var expectedMetrics = new FinancePanelLogic.FinancialMetrics
            {
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2022, 12, 31),
                TotalRevenue = 100000,
                BookingCount = 50,
                AverageBookingValue = 2000
            };

            var metrics = FinancePanelLogic.ShowYearlyData(validYear); // Call static method directly

            // Act
            Assert.IsNotNull(metrics);
            Assert.AreEqual(expectedMetrics.TotalRevenue, metrics.TotalRevenue);
            Assert.AreEqual(expectedMetrics.BookingCount, metrics.BookingCount);
        }

        [TestMethod]
        public void ShowYearlyDataUI_InvalidYear_ShowsErrorMessage()
        {
            // Arrange
            int invalidYear = -2022;

            // Act
            var result = FinancePanelLogic.ShowYearlyData(invalidYear); // Call static method directly

            // Assert
            Assert.IsNull(result); // Assuming the logic returns null for invalid input
        }

        [TestMethod]
        public void ShowMonthlyDataUI_ValidMonth_ReturnsMetrics()
        {
            // Arrange
            int validYear = 2022;
            int validMonth = 3; // March
            var expectedMetrics = new FinancePanelLogic.FinancialMetrics
            {
                StartDate = new DateTime(2022, 3, 1),
                EndDate = new DateTime(2022, 3, 31),
                TotalRevenue = 50000,
                BookingCount = 20,
                AverageBookingValue = 2500
            };

            var metrics = FinancePanelLogic.ShowMonthlyData(validYear, validMonth); // Call static method directly

            // Act
            Assert.IsNotNull(metrics);
            Assert.AreEqual(expectedMetrics.TotalRevenue, metrics.TotalRevenue);
            Assert.AreEqual(expectedMetrics.BookingCount, metrics.BookingCount);
        }

        [TestMethod]
        public void ShowMonthlyDataUI_InvalidMonth_ShowsErrorMessage()
        {
            // Arrange
            int validYear = 2022;
            int invalidMonth = 13; // Invalid month

            // Act
            var result = FinancePanelLogic.ShowMonthlyData(validYear, invalidMonth); // Call static method directly

            // Assert
            Assert.IsNull(result); // Assuming the logic returns null for invalid input
        }

        [TestMethod]
        public void ShowDailyDataUI_ValidDate_ReturnsMetrics()
        {
            // Arrange
            DateTime validDate = new DateTime(2023, 1, 15);
            var expectedMetrics = new FinancePanelLogic.FinancialMetrics
            {
                StartDate = validDate,
                EndDate = validDate,
                TotalRevenue = 2000,
                BookingCount = 5,
                AverageBookingValue = 400
            };

            var metrics = FinancePanelLogic.ShowDailyData(validDate); // Call static method directly

            // Act
            Assert.IsNotNull(metrics);
            Assert.AreEqual(expectedMetrics.TotalRevenue, metrics.TotalRevenue);
            Assert.AreEqual(expectedMetrics.BookingCount, metrics.BookingCount);
        }

        [TestMethod]
        public void ShowDailyDataUI_InvalidDate_ShowsErrorMessage()
        {
            // Arrange
            DateTime invalidDate = new DateTime(3000, 1, 1); // Future date

            // Act
            var result = FinancePanelLogic.ShowDailyData(invalidDate); // Call static method directly

            // Assert
            Assert.IsNull(result); // Assuming the logic returns null for invalid input
        }
    }
}