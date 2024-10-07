using Blog_Engine_2;
using Blog_Engine_2.Objects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Data;

namespace BaseTycoon;

static internal class Program
{
    private static async Task Main()
    {
        try
        {
            Console.Write("Введи путь к appsettings.json или оставь пустым, если файл в той же директории >>> ");
            var input = Console.ReadLine();
            var path = string.IsNullOrWhiteSpace(input) ? "appsettings.json" : input;

            var root = JObject.Parse(await File.ReadAllTextAsync(path));

            var connectionStrings =
                root.DescendantsAndSelf()
                    .OfType<JProperty>()
                    .Where(p => p is { Name: "ConnectionStrings" })
                    .Select(p => p.Value)
                    .FirstOrDefault()?
                    .ToObject<Dictionary<string, string>>();

            if (connectionStrings is null || connectionStrings.Count == 0)
            {
                throw new("Не найдено строк подключения.");
            }

            Console.WriteLine();

            var keys = connectionStrings.Keys.ToArray();
            for (var i = 0; i < connectionStrings.Count; i++)
            {
                Console.WriteLine($"{i}) {keys[i]}: {connectionStrings[keys[i]]}");
            }

            int selected;
            do
            {
                Console.Write("Выберите строку подключения >>> ");
                if (int.TryParse(Console.ReadLine(), out selected) && selected < connectionStrings.Count && selected >= 0)
                {
                    break;
                }

                Console.WriteLine("Такой строки подключения нет");
            } while (true);

            var connectionString = connectionStrings[keys[selected]];
            var optsBuilder = new DbContextOptionsBuilder<Context>();
            optsBuilder.UseSqlServer(connectionString);

            await using var context = new Context(optsBuilder.Options);

            while (true)
            {
                var counter = 0;
                Console.WriteLine($"{counter++}) Добавить Автора");
                Console.WriteLine($"{counter++}) Удалить Автора");
                Console.WriteLine($"{counter++}) Список Авторов");
                Console.WriteLine($"{counter++}) Создать базу данных");
                Console.WriteLine($"{counter}) Удалить базу данных");
                Console.WriteLine();
                Console.Write("Выберите действие >>> ");

                if (!int.TryParse(Console.ReadLine(), out selected))
                {
                    Console.WriteLine("Неверный ввод");
                    continue;
                }

                Console.WriteLine();

                switch ((Actions)selected)
                {
                    case Actions.AddAuthor:
                        await AddAuthor(context);
                        break;
                    case Actions.RemoveAuthor:
                        await RemoveAuthor(context);
                        break;
                    case Actions.ListAuthors:
                        await ListAuthors(context);
                        break;
                    case Actions.CreateDatabase:
                        await CreateDb(context);
                        break;
                    case Actions.DeleteDatabase:
                        await DeleteDb(context);
                        break;
                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }

                Console.WriteLine("Тыкни клавишу, чтобы продолжить");
                Console.ReadKey();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        Console.ReadKey();

    }

    private static async Task AddAuthor(Context context)
    {
        Console.Write("Введите логин >>> ");
        var login = Console.ReadLine()!;

        if (login.Length > Constants.MaxLength || string.IsNullOrWhiteSpace(login))
        {
            Console.WriteLine($"Логин не должен быть пустым и должен быть не длиннее {Constants.MaxLength} символов");
            return;
        }

        if (context.Users.SingleOrDefault(u => u.Login == login) is not null)
        {
            Console.WriteLine("Логин уже используется");
            return;
        }

        Console.Write("Введите пароль >>> ");
        var password = Console.ReadLine()!;

        if (password.Length > Constants.MaxLength || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine($"Пароль не должен быть пустым и должен быть не длиннее {Constants.MaxLength} символов");
            return;
        }

        context.Users.Add(new User { Login = login, Password = password, Role = RoleUser.Author });

        await context.SaveChangesAsync();
        Console.WriteLine("Успешно!");
    }

    private static async Task<List<string>> ListAuthors(Context context)
    {
        var authors = await context.Users
            .Where(u => u.Role == RoleUser.Author)
            .Select(u => u.Login)
            .ToListAsync();

        if (authors.Count == 0)
        {
            Console.WriteLine("Авторов не найдено");
        }
        else
        {
            for (var i = 0; i < authors.Count; i++)
            {
                Console.WriteLine($"{i}) {authors[i]}");
            }
        }

        return authors;
    }

    private static async Task RemoveAuthor(Context context)
    {
        var authors = await ListAuthors(context);

        if (authors.Count == 0)
        {
            Console.WriteLine("Удалять некого");
            return;
        }

        Console.WriteLine();

        int selected;
        do
        {
            Console.Write("Введите номер Автора >>> ");
            if (int.TryParse(Console.ReadLine(), out selected) && selected < authors.Count && selected >= 0)
            {
                break;
            }

            Console.WriteLine("Неверный ввод");
        } while (true);

        if (AskConfirmation($"Удалить {authors[selected]}?"))
        {
            var user = await context.Users.SingleOrDefaultAsync(u => u.Login == authors[selected]);

            if (user is null)
            {
                Console.WriteLine("Автор не найден");
                return;
            }

            context.Remove(user);
            await context.SaveChangesAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static async Task CreateDb(Context context)
    {
        if (AskConfirmation("Это удалит предыдущую базу, если она есть. Продолжить?"))
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static async Task DeleteDb(Context context)
    {
        if (AskConfirmation("Это удалит базу, если она есть. Продолжить?"))
        {
            await context.Database.EnsureDeletedAsync();

            Console.WriteLine("Успешно!");
        }
    }

    private static bool AskConfirmation(string prompt)
    {
        Console.Write($"{prompt} (y/n) >>> ");
        return Console.ReadLine()!.ToLower() == "y";
    }

    private enum Actions
    {
        AddAuthor,
        RemoveAuthor,
        ListAuthors,
        CreateDatabase,
        DeleteDatabase
    }
}
