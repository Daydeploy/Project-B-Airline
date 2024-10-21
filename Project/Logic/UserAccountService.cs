using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Logic
{
    public class UserAccountService
    {
        private AccountsLogic _accountsLogic;
        private List<BookingModel> _bookings;

        public bool IsLoggedIn { get; set; }
        public int CurrentUserId { get; set; }

        public UserAccountService()
        {
            _accountsLogic = new AccountsLogic();
            _bookings = BookingAccess.LoadAll();
        }

        public bool CreateAccount(string email, string password, string fullName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName))
            {
                return false;
            }

            var existingAccount =
                _accountsLogic._accounts.FirstOrDefault(a =>
                    a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (existingAccount != null)
            {
                return false; // Account with this email already exists
            }

            int newId = _accountsLogic._accounts.Max(a => a.Id) + 1;
            CurrentUserId = newId;
            var newAccount = new AccountModel(newId, email, password, fullName);
            _accountsLogic.UpdateList(newAccount);
            return true;
        }

        public AccountModel Login(string email, string password)
        {
            // Set current user id
            CurrentUserId =
                _accountsLogic._accounts.FirstOrDefault(a => a.EmailAddress == email)?.Id ?? -1; // Set to -1 if not found
            var account = _accountsLogic.CheckLogin(email, password);
            if (account != null)
            {
                IsLoggedIn = true;
            }
            else
            {
                IsLoggedIn = false;
            }

            return account;
        }

        public bool ManageAccount(int userId, string? newEmail = null, string? newPassword = null, string? newFullName = null)
        {
            var account = _accountsLogic.GetById(userId);
            if (account == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                account.EmailAddress = newEmail;
            }

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                account.Password = newPassword;
            }

            if (!string.IsNullOrWhiteSpace(newFullName))
            {
                account.FullName = newFullName;
            }

            _accountsLogic.UpdateList(account);
            return true;
        }

        // Note: The following methods are placeholders and would need to be implemented
        // with actual flight booking data and logic.

        public bool BookFlight(int userId, int flightId, int totalPrice, string seatNumber, bool hasCheckedBaggage)
        {
            // Generate a new booking ID (this should be handled by the database in a real application)
            int bookingId = _bookings.Count > 0 ? _bookings.Max(b => b.BookingId) + 1 : 1;
            // For now, we'll just add a new booking to the list
            _bookings.Add(new BookingModel(bookingId, userId, flightId, totalPrice, seatNumber, hasCheckedBaggage));

            // Save the updated bookings
            BookingAccess.WriteAll(_bookings);

            return true;
        }

        public List<FlightBooking> GetBookedFlights(int userId)
        {
            var userBookings = _bookings.Where(b => b.UserId == userId).ToList();
            var flightsLogic = new FlightsLogic();
            var flights = flightsLogic.GetAllFlights();

            return userBookings.Select(booking =>
            {
                var flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
                return new FlightBooking
                {
                    FlightId = booking.FlightId,
                    FlightNumber = flight?.FlightNumber ?? "Unknown",
                    DepartureTime = flight?.DepartureTime ?? DateTime.MinValue.ToString(),
                    ArrivalTime = flight?.ArrivalTime ?? DateTime.MinValue.ToString()
                };
            }).ToList();
        }

        public bool CheckIn(int flightId)
        {
            // For now, we'll just return true to simulate a successful check-in
            // In a real application, you'd want to update the booking status
            return true;
        }

        public bool ModifyBooking(int flightId, BookingDetails newDetails)
        {
            var booking = _bookings.FirstOrDefault(b => b.FlightId == flightId);
            if (booking == null)
            {
                return false;
            }

            // Update the booking details
            booking.SeatNumber = newDetails.SeatNumber;
            booking.HasCheckedBaggage = newDetails.HasCheckedBaggage;

            // Save the updated bookings
            BookingAccess.WriteAll(_bookings);

            return true;
        }
    }

    // Placeholder classes for flight-related functionality
    public class FlightBooking
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
    }

    public class BookingDetails
    {
        public string SeatNumber { get; set; }
        public bool HasCheckedBaggage { get; set; }
    }
}
