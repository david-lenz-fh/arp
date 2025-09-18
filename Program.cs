using System.Net;
using System.Text;
using mrp;
using MRP.Data;

var db=PostgresDB.Initialize();
Console.WriteLine(await db.ChangeData("INSERT INTO mrp_user (username, password) VALUES (@name, @password)", new Dictionary<string, object> { ["@name"] = "OB",["@password"]="12"}));
Console.WriteLine(await db.ChangeData("DELETE FROM mrp_user WHERE username=@name", new Dictionary<string, object> { ["@name"] = "OB" }));
db.ReadData("",null).
var mrp_server = new MRPServer();
mrp_server.Listen();
