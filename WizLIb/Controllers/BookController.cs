using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WizLib_DataAccess.Data;
using WizLib_Model.Models;
using WizLib_Model.ViewModels;

namespace WizLib.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Book> objList = _db.Books.Include(b => b.Publisher).Include(b => b.BookAuthors).ThenInclude(ba => ba.Author).ToList();
            //List<Book> objList = _db.Books.ToList();
            //foreach (var obj in objList)
            //{
            //    //Least Efficient
            //    //obj.Publisher = _db.Publishers.FirstOrDefault(p => p.Publisher_Id == obj.Publisher_Id);

            //    //Explicit Loading More Efficient
            //    _db.Entry(obj).Reference(b => b.Publisher).Load();
            //    _db.Entry(obj).Collection(b => b.BookAuthors).Load();
            //    foreach(var bookAuth in obj.BookAuthors)
            //    {
            //        _db.Entry(bookAuth).Reference(u => u.Author).Load();
            //    }
            //}
            return View(objList);
        }

        public IActionResult AddEdit(int? id)
        {
            BookVM obj = new BookVM();
            obj.PublisherList = _db.Publishers.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Publisher_Id.ToString()
            });
            if (id == null)
            {   //Add
                return View(obj);
            }
            obj.Book = _db.Books.FirstOrDefault(u => u.Book_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(BookVM obj)
        {
            if (obj.Book.Book_Id == 0)
            {
                //Create new
                _db.Books.Add(obj.Book);
            }
            else
            {
                //Update Category
                _db.Books.Update(obj.Book);
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            BookVM obj = new BookVM();

            if (id == null)
            {   //Add
                return View(obj);
            }
            //Edit
            obj.Book = _db.Books.Include(b => b.BookDetail).FirstOrDefault(u => u.Book_Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(BookVM obj)
        {
            if (obj.Book.BookDetail.BookDetail_Id == 0)
            {
                //Create new
                _db.BookDetails.Add(obj.Book.BookDetail);
                _db.SaveChanges();
                var BookFromDb = _db.Books.FirstOrDefault(b => b.Book_Id == obj.Book.Book_Id);
                BookFromDb.BookDetail_Id = obj.Book.BookDetail.BookDetail_Id;
                _db.SaveChanges();
            }
            else
            {
                //Update Category
                _db.BookDetails.Update(obj.Book.BookDetail);
                _db.SaveChanges();
            }
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var objFromDb = _db.Books.FirstOrDefault(c => c.Book_Id == id);
            _db.Books.Remove(objFromDb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ManageAuthors(int id)
        {
            BookAuthorVM obj = new BookAuthorVM
            {
                BookAuthorList = _db.BookAuthors.Include(ba => ba.Author).Include(ba => ba.Book)
                                    .Where(ba => ba.Book_Id == id).ToList(),
                BookAuthor = new BookAuthor()
                {
                    Book_Id = id
                },
                Book = _db.Books.FirstOrDefault(b => b.Book_Id == id)
            };
            List<int> tempListOfAssignedAuthors = obj.BookAuthorList.Select(ba => ba.Author_Id).ToList();
            //NOT IN Clause in LINQ
            //get all the Authors whose Id is not in tempListOfAssignedAuthors
            var tempList = _db.Authors.Where(a => !tempListOfAssignedAuthors.Contains(a.Author_Id)).ToList();

            obj.AuthorList = tempList.Select(a => new SelectListItem
            {
                Text = a.FullName,
                Value = a.Author_Id.ToString()
            });

            return View(obj);
        }

        [HttpPost]
        public IActionResult ManageAuthors(BookAuthorVM bookAuthorVM)
        {
            if (bookAuthorVM.BookAuthor.Book_Id != 0 && bookAuthorVM.BookAuthor.Author_Id != 0)
            {
                _db.BookAuthors.Add(bookAuthorVM.BookAuthor);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(ManageAuthors), new { @id = bookAuthorVM.BookAuthor.Book_Id }); 
        }

        [HttpPost]
        public IActionResult RemoveAuthors(int authorId, BookAuthorVM bookAuthorVM)
        {
            int bookId = bookAuthorVM.Book.Book_Id;
            BookAuthor bookAuthor = _db.BookAuthors.FirstOrDefault(
                ba => ba.Author_Id == authorId && ba.Book_Id == bookId);
            _db.BookAuthors.Remove(bookAuthor);
            _db.SaveChanges();

            return RedirectToAction(nameof(ManageAuthors), new { @id = bookId });
        }

        public IActionResult PlayGround()
        {
            //var bookTemp = _db.Books.FirstOrDefault();
            //bookTemp.Price = 100;

            //var bookCollection = _db.Books;
            //double totalPrice = 0;

            //foreach (var book in bookCollection)
            //{
            //    totalPrice += book.Price;
            //}

            //var bookList = _db.Books.ToList();
            //foreach (var book in bookList)
            //{
            //    totalPrice += book.Price;
            //}

            //var bookCollection2 = _db.Books;
            //var bookCount1 = bookCollection2.Count();

            //var bookCount2 = _db.Books.Count();


            ////Least Efficient as it return all books then filters in memory
            //IEnumerable<Book> BookList1 = _db.Books;
            //var filteredBook1 = BookList1.Where(b => b.Price > 500).ToList();

            ////More efficient, Filters when selecting from DB
            //IEnumerable<Book> BookList1b = _db.Books.Where(b => b.Price > 500).ToList();
            ////Even More Efficient, Filters when selecting from Db
            //IEnumerable<Book> BookList1c;
            //BookList1c = _db.Books.Where(b => b.Price > 500).ToList();

            ////More efficient, Filters when selecting from DB
            //IQueryable<Book> BookList2 = _db.Books;
            //var filteredBook2 = BookList2.Where(b => b.Price > 500).ToList();
            ////Even More Efficient, Filters when selecting from Db
            //List<Book> BookList2b = _db.Books.Where(b => b.Price > 500).ToList();


            //Manually chage entity state
            //var category = _db.Categories.FirstOrDefault();
            //_db.Entry(category).State = EntityState.Modified;

            //Updating Related Data
            //var BookTemp1 = _db.Books.Include(b => b.BookDetail).FirstOrDefault(b => b.BookDetail_Id == 2);
            //BookTemp1.BookDetail.NumberOfChapters = 2222;
            ////Update sets all to modified state which updates everything
            //_db.Books.Update(BookTemp1);
            //_db.SaveChanges();

            //var BookTemp2 = _db.Books.Include(b => b.BookDetail).FirstOrDefault(b => b.BookDetail_Id == 2);
            //BookTemp2.BookDetail.Weight = 3333;
            ////Attach switches to Add for new generated Ids & updates exiting, useful for mixed objects
            //_db.Books.Attach(BookTemp2);
            //_db.SaveChanges();

            //VIEWS
            var viewList = _db.BookDetailsFromView.ToList();
            var viewList1 = _db.BookDetailsFromView.FirstOrDefault();
            var viewList2 = _db.BookDetailsFromView.Where(bd => bd.Price > 500);

            //RAW SQL
            var bookRaw = _db.Books.FromSqlRaw("Select * from dbo.books").ToList(); //Not to be used with parameters due to SQL Injection

            int id = 2;
            var bookTemp1 = _db.Books.FromSqlInterpolated($"Select * from dbo.books where Bood_Id={id}").ToList();

            var booksSproc = _db.Books.FromSqlInterpolated($"EXEC dbo.getAllBookDetails {id}");

            //.NET 5 only
            var BookFilter1 = _db.Books.Include(b => b.BookAuthors.Where(ba => ba.Author_Id == 5)).ToList();
            var BookFilter1 = _db.Books.Include(b => b.BookAuthors.OrderByDescending(ba => ba.Author_Id).Take(2)).ToList();

            return RedirectToAction(nameof(Index));
        }
    }
}
