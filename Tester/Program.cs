using FileDB.Net;
using System.Net;
using System.Text.Json;

Manager manager = Manager.Create
(
    "C:\\Users\\LSW\\Desktop\\test",
    Scheme.Create
    (
        ("ID", "ID in DB", SchemeType.CanInt), 
        ("Name", "User name", SchemeType.CanString),
        ("PhoneNumber", "User's phone number", SchemeType.CanInt | SchemeType.CanString)
    ),
    IPEndPoint.Parse("127.0.0.1:3000"),
    "hello"
);

manager.Insert(new
{
    ID = 5,
    Name = "Lee",
    PhoneNumber = "12345",
});
manager.SaveChanges();

manager = Manager.Load
(
    "C:\\Users\\LSW\\Desktop\\test",
    "hello"    
);

Console.WriteLine(JsonSerializer.Serialize(manager));