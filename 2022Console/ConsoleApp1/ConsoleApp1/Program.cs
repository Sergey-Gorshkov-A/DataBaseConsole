using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Xml;
using ConsoleApp1;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Any;
using NPOI.SS.Formula.Functions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1
{
    class Program
    {
        private const int Port = 5000;
        static void Main(string[] args)
        {
            Console.WriteLine("Запуск сервера...");

            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Console.WriteLine($"Сервер запущен и ожидает подключения на порту {Port}...");

            while (true)
            {
                try
                {
                    using TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("Клиент подключен.");

                    using NetworkStream stream = client.GetStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string command = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"Получена команда: {command}");

                    string response = ExecuteDatabaseQuery(command);
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                    Console.WriteLine("Ответ отправлен клиенту.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
        }

        //public static IQueryable<object> GetTable(BloggingContext context, string table)
        //{
            //var t = context.GetType().GetFields().Select(field => field.Name)
                            //.ToList();
            //return t;
        //}

        public static DbSet<Area> GetAreas(ZooContext context)
        {
            return context.Areas;
        }

        public static DbSet<AdultAnimal> GetAdultAnimals(ZooContext context)
        {
            return context.AdultAnimals;
        }

        public static DbSet<Animal> GetAnimals(ZooContext context)
        {
            return context.Animals;
        }

        public static DbSet<Aviary> GetAviaries(ZooContext context)
        {
            return context.Aviaries;
        }

        public static DbSet<BabyAnimal> GetBabyAnimals(ZooContext context)
        {
            return context.BabyAnimals;
        }

        public static DbSet<Day> GetDays(ZooContext context)
        {
            return context.Days;
        }

        public static DbSet<Employer> GetEmployers(ZooContext context)
        {
            return context.Employers;
        }

        public static DbSet<Event> GetEvents(ZooContext context)
        {
            return context.Events;
        }

        public static DbSet<WorkingShift> GetWorkingShifts(ZooContext context)
        {
            return context.WorkingShifts;
        }

        public static string ExecuteDatabaseQuery(string query)
            {
            try
            {
                var builder = new HostApplicationBuilder();

                builder.Services.AddDbContext<ZooContext>(options =>
                {
                    options.UseSqlite($"Data Source={Path.GetFullPath(@"..\..\..\zoo.db")}");
                });

                var app = builder.Build();
                var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                using var context = services.GetRequiredService<ZooContext>();

                // регистрация пользователя
                if (query.StartsWith("REGISTRATION:")) 
                {
                    string formated_query = query.Substring(13);
                    string[] query_words = formated_query.Split(new char[] { ';' });

                    byte[] password = Encoding.UTF8.GetBytes(query_words[1]);
                    byte[] hashValue = SHA256.HashData(password);
                    string hashPassword = Convert.ToHexString(hashValue);
                    var user = new User { Login = query_words[0], Password = hashPassword };
                    context.Users.Add(user);
                    context.SaveChanges();
                    return "Регистрация прошла успешно";
                }

                // авторизация пользователя
                if (query.StartsWith("AUTH:"))
                {
                    string formated_query = query.Substring(5);
                    string[] query_words = formated_query.Split(new char[] { ';' });
                    List<User> users = context.Users.ToList();
                    foreach (User user in users)
                    {
                        if (user.Login == query_words[0])
                        {
                            byte[] sentHashValue = Convert.FromHexString(user.Password);
                            byte[] getPassword = Encoding.UTF8.GetBytes(query_words[1]);
                            byte[] compareHashValue = SHA256.HashData(getPassword);
                            bool same = sentHashValue.SequenceEqual(compareHashValue);
                            if (same)
                            {
                                return "Вход успешен";
                            }
                            else
                            {
                                return "Неправильный пароль";
                            }
                        }
                    }
                    return "Такого пользователя нет";
                }

                // обработка команды: "GET:<Table>;<Filter>"
                if (query.StartsWith("GET:"))
                {
                    string formated_query = query.Substring(4);
                    string[] query_words = formated_query.Split(new char[] { ';' });
                    //var table1 = GetTable(context, query_words[0]);

                    InvokerORM invoker = new InvokerORM();
                    ReceiverORM receiver = new ReceiverORM();
                    Get get = new Get(receiver, context, query_words[0], query_words[1]);
                    invoker.SetCommand(get);
                    return invoker.Run();
                }

                // обработка команды: "ADD:<Table>;<Values>"
                if (query.StartsWith("ADD:"))
                {
                    string formated_query = query.Substring(4);
                    string[] query_words = formated_query.Split(new char[] { ';' });

                    InvokerORM invoker = new InvokerORM();
                    ReceiverORM receiver = new ReceiverORM();
                    Add add = new Add(receiver, context, query_words[0], query_words);
                    invoker.SetCommand(add);
                    return invoker.Run();
                }

                // обработка команды: "DELETE:<Table>;<Field>=<Value>
                if (query.StartsWith("DELETE:"))
                {
                    string formated_query = query.Substring(7);
                    string[] query_words = formated_query.Split(new char[] { ';' });
                    string[] delete_words = query_words[1].Split(new char[] { '=' });

                    InvokerORM invoker = new InvokerORM();
                    ReceiverORM receiver = new ReceiverORM();
                    Delete delete = new Delete(receiver, context, query_words[0], delete_words);
                    invoker.SetCommand(delete);
                    return invoker.Run();
                }

                // обработка команды: "UPDATE:<Table>;<Field>=<Value>;<Field>=<newValue>
                if (query.StartsWith("UPDATE:"))
                {
                    string formated_query = query.Substring(7);
                    string[] query_words = formated_query.Split(new char[] { ';' });
                    string[] filter_words = query_words[1].Split(new char[] { '=' });
                    string[] update_words = query_words[2].Split(new char[] { '=' });

                    InvokerORM invoker = new InvokerORM();
                    ReceiverORM receiver = new ReceiverORM();
                    Update delete = new Update(receiver, context, query_words[0], filter_words, update_words);
                    invoker.SetCommand(delete);
                    return invoker.Run();
                }

                // обработка sql-команды: "SQL:<command>
                if (query.StartsWith("SQL:"))
                {
                    string formated_query = query.Substring(4);
                    string table = "";
                    if (formated_query.StartsWith("SELECT"))
                    {
                        string[] query_words = formated_query.Split(new char[] { ' ' });
                        for (int i = 0; i < query_words.Length; i++)
                        {
                            if (query_words[i] == "FROM")
                            {
                                table = query_words[i + 1];
                                break;
                            }
                        }
                        switch (table)
                        {
                            case "Areas":
                                return JsonSerializer.Serialize(context.Areas.FromSqlRaw(formated_query).ToList());

                            case "AdultAnimals":
                                return JsonSerializer.Serialize(context.AdultAnimals.FromSqlRaw(formated_query).ToList());

                            case "Animals":
                                return JsonSerializer.Serialize(context.Animals.FromSqlRaw(formated_query).ToList());

                            case "Aviaries":
                                return JsonSerializer.Serialize(context.Aviaries.FromSqlRaw(formated_query).ToList());

                            case "BabyAnimals":
                                return JsonSerializer.Serialize(context.BabyAnimals.FromSqlRaw(formated_query).ToList());

                            case "Days":
                                return JsonSerializer.Serialize(context.Days.FromSqlRaw(formated_query).ToList());

                            case "Employers":
                                return JsonSerializer.Serialize(context.Employers.FromSqlRaw(formated_query).ToList());

                            case "Events":
                                return JsonSerializer.Serialize(context.Events.FromSqlRaw(formated_query).ToList());

                            case "WorkingShifts":
                                return JsonSerializer.Serialize(context.WorkingShifts.FromSqlRaw(formated_query).ToList());
                            
                            default:
                                return "Такой таблицы нет";
                        }
                    }
                    else 
                    {
                        context.Database.ExecuteSqlRaw(formated_query);
                        return "Запрос выполнен";
                    }
                }
                
                return "Неизвестная команда.";
            }
            catch (Exception ex)
            {
                return $"Ошибка выполнения запроса: {ex.Message}";
            }
        }
    }

    public abstract class CommandORM
    {
        public abstract string Execute();
    }

    public class Add : CommandORM
    {
        ReceiverORM receiverORM;
        ZooContext context1;
        string name;
        string[] addValues;
        public Add(ReceiverORM receiverORM, ZooContext context, string tablename, string[] addValues)
        {
            this.receiverORM = receiverORM;
            this.context1 = context;
            this.name = tablename;
            this.addValues = addValues;
        }

        public override string Execute()
        {
            return receiverORM.CommandAdd(context1, name, addValues);
        }
    }

    class Delete : CommandORM
    {
        ReceiverORM receiverORM;
        ZooContext context1;
        string name;
        string[] deleteValues;
        public Delete(ReceiverORM receiverORM, ZooContext context, string tablename, string[] deleteValues)
        {
            this.receiverORM = receiverORM;
            this.context1 = context;
            this.name = tablename;
            this.deleteValues = deleteValues;
        }

        public override string Execute()
        {
            return receiverORM.CommandDelete(context1, name, deleteValues);
        }
    }

    class Update : CommandORM
    {
        ReceiverORM receiverORM;
        ZooContext context1;
        string name;
        string[] filterValues;
        string[] updateValues;
        public Update(ReceiverORM receiverORM, ZooContext context, string tablename, string[] filterValues, string[] updateValues)
        {
            this.receiverORM = receiverORM;
            this.context1 = context;
            this.name = tablename;
            this.filterValues = filterValues;
            this.updateValues = updateValues;
        }

        public override string Execute()
        {
            return receiverORM.CommandUpdate(context1, name, filterValues, updateValues);
        }
    }

    class Get : CommandORM
    {
        ReceiverORM receiverORM;
        ZooContext context1;
        string name;
        string filter;
        public Get(ReceiverORM receiverORM, ZooContext context, string tablename, string filter)
        {
            this.receiverORM = receiverORM;
            this.context1 = context;
            this.name = tablename;
            this.filter = filter;
        }

        public override string Execute()
        {
            return receiverORM.CommandGet(context1, name, filter);
        }
    }

    public class ReceiverORM
    {
        public string CommandAdd(ZooContext context, string tablename, string[] values)
        {
            switch (tablename)
            {
                case "Areas":
                    var entity1 = new Area { Area_name = values[1], Adress = values[2] };
                    context.Areas.Add(entity1);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "AdultAnimals":
                    var entity2 = new AdultAnimal { AnimalId = int.Parse(values[1]), Name = values[2] };
                    context.AdultAnimals.Add(entity2);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "Animals":
                    var entity3 = new Animal { AviaryId = int.Parse(values[1]), TypeOfAnimal = values[2], Status = values[3] };
                    context.Animals.Add(entity3);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "Aviaries":
                    var entity4 = new Aviary { AviaryName = values[1], AreaId = int.Parse(values[2]), };
                    context.Aviaries.Add(entity4);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "BabyAnimals":
                    var entity5 = new BabyAnimal { AnimalId = int.Parse(values[1]), Name = values[2], AdultAnimalId = int.Parse(values[3]) };
                    context.BabyAnimals.Add(entity5);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "Days":
                    var entity6 = new Day { Date = DateTime.ParseExact(values[1], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) };
                    context.Days.Add(entity6);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "Employers":
                    var entity7 = new Employer { Fio = values[1], AreaId = int.Parse(values[2]) };
                    context.Employers.Add(entity7);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "Events":
                    var entity8 = new Event
                    {
                        DayId = int.Parse(values[1]),
                        AnimalId = int.Parse(values[2]),
                        WorkingShiftId = int.Parse(values[3]),
                        IsBorn = Convert.ToBoolean(values[4]),
                        IsDie = Convert.ToBoolean(values[5]),
                        EventTime = DateTime.ParseExact(values[6], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                        EventText = values[7]
                    };
                    context.Events.Add(entity8);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                case "WorkingShifts":
                    var entity9 = new WorkingShift
                    {
                        TimeBegin = DateTime.ParseExact(values[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                        TimeEnd = DateTime.ParseExact(values[2], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture),
                        EmployerId = int.Parse(values[3])
                    };
                    context.WorkingShifts.Add(entity9);
                    context.SaveChanges();
                    return "Запись добавлена.";
                
                default:
                    return "Ничего не добавлено";
            }
        }

        public string CommandUpdate(ZooContext context, string tablename, string[] values, string[] updateValues)
        {
            switch (tablename)
            {
                case "Areas":
                    string fieldValue = "";
                    List<Area> areaList = context.Areas.ToList();
                    foreach (Area area in areaList)
                    {
                        foreach (PropertyInfo field in area.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue = area.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(area).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in area.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = area.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(area, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(area, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(area, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Areas.Update(area);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "AdultAnimals":
                    string fieldValue2 = "";
                    List<AdultAnimal> adultAnimalList = context.AdultAnimals.ToList();
                    foreach (AdultAnimal adultAnimal in adultAnimalList)
                    {
                        foreach (PropertyInfo field in adultAnimal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue2 = adultAnimal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(adultAnimal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue2, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in adultAnimal.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = adultAnimal.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(adultAnimal, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(adultAnimal, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(adultAnimal, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.AdultAnimals.Update(adultAnimal);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Animals":
                    string fieldValue3 = "";
                    List<Animal> animalList = context.Animals.ToList();
                    foreach (Animal animal in animalList)
                    {
                        foreach (PropertyInfo field in animal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue3 = animal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(animal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue3, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in animal.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = animal.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(animal, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(animal, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(animal, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Animals.Update(animal);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Aviaries":
                    string fieldValue4 = "";
                    List<Aviary> aviaryList = context.Aviaries.ToList();
                    foreach (Aviary aviary in aviaryList)
                    {
                        foreach (PropertyInfo field in aviary.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue4 = aviary.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(aviary).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue4, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in aviary.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = aviary.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(aviary, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(aviary, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(aviary, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Aviaries.Update(aviary);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "BabyAnimals":
                    string fieldValue5 = "";
                    List<BabyAnimal> babyAnimalList = context.BabyAnimals.ToList();
                    foreach (BabyAnimal babyAnimal in babyAnimalList)
                    {
                        foreach (PropertyInfo field in babyAnimal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue5 = babyAnimal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(babyAnimal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue5, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in babyAnimal.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = babyAnimal.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(babyAnimal, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(babyAnimal, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(babyAnimal, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.BabyAnimals.Update(babyAnimal);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Days":
                    string fieldValue6 = "";
                    List<Day> dayList = context.Days.ToList();
                    foreach (Day day in dayList)
                    {
                        foreach (PropertyInfo field in day.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue6 = day.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(day).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue6, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in day.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = day.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(day, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(day, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(day, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Days.Update(day);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Employers":
                    string fieldValue7 = "";
                    List<Employer> employerList = context.Employers.ToList();
                    foreach (Employer employer in employerList)
                    {
                        foreach (PropertyInfo field in employer.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue7 = employer.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(employer).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue7, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in employer.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = employer.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(employer, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(employer, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(employer, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Employers.Update(employer);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Events":
                    string fieldValue8 = "";
                    List<Event> eventList = context.Events.ToList();
                    foreach (Event e in eventList)
                    {
                        foreach (PropertyInfo field in e.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue8 = e.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(e).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue8, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in e.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = e.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(e, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(e, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(e, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.Events.Update(e);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "WorkingShifts":
                    string fieldValue9 = "";
                    List<WorkingShift> workingShiftList = context.WorkingShifts.ToList();
                    foreach (WorkingShift shift in workingShiftList)
                    {
                        foreach (PropertyInfo field in shift.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue = shift.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(shift).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue9, values[1]);
                        if (q)
                        {
                            foreach (PropertyInfo field in shift.GetType().GetProperties())
                            {
                                if (field.Name == updateValues[0])
                                {
                                    var updateField = shift.GetType()
                                                      .GetProperty(field.Name);
                                    if (updateField.PropertyType == typeof(int))
                                    {
                                        updateField.SetValue(shift, int.Parse(updateValues[1]));
                                    }
                                    if (updateField.PropertyType == typeof(string))
                                    {
                                        updateField.SetValue(shift, updateValues[1]);
                                    }
                                    if (updateField.PropertyType == typeof(DateTime))
                                    {
                                        updateField.SetValue(shift, DateTime.ParseExact(updateValues[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                                    }
                                    context.WorkingShifts.Update(shift);
                                    context.SaveChanges();
                                    return "Запись обновлена.";
                                }
                            }
                        }
                    }
                    return "Такое поле отсутстсвует";

                default:
                    return "Ничего не обновлено";
            }
        }

        public string CommandDelete(ZooContext context, string tablename, string[] values)
        {
            switch (tablename)
            {
                case "Areas":
                    string fieldValue = "";
                    List<Area> areaList = context.Areas.ToList();
                    foreach (Area area in areaList)
                    {
                        foreach (PropertyInfo field in area.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue = area.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(area).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue, values[1]);
                        if (q)
                        {
                            context.Areas.Remove(area);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "AdultAnimals":
                    string fieldValue1 = "";
                    List<AdultAnimal> adultAnimalList = context.AdultAnimals.ToList();
                    foreach (AdultAnimal adultAnimal in adultAnimalList)
                    {
                        foreach (PropertyInfo field in adultAnimal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue1 = adultAnimal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(adultAnimal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue1, values[1]);
                        if (q)
                        {
                            context.AdultAnimals.Remove(adultAnimal);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Animals":
                    string fieldValue2 = "";
                    List<Animal> AnimalList = context.Animals.ToList();
                    foreach (Animal animal in AnimalList)
                    {
                        foreach (PropertyInfo field in animal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue2 = animal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(animal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue2, values[1]);
                        if (q)
                        {
                            context.Animals.Remove(animal);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Aviaries":
                    string fieldValue3 = "";
                    List<Aviary> aviaryList = context.Aviaries.ToList();
                    foreach (Aviary aviary in aviaryList)
                    {
                        foreach (PropertyInfo field in aviary.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue3 = aviary.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(aviary).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue3, values[1]);
                        if (q)
                        {
                            context.Aviaries.Remove(aviary);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "BabyAnimals":
                    string fieldValue4 = "";
                    List<BabyAnimal> babyAnimalList = context.BabyAnimals.ToList();
                    foreach (BabyAnimal animal in babyAnimalList)
                    {
                        foreach (PropertyInfo field in animal.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue4 = animal.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(animal).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue4, values[1]);
                        if (q)
                        {
                            context.BabyAnimals.Remove(animal);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Days":
                    string fieldValue5 = "";
                    List<Day> dayList = context.Days.ToList();
                    foreach (Day day in dayList)
                    {
                        foreach (PropertyInfo field in day.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue5 = day.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(day).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue5, values[1]);
                        if (q)
                        {
                            context.Days.Remove(day);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Employers":
                    string fieldValue6 = "";
                    List<Employer> employerList = context.Employers.ToList();
                    foreach (Employer employer in employerList)
                    {
                        foreach (PropertyInfo field in employer.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue6 = employer.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(employer).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue6, values[1]);
                        if (q)
                        {
                            context.Employers.Remove(employer);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "Events":
                    string fieldValue7 = "";
                    List<Event> eventsList = context.Events.ToList();
                    foreach (Event e in eventsList)
                    {
                        foreach (PropertyInfo field in e.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue7 = e.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(e).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue7, values[1]);
                        if (q)
                        {
                            context.Events.Remove(e);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                case "WorkingShifts":
                    string fieldValue8 = "";
                    List<WorkingShift> workingShiftList = context.WorkingShifts.ToList();
                    foreach (WorkingShift shift in workingShiftList)
                    {
                        foreach (PropertyInfo field in shift.GetType().GetProperties())
                        {
                            if (field.Name == values[0])
                            {
                                fieldValue8 = shift.GetType()
                                             .GetProperty(field.Name)
                                             .GetValue(shift).ToString();
                                break;
                            }
                        }
                        var q = String.Equals(fieldValue8, values[1]);
                        if (q)
                        {
                            context.WorkingShifts.Remove(shift);
                            context.SaveChanges();
                            return "Запись удалена.";
                        }
                    }
                    return "Такое поле отсутстсвует";

                default:
                    return "Ничего не удалено";
            }
        }

        public string CommandGet(ZooContext context, string tablename, string filter)
        {
            string[] values_1 = filter.Split("=");
            string[] values_2 = filter.Split(">");
            string[] values_3 = filter.Split("<");
            string fieldValue = "";
            switch (tablename)
            {
                
                case "Areas":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetAreas(context).ToList());
                    }
                    else
                    {
                        List<Area> areaList = context.Areas.ToList();
                        List<Area> areas = new List<Area>();
                        foreach (Area area in areaList)
                        {
                            foreach (PropertyInfo field in area.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = area.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(area).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            areas.Add(area);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = area.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(area) > int.Parse(values_2[1]))
                                            {
                                                areas.Add(area);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(area) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                areas.Add(area);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = area.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(area) < int.Parse(values_3[1]))
                                            {
                                                areas.Add(area);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(area) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                areas.Add(area);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(areas);
                    }
                case "AdultAnimals":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetAdultAnimals(context).ToList());
                    }
                    else
                    {
                        List<AdultAnimal> adultAnimalList = context.AdultAnimals.ToList();
                        List<AdultAnimal> adultAnimals = new List<AdultAnimal>();
                        foreach (AdultAnimal adultAnimal in adultAnimalList)
                        {
                            foreach (PropertyInfo field in adultAnimal.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = adultAnimal.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(adultAnimal).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            adultAnimals.Add(adultAnimal);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = adultAnimal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(adultAnimal) > int.Parse(values_2[1]))
                                            {
                                                adultAnimals.Add(adultAnimal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(adultAnimal) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                adultAnimals.Add(adultAnimal);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = adultAnimal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(adultAnimal) < int.Parse(values_3[1]))
                                            {
                                                adultAnimals.Add(adultAnimal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(adultAnimal) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                adultAnimals.Add(adultAnimal);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(adultAnimals);
                    }
                case "Animals":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetAnimals(context).ToList());
                    }
                    else
                    {
                        List<Animal> animalList = context.Animals.ToList();
                        List<Animal> animals = new List<Animal>();
                        foreach (Animal animal in animalList)
                        {
                            foreach (PropertyInfo field in animal.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = animal.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(animal).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            animals.Add(animal);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = animal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(animal) > int.Parse(values_2[1]))
                                            {
                                                animals.Add(animal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(animal) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                animals.Add(animal);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = animal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(animal) < int.Parse(values_3[1]))
                                            {
                                                animals.Add(animal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(animal) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                animals.Add(animal);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(animals);
                    }
                case "Aviaries":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetAviaries(context).ToList());
                    }
                    else
                    {
                        List<Aviary> aviaryList = context.Aviaries.ToList();
                        List<Aviary> aviaries = new List<Aviary>();
                        foreach (Aviary aviary in aviaryList)
                        {
                            foreach (PropertyInfo field in aviary.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = aviary.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(aviary).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            aviaries.Add(aviary);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = aviary.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(aviary) > int.Parse(values_2[1]))
                                            {
                                                aviaries.Add(aviary);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(aviary) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                aviaries.Add(aviary);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = aviary.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(aviary) < int.Parse(values_3[1]))
                                            {
                                                aviaries.Add(aviary);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(aviary) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                aviaries.Add(aviary);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(aviaries);
                    }
                case "BabyAnimals":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetBabyAnimals(context).ToList());
                    }
                    else
                    {
                        List<BabyAnimal> babyAnimalList = context.BabyAnimals.ToList();
                        List<BabyAnimal> babyAnimals = new List<BabyAnimal>();
                        foreach (BabyAnimal babyAnimal in babyAnimalList)
                        {
                            foreach (PropertyInfo field in babyAnimal.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = babyAnimal.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(babyAnimal).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            babyAnimals.Add(babyAnimal);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = babyAnimal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(babyAnimal) > int.Parse(values_2[1]))
                                            {
                                                babyAnimals.Add(babyAnimal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(babyAnimal) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                babyAnimals.Add(babyAnimal);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = babyAnimal.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(babyAnimal) < int.Parse(values_3[1]))
                                            {
                                                babyAnimals.Add(babyAnimal);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(babyAnimal) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                babyAnimals.Add(babyAnimal);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(babyAnimalList);
                    }
                case "Days":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetDays(context).ToList());
                    }
                    else
                    {
                        List<Day> dayList = context.Days.ToList();
                        List<Day> days = new List<Day>();
                        foreach (Day day in dayList)
                        {
                            foreach (PropertyInfo field in day.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = day.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(day).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            days.Add(day);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = day.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(day) > int.Parse(values_2[1]))
                                            {
                                                days.Add(day);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(day) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                days.Add(day);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = day.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(day) < int.Parse(values_3[1]))
                                            {
                                                days.Add(day);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(day) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                days.Add(day);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(days);
                    }
                case "Employers":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetEmployers(context).ToList());
                    }
                    else
                    {
                        List<Employer> employerList = context.Employers.ToList();
                        List<Employer> employers = new List<Employer>();
                        foreach (Employer employer in employerList)
                        {
                            foreach (PropertyInfo field in employer.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = employer.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(employer).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            employers.Add(employer);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = employer.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(employer) > int.Parse(values_2[1]))
                                            {
                                                employers.Add(employer);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(employer) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                employers.Add(employer);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = employer.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(employer) < int.Parse(values_3[1]))
                                            {
                                                employers.Add(employer);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(employer) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                employers.Add(employer);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(employers);
                    }
                case "Events":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetEvents(context).ToList());
                    }
                    else
                    {
                        List<Event> eventList = context.Events.ToList();
                        List<Event> events = new List<Event>();
                        foreach (Event e in events)
                        {
                            foreach (PropertyInfo field in e.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = e.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(e).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            events.Add(e);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = e.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(e) > int.Parse(values_2[1]))
                                            {
                                                events.Add(e);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(e) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                events.Add(e);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = e.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(e) < int.Parse(values_3[1]))
                                            {
                                                events.Add(e);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(e) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                events.Add(e);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(events);
                    }
                case "WorkingShifts":
                    if (filter == "ALL")
                    {
                        return JsonSerializer.Serialize(Program.GetWorkingShifts(context).ToList());
                    }
                    else
                    {
                        List<WorkingShift> workingShiftList = context.WorkingShifts.ToList();
                        List<WorkingShift> shifts = new List<WorkingShift>();
                        foreach (WorkingShift shift in workingShiftList)
                        {
                            foreach (PropertyInfo field in shift.GetType().GetProperties())
                            {
                                if (values_1.Length > 1)
                                {
                                    if (field.Name == values_1[0])
                                    {
                                        fieldValue = shift.GetType()
                                                     .GetProperty(field.Name)
                                                     .GetValue(shift).ToString();
                                        if (String.Equals(fieldValue, values_1[1]))
                                        {
                                            shifts.Add(shift);
                                            break;
                                        }
                                    }
                                }
                                if (values_2.Length > 1)
                                {
                                    if (field.Name == values_2[0])
                                    {
                                        var cField = shifts.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(shift) > int.Parse(values_2[1]))
                                            {
                                                shifts.Add(shift);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(shift) > DateTime.ParseExact(values_2[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                shifts.Add(shift);
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (values_3.Length > 1)
                                {
                                    if (field.Name == values_3[0])
                                    {
                                        var cField = shift.GetType()
                                                      .GetProperty(field.Name);
                                        if (cField.PropertyType == typeof(int))
                                        {
                                            if ((int)cField.GetValue(shift) < int.Parse(values_3[1]))
                                            {
                                                shifts.Add(shift);
                                                break;
                                            }
                                        }
                                        if (cField.PropertyType == typeof(DateTime))
                                        {
                                            if ((DateTime)cField.GetValue(shift) < DateTime.ParseExact(values_3[1], "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture))
                                            {
                                                shifts.Add(shift);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return JsonSerializer.Serialize(shifts);
                    }
                default:
                    return "Такой таблицы нет";
            }
        }                 
    }

    
    public class InvokerORM
    {
        CommandORM command;
        public void SetCommand(CommandORM c)
        {
            command = c;
        }

        public string Run()
        {
            return command.Execute();
        }
    }
}