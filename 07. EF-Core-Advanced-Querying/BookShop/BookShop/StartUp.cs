namespace BookShop
{
    using BookShop.Data;
    using BookShop.Initializer;
    using BookShop.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //// 0.	Book Shop Database
                //DbInitializer.ResetDatabase(db);

                //`var input = Console.ReadLine();

                //// 1.	Age Restriction
                //Console.WriteLine(GetBooksByAgeRestriction(db, input)); 

                //// 2.	Golden Books
                //Console.WriteLine(GetGoldenBooks(db));

                //// 3.	Books by Price
                //Console.WriteLine(GetBooksByPrice(db));

                //// 4.	Not Released In
                //var year = int.Parse(input);
                //Console.WriteLine(GetBooksNotRealeasedIn(db, input));

                //// 5.	Book Titles by Category
                //Console.WriteLine(GetBooksByCategory(db, input));

                //// 6.	Released Before Date
                //Console.WriteLine(GetBooksReleasedBefore(db, input));

                //// 7.	Author Search
                //Console.WriteLine(GetAuthorNamesEndingIn(db, input));

                //// 8.	Book Search
                //Console.WriteLine(GetBookTitlesContaining(db, input));

                //// 9.	Book Search by Author
                //Console.WriteLine(GetBooksByAuthor(db, input));

                //// 10. Count Books
                //var lengthCheck = int.Parse(input);
                //Console.WriteLine(CountBooks(db, lengthCheck));

                //// 11. Total Book Copies
                //Console.WriteLine(CountCopiesByAuthor(db));

                //// 12. Profit by Category
                //Console.WriteLine(GetTotalProfitByCategory(db));

                //// 13. Most Recent Books
                Console.WriteLine(GetMostRecentBooks(db));
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction.Equals(ageRestriction))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var editionType = (EditionType)Enum.Parse(typeof(EditionType), "Gold");

            var books = context.Books
                .Where(b => b.EditionType.Equals(editionType) && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .ToArray();

            var result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return result.ToString().Trim();
        }

        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => !b.ReleaseDate.Value.Year.Equals(year))
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries).ToArray();

            var books = context.Books
                .Where(b => b.BookCategories
                    .Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => DateTime.Compare(b.ReleaseDate.Value, dateTime) < 0)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    EditionType = Enum.GetName(typeof(EditionType), b.EditionType),
                    Price = b.Price
                })
                .ToArray();

            var result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return result.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Books
                .Where(b => EF.Functions.Like(b.Author.FirstName, $"%{input}"))
                .Select(b => new
                {
                    FullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .OrderBy(a => a.FullName)
                .Distinct()
                .ToList();

            var result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.FullName}");
            }

            return result.ToString().Trim();
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => EF.Functions.Like(b.Title.ToLower(), $"%{input.ToLower()}%"))
                .Select(b => new
                {
                    Title = b.Title
                })
                .OrderBy(a => a.Title)
                .ToArray();

            var result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book.Title);
            }

            return result.ToString().Trim();
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => EF.Functions.Like(b.Author.LastName.ToLower(), $"{input.ToLower()}%"))
                .Select(b => new
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .OrderBy(a => a.BookId)
                .ToArray();

            var result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} ({book.AuthorFullName})");
            }

            return result.ToString().Trim();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    BooksCount = a.Books.Select(b => b.Copies).Sum()
                })
                .OrderByDescending(a => a.BooksCount)
                .ToArray();


            var result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine($"{author.AuthorName} - {author.BooksCount}");
            }

            return result.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {

                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(a => a.TotalProfit)
                .ThenBy (a => a.CategoryName)
                .ToList();

            var result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"{category.CategoryName} ${category.TotalProfit:F2}");
            }

            return result.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks.OrderByDescending(cb => cb.Book.ReleaseDate).Take(3)
                        .Select(b => new
                        {
                            BookTitle = b.Book.Title,
                            ReleaseYear = b.Book.ReleaseDate.Value.Year
                        })
                })
                .OrderBy(a => a.CategoryName)
                .ToArray();

            var result = new StringBuilder();

            foreach (var category in categories)
            {
                result.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Books)
                {
                    result.AppendLine($"{book.BookTitle} ({book.ReleaseYear})");
                }
            }

            return result.ToString().Trim();
        }
    }
}
