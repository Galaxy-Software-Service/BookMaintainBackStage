using BookMaintainBackStage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BookMaintainBackStage.Controllers
{
    // TODO: 網址傳參數防呆
    public class BookController : Controller
    {
        /// <summary>
        /// 在這個 Controller 類別中，包含了以下幾個 Action 方法：
            //Index：用來查詢顯示所有書籍的列表。
            //Create：用來新增書籍的頁面。
            //Delete：用來刪除書籍的頁面。
            //Edit：用來編輯書籍的頁面。
            //Detail:用來顯示書籍明細的頁面。
            //BorrowedRecord：用來查看指定書籍的借閱紀錄。
        /// </summary>

        readonly Models.CodeService codeService = new Models.CodeService();
        readonly Models.BookService bookService = new Models.BookService();

        [HttpGet()]
        public ActionResult Index()
        {
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            //借閱人
            ViewBag.UserEnameSelectList = this.codeService.GetUserCodeTable();
            //借閱狀態
            ViewBag.BookStatusSelectList = this.codeService.GetBookStatusCodeTable();

            return View();
        }

        /// <summary>
        /// 書籍資料查詢
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Index(Models.BookSearchArg arg)
        {
            //設定下拉選單資料
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            //借閱人
            ViewBag.UserEnameSelectList = this.codeService.GetUserCodeTable();
            //借閱狀態
            ViewBag.BookStatusSelectList = this.codeService.GetBookStatusCodeTable();
            //搜尋
            ViewBag.SearchResult = bookService.GetBooksByCondition(arg);
            return View("Index");
        }

        /// <summary>
        /// 新增書籍頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult Create()
        {
            //設定下拉選單資料
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            return View();
        }

        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Create(Models.BookCreateArg arg)
        {
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            try
            {
                // 格式驗證
                if (ModelState.IsValid)
                {
                    //新增書籍
                    bookService.InsertBook(arg);
                    TempData["message"] = Constants.Message.INSERT_SUCCESS;
                    //成功新增便刷新頁面並去掉input value
                    return RedirectToAction("Create", "Book", null);
                }
            }
            catch (Exception ex)
            {
                //目前新增失敗是留在原頁面，不會清掉input值
                TempData["message"] = Constants.Message.INSERT_FAILED;
            }
            return View();
        }

        /// <summary>
        /// 刪除書籍
        /// </summary>
        // TODO: 刪除防呆
        [HttpPost()]
        public JsonResult Delete(Models.BookSearchArg arg)
        {
            var bookData = bookService.GetBooksByCondition(arg)[0];
            try
            {
                if (bookData.BookStatusCode == Constants.BookStatus.IS_BORROWED)
                {
                    throw new Exception("書籍狀態為已借出，故不可刪除");
                }
                else
                {
                    if (bookService.DeleteBookByBookID(arg.BookID))
                    {
                        return Json(new { success = true });
                    }
                    throw new Exception("刪除失敗");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }




        }

        /// <summary>
        /// 修改書籍資訊
        /// </summary>
        /// <param name="bookID"></param>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult Edit(Models.BookSearchArg arg)
        {
            //設定下拉選單資料
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            //借閱人
            ViewBag.UserFullNameSelectList = this.codeService.GetFullNameCodeTable();
            //借閱狀態
            ViewBag.BookStatusSelectList = this.codeService.GetBookStatusCodeTable();

            //預設值帶修改書籍的資料
            var bookData = bookService.GetBooksByCondition(arg)[0];

            return View(bookData);
        }

        /// <summary>
        /// 修改書籍資訊
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Edit(Models.BookEditArg arg)
        {
            //設定下拉選單資料
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            //借閱人
            ViewBag.UserFullNameSelectList = this.codeService.GetFullNameCodeTable();
            //借閱狀態
            ViewBag.BookStatusSelectList = this.codeService.GetBookStatusCodeTable();

            try
            {
                // 格式驗證
                if (ModelState.IsValid)
                {
                    //修改書籍
                    bookService.EditBook(arg);
                    TempData["message"] = Constants.Message.INSERT_SUCCESS;
                }
                return View();                
            }
            catch (Exception ex)
            {
                TempData["message"] = Constants.Message.INSERT_FAILED;
                return View();
            }
        }

        /// <summary>
        /// 書籍明細畫面顯示
        /// </summary>
        [HttpGet()]
        public ActionResult Detail(Models.BookSearchArg arg)
        {
            //設定下拉選單資料
            //圖書類別
            ViewBag.CategorySelectList = this.codeService.GetBookCategoryCodeTable();
            //借閱人
            ViewBag.UserFullNameSelectList = this.codeService.GetFullNameCodeTable();
            //借閱狀態
            ViewBag.BookStatusSelectList = this.codeService.GetBookStatusCodeTable();

            //預設值帶修改書籍的資料
            var bookData = bookService.GetBooksByCondition(arg)[0];
            return View(bookData);
        }

        /// <summary>
        /// 借閱紀錄
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult BorrowedRecord(int bookID)
        {
            ViewBag.BorrowedRecordResult = bookService.GetBorrowedRecordByBookID(bookID);
            return View();
        }
     
    }
}