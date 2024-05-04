// See https://aka.ms/new-console-template for more information

using DbCopy;
using Domain;
using Microsoft.EntityFrameworkCore;

//Console.WriteLine("Hello, World!");


var db = new GwtimeTest2Context();
var evt = await db.Events
    .AsNoTracking()
    .Include(e=>e.User)
    .ThenInclude(e=>e.UserGroup)
    .Include(e=>e.Reader)
    .ToListAsync(CancellationToken.None);
foreach (var e in evt)
{
    new Domain.Event()
    {
        
    };
    Console.WriteLine(e.DateTime);
}