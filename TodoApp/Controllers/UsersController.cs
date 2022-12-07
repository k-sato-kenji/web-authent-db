using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    // アクセス制限
    [Authorize(Roles = "Administrators")]
    public class UsersController : Controller
    {
        private TodoesContext db = new TodoesContext();

        // その通信方法にはざっくり分けると
        // Get（データをもらう）通信と
        // Post（データを送る）通信の二つがあります。


        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            //空のListを設定(追記)
            this.SetRoles(new List<Role>());

            return View();
        }

        // POST: Users/Create
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserName,Password,RoleIds")] User user)
        {
            //  画面のロールの取得
            var roles = db.Roles.Where(role => user.RoleIds.Contains(role.Id)).ToList();
            
            
            if (ModelState.IsValid)
            {
                // 画面で設定されたロールの取得
                user.Roles = roles;

                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // ロールの復元
            this.SetRoles(roles);

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            //userを設定(追記)
            this.SetRoles(user.Roles);

            return View(user);
        }

        // POST: Users/Edit/5
        // 過多ポスティング攻撃を防止するには、バインド先とする特定のプロパティを有効にしてください。
        // 詳細については、https://go.microsoft.com/fwlink/?LinkId=317598 を参照してください。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserName,Password,RoleIds")] User user)
        {

            //  画面のロールの取得
            var roles = db.Roles.Where(role => user.RoleIds.Contains(role.Id)).ToList();


            if (ModelState.IsValid)
            {
                // ＤＢからＩＤを元に取得
                var dbUser = db.Users.Find(user.Id);
                if(dbUser == null)
                {
                    return HttpNotFound();
                }

                // 画面のデータをｄｂ側に反映
                dbUser.UserName = user.UserName;
                dbUser.Password = user.Password;

                //前のロールのクリア
                dbUser.Roles.Clear();
                // 新規登録
                foreach (var role in roles)
                {
                    dbUser.Roles.Add(role);
                }

                db.Entry(dbUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // 選択内容の復元
            this.SetRoles(user.Roles);

            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
        
        private void SetRoles(ICollection<Role> userRoles)
        {
            var roles = userRoles.Select(item => item.Id).ToArray();

            var list = db.Roles.Select(item => new SelectListItem()
            {
                Text = item.RoleName,
                Value = item.Id.ToString(),
                Selected = roles.Contains(item.Id)
            }).ToList();
            // Create.cshtml
            // Edit.cshtml   で使用する。
            // @Html.ListBox("RoleIds", null, new { @class = "form-control" })
            ViewBag.RoleIds = list;

        }

    }
}
