using FileDB.Net;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

//Table<D> table = Table<D>.Create
//(
//    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test",
//    "ID"
////IPEndPoint.Parse("127.0.0.1:3000")
//);

//for (int i = 0; i < 100000; i++)
//{
//    table.Insert(new D()
//    {
//        ID = i,
//        Name = "Lee-" + i,
//        PhoneNumber = "12345-" + i,
//        Data = new List<object> { "Hello", "world", 0, i }
//    });
//}

//table.SaveChanges();

Table<D> table2 = Table<D>.Load
(
    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\test"
);

table2.Insert(new D()
{
    ID = 999999999,
    Data = [],
    Name = "0",
    PhoneNumber = "test",
});

table2.SaveChanges();

table2.Insert(new D()
{
    ID = 0,
    Data = [],
    Name = "1",
    PhoneNumber = "tests",
});

var list = table2.FindAll(x => x.ID == 0);
Console.WriteLine(JsonSerializer.Serialize(list));

table2.SaveChanges();

class D
{
    public required int ID { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Name { get; set; }
    public required List<object> Data { get; set; }
}