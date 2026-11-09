using System;
using System.Collections.Generic;
using System.Linq;

class Author
{
    public string Name { get; set; }
    public string Country { get; set; }
    public List<Book> Books { get; set; } = new List<Book>();

    public Author(string name, string country)
    {
        Name = name; Country = country;
    }

    public void ShowBooks()
    {
        Console.WriteLine($"Книги автора {Name}:");
        foreach (var b in Books) Console.WriteLine($"- {b.Title}");
    }
}
class Category
{
    public int Code { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; } = new List<Book>();

    public Category(int code, string name)
    {
        Code = code; Name = name;
    }
}

class Book
{
    public string ISBN { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public Author BookAuthor { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Book(string isbn, string title, int year, Author author)
    {
        ISBN = isbn; Title = title; Year = year;
        BookAuthor = author;
        author.Books.Add(this);
    }

    public void GetInfo()
    {
        Console.WriteLine($"Книга: '{Title}' ({Year}), Автор: {BookAuthor.Name}, Статус: {(IsAvailable ? "Доступна" : "Выдана")}");
    }
    public bool CheckAvailability() => IsAvailable;
}

class Loan
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public Book LoanedBook { get; set; }

    public Loan(int id, Book book, int days)
    {
        Id = id; LoanedBook = book;
        IssueDate = DateTime.Now;
        ReturnDate = DateTime.Now.AddDays(days);
    }
    public void Extend(int days) => ReturnDate = ReturnDate.AddDays(days);
    public bool IsOverdue() => DateTime.Now > ReturnDate;
}

class Reader
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public List<Loan> Loans { get; set; } = new List<Loan>();

    public Reader(int id, string name, string phone)
    {
        Id = id; Name = name; Phone = phone;
    }
    public void BorrowBook(Loan loan) => Loans.Add(loan);
    public void ReturnBook(Loan loan) => Loans.Remove(loan);
}
class Librarian
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Shift { get; set; }

    public Librarian(int id, string name, string shift)
    {
        Id = id; Name = name; Shift = shift;
    }

    public Loan IssueBook(Book book, Reader reader, int days)
    {
        if (book.CheckAvailability())
        {
            book.IsAvailable = false;
            Loan newLoan = new Loan(new Random().Next(100, 999), book, days);
            reader.BorrowBook(newLoan);
            Console.WriteLine($"[Операция] Библиотекарь {Name} выдал '{book.Title}' читателю {reader.Name}.");
            return newLoan;
        }
        Console.WriteLine($"[Ошибка] Книга '{book.Title}' сейчас недоступна.");
        return null;
    }

    public void AcceptReturn(Loan loan, Reader reader)
    {
        loan.LoanedBook.IsAvailable = true;
        reader.ReturnBook(loan);
        Console.WriteLine($"[Операция] Библиотекарь {Name} принял возврат '{loan.LoanedBook.Title}'.");
    }
}

class Library
{
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Book> BookList { get; set; } = new List<Book>();

    public Library(string name, string address)
    {
        Name = name; Address = address;
    }
    public void AddBook(Book book)
    {
        BookList.Add(book);
        Console.WriteLine($"[Библиотека] Добавлена книга: {book.Title}");
    }
}

class Fine
{
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public bool IsPaid { get; set; } = false;

    public void Calculate(Loan loan)
    {
        if (loan.IsOverdue())
        {
            int days = (DateTime.Now - loan.ReturnDate).Days;
            if (days > 0)
            {
                Amount = days * 50; Reason = $"Просрочка на {days} дней";
                Console.WriteLine($"[Штраф] Начислен: {Amount} руб. Причина: {Reason}");
            }
        }
        else Console.WriteLine("[Штраф] Штрафов нет. Сдано вовремя.");
    }
    public void Pay()
    {
        IsPaid = true;
        Console.WriteLine($"[Штраф] Оплачен на сумму {Amount} руб.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Library lib = new Library("Центральная", "ул. Пушкина, 10");
        Author author1 = new Author("Джордж Оруэлл", "Великобритания");
        Book book1 = new Book("978-045", "1984", 1949, author1);

        lib.AddBook(book1);
        Reader reader = new Reader(1, "Жаслан", "8-800-555-35-35");
        Librarian librarian = new Librarian(101, "Анна", "Дневная");

        Loan loan = librarian.IssueBook(book1, reader, 14);
        librarian.IssueBook(book1, reader, 7);

        Fine fine = new Fine();
        fine.Calculate(loan);
        librarian.AcceptReturn(loan, reader);
    }
}