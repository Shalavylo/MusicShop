using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MusicStore.Library.data;
using MusicStore.Library.Models;
using MusicStore.Library.Services;

internal class Program
{
    private static readonly MusicStoreContext db = new();
    private static AppUser? currentUser;
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        StoreInitializer.Initialize(db);

        while (true)
        {
            Console.WriteLine("===== Music Store =====");
            Console.WriteLine("1. Увійти");
            Console.WriteLine("2. Реєстрація покупця");
            Console.WriteLine("0. Вийти");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Login();
                if (currentUser != null)
                    MainMenu();
            }
            else if (choice == "2")
                RegisterCustomer();
            else if (choice == "0")
                return;
            else
                Console.WriteLine("Невірний вибір!");
        }
    }

    private static void Login()
    {
        Console.Write("Логін --> ");
        string login = Console.ReadLine() ?? "";

        Console.Write("Пароль --> ");
        string password = Console.ReadLine() ?? "";

        var user = db.AppUsers
            .Include(a => a.Customer)
            .FirstOrDefault(a => a.Login == login && a.Password == password);

        if(user !=  null)
        {
            currentUser = user;
            Console.WriteLine("Вхід успішний!");
        }
        else
        {
            Console.WriteLine("Неправильний логін або пароль!");
        }
    }

    private static string ReadText(string promt)
    {
        Console.Write(promt);
        return Console.ReadLine()?.Trim() ?? "";
    }

    private static int ReadInt(string promt)
    {
        while (true)
        {
            Console.Write(promt);
            if (int.TryParse(Console.ReadLine(), out int value)) 
                return value;
            Console.WriteLine("Введіть ціле число.");
        }
    }

    private static void RegisterCustomer()
    {
        string name = ReadText("Введіть ім'я та прізвище --> ");
        string phone = ReadText("Телефон --> ");
        string login = ReadText("Логін --> ");
        string password = ReadText("Пароль --> ");

        if(string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(phone)
            || string.IsNullOrWhiteSpace(login)
            || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Ця інформація повинна бути заповнена!");
            return;
        }
        if(db.AppUsers.Any(a => a.Login == login))
        {
            Console.WriteLine("Цей логін вже зайнятий!");
            return;
        }

        var customer = new Customer
        { 
            FullName = name, Phone = phone 
        };
        var user = new AppUser
        {
            Login = login,
            Password = password,
            Role = "Customer",
            Customer = customer
        };

        db.AppUsers.Add(user);
        db.SaveChanges();
        Console.WriteLine("Покупця зареєстровано!");
    }

    private static void MainMenu()
    {
        while (currentUser != null)
        {
            Console.WriteLine("===== Головне Меню =====");
            Console.WriteLine("1. Асортимент платівок");
            Console.WriteLine("2. Пошук платівок");
            Console.WriteLine("3. Новинки");
            Console.WriteLine("4. Продати платівку");
            Console.WriteLine("5. Відкласти платівку");
            Console.WriteLine("6. Мої відкладені платівки");
            if (IsAdmin())
            {
                Console.WriteLine("7. Додати платівку");
                Console.WriteLine("8. Редагувати платівку");
                Console.WriteLine("9. Видалити платівку");
                Console.WriteLine("10. Списати платівку");
                Console.WriteLine("11. Створити акцію");
                Console.WriteLine("12. Звіти популярності");
                Console.WriteLine("13. Список покупців");
            }
            Console.WriteLine("0. Вийти з акаунта");

            Console.Write("Ваш вибір --> ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAlbums(); 
                    break;
                case "2":
                    SearchAlbums();
                    break;
                case "3":
                    ShowNewAlbums();
                    break;
                case "4":
                    SellAlbum();
                    break;
                case "5":
                    ReserveAlbum();
                    break;
                case "6":
                    ShowMyReservations();
                    break;
                case "7" when IsAdmin():
                    AddAlbum();
                    break;
                case "8" when IsAdmin():
                    EditAlbum();
                    break;
                case "9" when IsAdmin():
                    DeleteAlbum();
                    break;
                case "10" when IsAdmin():
                    WriteOffAlbum();
                    break;
                case "11" when IsAdmin():
                    AddPromotion();
                    break;
                case "12" when IsAdmin():
                    ShowPopularityReport();
                    break;
                case "13" when IsAdmin():
                    ShowCustomers();
                    break;
                case "0":
                    currentUser = null;
                    break;
                default:
                    Console.WriteLine("Невірний вибір або немає прав.");
                    break;
            }
        }
    }

    private static bool IsAdmin() => currentUser?.Role == "Admin";
    private static void PrintAlbums(IEnumerable<MusicClasses> albums)
    {
        Console.WriteLine("\nID | Назва | Виконавець | Видавець | Треки | Жанр | Рік | Собівартість | Ціна | Залишок");
        foreach (MusicClasses album in albums)
        {
            Console.WriteLine($"{album.Id} | " +
                $"{album.Title} | {album.Artist} " +
                $"| {album.Publisher} | {album.TrackCount} " +
                $"| {album.Genre} | {album.ReleaseYear} " +
                $"| {album.Cost:F2} | {album.Price:F2} " +
                $"| {album.Quantity}");
        }
    }

    private static void ShowAlbums()
    {
        var a = db.MusicClassess.OrderBy(x => x.Title).ToList();
        PrintAlbums(a);
    }

    private static void AddAlbum()
    {
        string title = ReadText("Назва: ");
        string artist = ReadText("Виконавець/колектив: ");
        string publisher = ReadText("Видавець: ");
        int trackcount = ReadInt("Кількість треків: ");
        string genre = ReadText("Жанр: ");
        int releaseYear = ReadInt("Рік виходу: ");
        Console.Write("Собівартість: ");
        decimal cost = decimal.Parse(Console.ReadLine()!);
        Console.Write("Ціна продажу: ");
        decimal price = decimal.Parse(Console.ReadLine()!);
        int quantity = ReadInt("Кількість на складі: ");

        var album = new MusicClasses
        {
            Title = title,
            Artist = artist,
            Publisher = publisher,
            TrackCount = trackcount,
            Genre = genre,
            ReleaseYear = releaseYear,
            Cost = cost,
            Price = price,
            Quantity = quantity,
            AddedAt = DateTime.Now
        };

        if (string.IsNullOrWhiteSpace(album.Title) ||
            string.IsNullOrWhiteSpace(album.Artist) ||
            string.IsNullOrWhiteSpace(album.Publisher) ||
            string.IsNullOrWhiteSpace(album.Genre) ||
            album.TrackCount <= 0 || album.ReleaseYear < 1900 ||
            album.Cost < 0 || album.Price < 0 || album.Quantity < 0)
        {
            Console.WriteLine("Перевірте введені дані.");
            return;
        }

        db.MusicClassess.Add(album);
        db.SaveChanges();
        Console.WriteLine("Платівку додано!");
    }

    private static void EditAlbum()
    {
        ShowAlbums();
        var album = db.MusicClassess.Find(ReadInt("Id платівки -->"));
        if(album == null)
        {
            Console.WriteLine("Платівки не знайдено!");
            return;
        }

        string title = ReadText("Назва: ");
        string artist = ReadText("Виконавець/колектив: ");
        string publisher = ReadText("Видавець: ");
        int trackcount = ReadInt("Кількість треків: ");
        string genre = ReadText("Жанр: ");
        int releaseYear = ReadInt("Рік виходу: ");
        Console.Write("Собівартість: ");
        decimal cost = decimal.Parse(Console.ReadLine()!);
        Console.Write("Ціна продажу: ");
        decimal price = decimal.Parse(Console.ReadLine()!);
        int quantity = ReadInt("Кількість на складі: ");

        album.Title = title;
        album.Artist = artist;
        album.Publisher = publisher;
        album.TrackCount = trackcount;
        album.ReleaseYear = releaseYear;
        album.Quantity = quantity;
        album.Genre = genre;
        album.Cost = cost;
        album.Price = price;

        if (string.IsNullOrWhiteSpace(album.Title) ||
           string.IsNullOrWhiteSpace(album.Artist) ||
           string.IsNullOrWhiteSpace(album.Publisher) ||
           string.IsNullOrWhiteSpace(album.Genre) ||
           album.TrackCount <= 0 || album.ReleaseYear < 1900 ||
           album.Cost < 0 || album.Price < 0 || album.Quantity < 0)
        {
            Console.WriteLine("Некоректні дані. Скасовано.");
            db.Entry(album).Reload();
            return;
        }

        Console.WriteLine("Дані оновлено!");
        db.SaveChanges();
    }

    private static void DeleteAlbum()
    {
        ShowAlbums();
        int id = ReadInt("Введіть Id: ");
        var album = db.MusicClassess.Find(id);
        if (album == null)
        {
            Console.WriteLine("Id не знайденно!");
            return;
        }

        bool linked = db.Sales.Any(s => s.MusicClassesId == id) ||
                     db.WriteOffs.Any(w => w.MusicClassesId == id) ||
                     db.Reservations.Any(r => r.MusicClassesId == id);
        if (linked)
        {
            Console.WriteLine("Є історія операцій. Видалення заборонено; встановіть залишок 0 або списання.");
            return;
        }

        db.MusicClassess.Remove(album);
        db.SaveChanges();
        Console.WriteLine("Платівку видалено.");
    }

    private static void SellAlbum()
    {
        ShowAlbums();
        var album = db.MusicClassess.Find(ReadInt("Id платівки: "));
        if (album == null)
        {
            Console.WriteLine("Id не знайденно!");
            return;
        }

        int qty = ReadInt("Кількість: ");
        int activeReserved = db.Reservations
            .Where(a => a.MusicClassesId == album.Id && a.Status == "Active").Sum(r => (int?)r.Quantity) ?? 0;

        if (qty <= 0 || qty > album.Quantity - activeReserved)
        {
            Console.WriteLine("Недостатньо вільного товару.");
            return;
        }

        Customer? customer = currentUser!.CustomerId == null
            ? null
            : db.Customers.Find(currentUser.CustomerId.Value);
        
        if(IsAdmin())
        {
            int costomerId = ReadInt("Id покупця: ");
            if(costomerId != 0)
            {
                customer = db.Customers.Find(costomerId);
                if(customer == null)
                {
                    Console.WriteLine("Id незнайдено!");
                    return;
                }
            }
        }

        decimal discount = 0;

        // Знижка постійного клієнта
        if (customer != null)
        {
            discount = GetLoyaltyDiscount(customer.TotalSpend);
        }

        // Шукаємо активну акцію для жанру альбому
        var promo = db.Promotions.FirstOrDefault(p =>
            p.Genre == album.Genre &&
            p.StartDate <= DateTime.Now &&
            p.EndDate >= DateTime.Now);

        if (promo != null && promo.DiscountPercent > discount)
        {
            discount = promo.DiscountPercent;
        }

        decimal unitPrice = album.Price * (1 - discount / 100);
        decimal total = unitPrice * qty;

        album.Quantity -= qty;
        if (customer != null)
        {
            customer.TotalSpend += total;
        }

        Sale sale = new Sale
        {
            MusicClassesId = album.Id,
            CustomerId = customer == null ? null : customer.Id,
            Quantity = qty,
            UnitPrice = unitPrice,
            DiscountPercent = discount,
            SoldAt = DateTime.Now
        };

        db.Sales.Add(sale);
        db.SaveChanges();

        Console.WriteLine("Знижка: " + discount + "%");
        Console.WriteLine("До сплати: " + total.ToString("F2") + " грн");
    }

    private static decimal GetLoyaltyDiscount(decimal spent)
    {
        if (spent >= 10000)
            return 10;
        if (spent >= 5000)
            return 5;
        if (spent >= 2000)
            return 3;
        return 0;
    }

    private static void WriteOffAlbum()
    {
        ShowAlbums();
        var album = db.MusicClassess.Find(ReadInt("Id платівки: "));
        if(album == null)
        {
            Console.WriteLine("Платівки не знайдено!");
            return;
        }

        int qty = ReadInt("Кількість: ");
        int reserved = db.Reservations
            .Where(r => r.MusicClassesId == album.Id && r.Status == "Active")
            .Sum(r => (int?)r.Quantity) ?? 0;

        if (qty <= 0 || qty > album.Quantity - reserved)
        {
            Console.WriteLine("Некоректна кількість або товар зарезервований.");
            return;
        }

        string reason = ReadText("Причина списання: ");
        album.Quantity -= qty;
        db.WriteOffs.Add(new WriteOff
        {
            MusicClassesId = album.Id,
            Quantity = qty,
            Reason = reason,
            Date = DateTime.Now
        });
        db.SaveChanges();
        Console.WriteLine("Списання виконано!");
    }

    private static void AddPromotion()
    {
        string name = ReadText("Назва акції -> ");
        string genre = ReadText("Жанр -> ");
        Console.Write("Знижка у відсотках -> ");
        decimal discount = decimal.Parse(Console.ReadLine());
        int days = ReadInt("Тривалість у днях -> ");

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(genre) ||
            discount <= 0 || discount > 100 || days <= 0)
        {
            Console.WriteLine("Некоректні параметри.");
            return;
        }

        db.Promotions.Add(new Promotion
        {
            Name = name,
            Genre = genre,
            DiscountPercent = discount,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(days)
        });
        db.SaveChanges();
        Console.WriteLine("Акцію створено!");
    }

    private static void ReserveAlbum()
    {
        ShowAlbums();
        var album = db.MusicClassess.Find(ReadInt("Id платівки: "));
        if (album == null)
        {
            Console.WriteLine("Платівки не знайдено!");
            return;
        }

        Customer? customer = currentUser!.CustomerId == null
            ? null
            : db.Customers.Find(currentUser.CustomerId.Value);
        if(IsAdmin())
        {
            db.Customers.Find(ReadText("Id покупця -> "));
        }
        if (customer == null)
        {
            Console.WriteLine("Для резервування потрібен зареєстрований покупець.");
            return;
        }

        int qty = ReadInt("Кількість для відкладення: ");
        int reserved = db.Reservations
            .Where(r => r.MusicClassesId == album.Id && r.Status == "Active")
            .Sum(r => (int?)r.Quantity) ?? 0;
        if(qty <= 0 || qty > album.Quantity - reserved)
        {
            Console.WriteLine("Недостатньо вільних платівок!");
            return;
        }
        db.Reservations.Add(new Reservation
        {
            MusicClassesId = album.Id,
            CustomerId = customer.Id,
            Quantity = qty,
            ReservedAt = DateTime.Now,
            Status = "Active"
        });
        db.SaveChanges();
        Console.WriteLine("Платівку відкладено!");
    }

    private static void ShowMyReservations()
    {
        IQueryable<Reservation> reservations = db.Reservations.Include(a => a.MusicClasses)
            .Include(a => a.Customer).Where(a => a.Status == "Active");
        if(!IsAdmin())
            reservations = reservations.Where(a => a.CustomerId == currentUser!.CustomerId);

        var rows = reservations.ToList();
        foreach (var item in rows)
        {
            Console.WriteLine($"Id: {item.Id} | {item.MusicClasses!.Title} | {item.Customer!.FullName} | К-сть: {item.Quantity} | {item.ReservedAt:g}");
        }
        if(!rows.Any())
        {
            Console.WriteLine("Активних резервувань немає.");
        }
    }

    private static void SearchAlbums()
    {
        string name = ReadText("Назва -> ");
        string author = ReadText("Виконавець -> ");
        string genre = ReadText("Жанр -> ");

        var query = db.MusicClassess.AsQueryable();
        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(a => a.Title.Contains(name));
        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(a => a.Artist.Contains(author));
        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(a => a.Genre.Contains(genre));

        PrintAlbums(query.OrderBy(a => a.Title).ToList());
    }

    private static void ShowNewAlbums()
    {
        var rows = db.MusicClassess.Where(a => a.AddedAt >= DateTime.Now.AddDays(-30))
            .OrderByDescending(a => a.AddedAt).ToList();
        Console.WriteLine("Новинки (додані за останні 30 днів):");
        PrintAlbums(rows);
    }

    private static void ShowPopularityReport()
    {
        Console.WriteLine("1 - День");
        Console.WriteLine("2 - Тиждень");
        Console.WriteLine("3 - Місяць");
        Console.WriteLine("4 - Рік");

        int period = ReadInt("Оберіть період: ");

        DateTime start;

        if (period == 1)
        {
            start = DateTime.Today;
        }
        else if (period == 2)
            start = DateTime.Today.AddDays(-7);
        else if(period == 3)
            start = DateTime.Today.AddMonths(-1);
        else if(period == 4)
            start = DateTime.Today.AddYears(-1);
        else
        {
            Console.WriteLine("Період не вірний!");
            return;
        }

        var sales = db.Sales.Include(a => a.MusicClasses)
            .Where(a => a.SoldAt >= start).ToList();
        Console.WriteLine("\nПопулярні платівки: ");

        var albums = sales.GroupBy(a => a.MusicClasses.Title)
            .Select(g => new
            {
                Name = g.Key,
                Quantity = g.Sum(a => a.Quantity)
            }).OrderByDescending(a => a.Quantity).Take(10);

        foreach (var album in albums)
        {
            Console.WriteLine(album.Name + ": " + album.Quantity);
        }

        Console.WriteLine("\nПопулярні виконавці: ");
        var artists = sales.GroupBy(a => a.MusicClasses.Artist)
            .Select(g => new
            {
                Name = g.Key,
                Quantity = g.Sum(a => a.Quantity)
            }).OrderByDescending(a => a.Quantity).Take(10);
        foreach (var artist in artists)
        {
            Console.WriteLine(artist.Name + ": " + artist.Quantity);
        }

        Console.WriteLine("\nПопулярні жанри: ");
        var genres = sales.GroupBy(s => s.MusicClasses.Genre)
            .Select(g => new
            {
                Name = g.Key,
                Quantity = g.Sum(a => a.Quantity)
            }).OrderByDescending(a => a.Quantity).Take(10);
        foreach (var genre in genres)
        {
            Console.WriteLine(genre.Name + ": " + genre.Quantity);
        }
    }

    private static void ShowCustomers()
    {
        foreach (var c in db.Customers.OrderByDescending(a => a.TotalSpend).ToList())
        {
            Console.WriteLine($"ID {c.Id} | {c.FullName} | {c.Phone} | Витрачено: {c.TotalSpend:F2} | Знижка: {GetLoyaltyDiscount(c.TotalSpend)}%");
        }
    }
}