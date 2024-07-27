using FileDB.Net;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

Manager manager = Manager.Create
(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test",
    Scheme.Create
    (
        ("ID", "ID in DB", SchemeType.CanInt), 
        ("Name", "User name", SchemeType.CanString),
        ("PhoneNumber", "User's phone number", SchemeType.CanInt | SchemeType.CanString),
        ("Data", "List data", SchemeType.CanList | SchemeType.CanString | SchemeType.CanInt)
    )
    //IPEndPoint.Parse("127.0.0.1:3000")
);

manager.Insert(new
{
    ID = 5,
    Name = "Lee",
    PhoneNumber = "12345",
    Data = new object[]{ "Hello", "world", 0 }
});
manager.SaveChanges();

manager = Manager.Load
(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test"
);

Console.WriteLine(JsonSerializer.Serialize(manager));