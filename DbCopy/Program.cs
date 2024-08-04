using System;

namespace dbCopy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            using (var db = new SQLiteContext())
            {
                var events = db.Events.Where(e=>e.UserPtr != "0").ToList();

                foreach (var e in events)
                {
                    var appEvt = new Event();
                    appEvt.UserId = int.Parse(e.UserPtr);
                    appEvt.DateTime = (DateTime)e.DateTime;
                    appEvt.Code = short.Parse(e.EventCode);
                    appEvt.ReaderId = short.Parse(e.RdrPtr);


                    using (var ms = new Gwt0905Context())
                    {
                        try
                        {
                            ms.Events.Add(appEvt);
                            ms.SaveChanges();
                            Console.WriteLine($"Добавлено!");
                        }
                        catch (Exception exception)
                        {
                            // Console.WriteLine(exception.InnerException.Message);
                            Console.WriteLine($"Дубикат!");
                        }
                        
                    }
                }

                
            }

            
        }
    }
}
