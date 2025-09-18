using System.Net;
using System.Text;
using mrp;
using MRP.Data;
using MRP.model;

var db=PostgresDB.Initialize();
UserRepository userRepo = new UserRepository(db);
var foundUsr=await userRepo.FindById(1);
var addedUsr = await userRepo.AddUser(new User(0, "OBDave", "12345"));
if(addedUsr != null)
{
    Console.WriteLine("User hinzugefügt: {0}", addedUsr.ToString()); 
    if (await userRepo.Delete(addedUsr.Id))
    {
        Console.WriteLine("User gelöscht: {0}", addedUsr.ToString());
    }
}
else
{
    Console.WriteLine("User wurde nicht hinzugefügt");
}
if (foundUsr != null)
{
    Console.WriteLine("User gefunden: {0}",foundUsr.ToString());
}
else
{
    Console.WriteLine("Kein User gefunden");
}

var mrp_server = new MRPServer();
mrp_server.Listen();
