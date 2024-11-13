using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class AirportModel
{
    public int AirportID { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Name { get; set; }
    public string Code { get; set; } 
    public string Type { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }

    public AirportModel(int airportID, string country, string city, string name, string code, string type,
        string phoneNumber, string address)
    {
        AirportID = airportID;
        Country = country;
        City = city;
        Name = name;
        Code = code; 
        Type = type;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}