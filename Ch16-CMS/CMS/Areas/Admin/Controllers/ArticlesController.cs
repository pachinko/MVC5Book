using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using Microsoft.Security.Application;
using PagedList;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace CMS.Areas.Admin.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        ////ArticlesController()
        ////{
        ////    this.ApplicationDbContext = new ApplicationDbContext();
        ////    this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));
        ////}
        ///// <summary>
        ///// Application DB context
        ///// </summary>
        //protected ApplicationDbContext ApplicationDbContext { get; set; }

        ///// <summary>
        ///// User manager - attached to application DB context
        ///// </summary>
        //protected UserManager<ApplicationUser> UserManager { get; set; }

        private CMSDatabaseEntities db = new CMSDatabaseEntities();

        // GET: Admin/Articles
        public ActionResult Index(string q,int page=1,int pageSize=3)
        {
            //this.ApplicationDbContext = new ApplicationDbContext();
            //this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.ApplicationDbContext));

            var model = db.Article.AsQueryable();
            Guid curUserID = Guid.Parse(User.Identity.GetUserId());

            // 我目前只要做出「 current user 只能看到自己貼的行程」
            // 然後要做出「 marketing manager 能看到所有人的行程」。
            bool bContains = User.Identity.Name.Contains("@gmail.com");
            if (bContains)
                //if (User.IsInRole("management") )
            {
                // management sees all.
                model = model.Where(item => item.Hospital.Contains("") );

            }
            else
            {
                model = model.Where(item => item.CreateUser.Equals(curUserID));

            }

            var result = model.OrderBy(d => d.CreateDate).ToPagedList(page, pageSize);
            return View(result);
        }

        // GET: Admin/Articles/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // GET: Admin/Articles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Articles/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, Hospital, VisitTarget, CallNotes,PublishDate,ViewCount,CreateUser,CreateDate,UpdateUser,UpdateDate")] Article article)
//        public ActionResult Create([Bind(Include = "ID,CallNotes,PublishDate,CreateUser,CreateDate,UpdateUser,UpdateDate")] Article article)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    article.ID = Guid.NewGuid();

                    article.Hospital = Sanitizer.GetSafeHtmlFragment(article.Hospital);
                    article.VisitTarget = Sanitizer.GetSafeHtmlFragment(article.VisitTarget);

                    article.VisitDate = DateTime.Now;
                    article.CreateDate = DateTime.Now;
                    article.UpdateDate = DateTime.Now;

                    var thisUser = User;
                    // 目前登入的使用者
                    article.CreateUser = Guid.Parse(User.Identity.GetUserId());
                    article.UpdateUser = Guid.Parse(User.Identity.GetUserId());


                    //過濾XSS
                    article.CallNotes = Sanitizer.GetSafeHtmlFragment(article.CallNotes);
                    db.Article.Add(article);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (Exception excep)
            {
                string str = excep.ToString();
            }

            return View(article);
        }

        // GET: Admin/Articles/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Admin/Articles/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CallNotes,PublishDate,ViewCount,CreateUser,CreateDate,UpdateUser,UpdateDate")] Article article)
        {
            if (ModelState.IsValid)
            {
                //過濾XSS
                article.CallNotes = Sanitizer.GetSafeHtmlFragment(article.CallNotes);

                article.CreateDate = DateTime.Now;
                article.UpdateDate = DateTime.Now;
                //因為還沒做會員所以先給假的
                article.CreateUser = Guid.Empty;
                article.UpdateUser = Guid.Empty;

                db.Entry(article).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Admin/Articles/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Article.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Admin/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Article article = db.Article.Find(id);
            db.Article.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
