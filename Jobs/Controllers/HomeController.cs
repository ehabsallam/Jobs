using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Jobs.Models;
using WebApplication2.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Net.Mail;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
       private ApplicationDbContext db= new ApplicationDbContext();
        public ActionResult Index()
        {

            return View(db.Categories.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            Session["JobId"] = id ;
            return View(job);
        }

        [Authorize]
        public ActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apply(string Message )
        {
            var UserId = User.Identity.GetUserId();
            var JobId = (int)Session["JobId"];

            var check = db.ApplyForJobs.Where(a => a.UserId == UserId && a.JobId == JobId).ToList();

            if(check.Count < 1) { 
            var job = new ApplyForJob();
            job.UserId = UserId;
            job.JobId = JobId;
            job.ApplyDate = DateTime.Now;
            job.message = Message;
            db.ApplyForJobs.Add(job);
            db.SaveChanges();
                ViewBag.Result = "Apply sent succesfuly";
            }
            else
            {
                ViewBag.Result = " Alredy Applyed";
            }


            return View();
        }
        [Authorize]
        public ActionResult GetJobByUser()
        {
            var UserId = User.Identity.GetUserId();
            var Job = db.ApplyForJobs.Where(a => a.UserId == UserId);
            return View(Job.ToList());
        }
        [Authorize]
        public ActionResult DetailsOfJob(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            
            return View(job);
        }



        public ActionResult GetJobsByPublisher()
        {
            var UserId = User.Identity.GetUserId();
            var Jobs = from app in db.ApplyForJobs
                       join job in db.Jobs
                       on app.JobId equals job.Id
                       where job.User.Id == UserId
                       select app;

            var grouped = from j in Jobs
                          group j by j.job.JobTitle
                          into gr
                          select new JobsViewModel
                          {
                              JobTitle = gr.Key,
                              Items = gr
                          };
            return View(grouped.ToList());

        }






        public ActionResult Edit(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }


            return View(job);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        public ActionResult Edit(ApplyForJob job)
        {
            if (ModelState.IsValid)
            {
                job.ApplyDate = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetJobByUser");
            }
            return View(job);

        }




        public ActionResult Delete(int id)
        {
            var job = db.ApplyForJobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();

            }
            return View(job);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        public ActionResult Delete(ApplyForJob job)
        {

            // TODO: Add delete logic here
            var myjob = db.ApplyForJobs.Find(job.Id);
            db.ApplyForJobs.Remove(myjob);
            db.SaveChanges();

            return RedirectToAction("GetJobByUser");


        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
         

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactModel contact)
        {
            var mail = new MailMessage();
            //var loginInfo = new NetworkCredential("ehab.r.sallam@gmail.com", "ehab8121996");
            mail.From = new MailAddress(contact.Email);
            mail.To.Add(new MailAddress("ehab.ragab.a@gmail.com"));
            mail.Subject = contact.Subject;
            mail.Body = contact.Message;

            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("ehab.ragab.a@gmail.com", "passs");
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

           // smtpClient.Credentials = loginInfo;
            smtpClient.Send(mail);


            return RedirectToAction("Index");
        }



        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string SearchName)
        {
            var result = db.Jobs.Where(a => a.JobTitle.Contains(SearchName)
            || a.JobContent.Contains(SearchName)
            || a.category.CategoryName.Contains(SearchName)
            || a.category.CategoryDescription.Contains(SearchName)).ToList();
            return View(result);
        }

    }
}