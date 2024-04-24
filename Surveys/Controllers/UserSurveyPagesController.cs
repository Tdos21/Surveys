using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Surveys.Models;

namespace Surveys.Controllers
{
    public class UserSurveyPagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly ApplicationDbContext _context;

        public UserSurveyPagesController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: UserSurveyPages
        [Authorize]
        public ActionResult Index()
        {
            return View(db.UserSurveyPages.ToList());
        }

        public ActionResult Stats()
        {
            var userSurveyPages = _context.UserSurveyPages.ToList();
            if (userSurveyPages.Any())
            {
                // Calculate total submissions
                int totalSubmissions = userSurveyPages.Count();
                ViewBag.TotalSubmissions = totalSubmissions;

                // Calculate average age
                double totalAge = 0;
                DateTime oldestDateOfBirth = DateTime.MinValue;
                DateTime youngestDateOfBirth = DateTime.MaxValue;

                foreach (var userSurveyPage in userSurveyPages)
                {
                    totalAge += CalculateAge(userSurveyPage.DateOfBirth);

                    // Update oldestDateOfBirth if necessary
                    if (userSurveyPage.DateOfBirth > oldestDateOfBirth)
                    {
                        oldestDateOfBirth = userSurveyPage.DateOfBirth;
                    }

                    if (userSurveyPage.DateOfBirth < youngestDateOfBirth)
                    {
                        youngestDateOfBirth = userSurveyPage.DateOfBirth;
                    }
                }

                double averageAge = totalAge / totalSubmissions;
                double roundedAverageAge = Math.Round(averageAge, 2);
                ViewBag.AverageAge = roundedAverageAge;




                // Calculate age of the oldest person
                int oldestAge = CalculateAge(oldestDateOfBirth);
                ViewBag.OldestAge = oldestAge;
                //ViewBag.OldestPersonName = _context.UserSurveyPages
                //                                    .FirstOrDefault(u => u.DateOfBirth == oldestDateOfBirth)
                //                                    ?.Fullname;

                int youngestAge = CalculateAge(youngestDateOfBirth);
                ViewBag.YoungestAge = youngestAge;
                //ViewBag.YoungestPersonName = _context.UserSurveyPages
                //                                    .FirstOrDefault(u => u.DateOfBirth == youngestDateOfBirth)
                //                                    ?.Fullname;



                // Calculate percentage of people who checked Pizza
                int pizzaLoversCount = userSurveyPages.Count(u => u.Pizza);
                double pizzaLoversPercentage = (double)pizzaLoversCount / totalSubmissions * 100;
                double roundedPizzaPercentage = Math.Round(pizzaLoversPercentage, 2);
                ViewBag.PizzaLoversPercentage = roundedPizzaPercentage;


                // Calculate percentage of people who checked Pasta
                int pastaLoversCount = userSurveyPages.Count(u => u.Pasta);
                double pastaLoversPercentage = (double)pastaLoversCount / totalSubmissions * 100;
                double roundedPercentage = Math.Round(pastaLoversPercentage, 2);
                ViewBag.PastaLoversPercentage = roundedPercentage;

                //Pap&Wors
                int papAndWorsLoversCount = userSurveyPages.Count(u => u.PapAndWors);
                double papAndWorsLoversPercentage = (double)papAndWorsLoversCount / totalSubmissions * 100;
                double roundedPapAndWorsPercentage = Math.Round(papAndWorsLoversPercentage, 2);
                ViewBag.PapAndWorsLoversPercentage = roundedPapAndWorsPercentage;


                // Create a dictionary to store the most rated ratings for each activity
                var mostRatedRatings = new Dictionary<string, string>();

                // Define the list of activities
                var activities = new List<string> { "Average ratings of Watching TV", "Average ratings of Listening to Radio", "Average ratings of Eating Out", "Average ratings of Watching Movies" };

                foreach (var activity in activities)
                {
                    // Filter survey responses for the current activity
                    var activitySurveyPages = userSurveyPages.Where(u => GetActivityRating(u, activity) != 0).ToList();

                    if (activitySurveyPages.Any())
                    {
                        // Group responses by rating and find the most rated rating
                        var mostRatedRating = activitySurveyPages.GroupBy(u => GetActivityRating(u, activity))
                            .Select(g => new { Rating = g.Key, Count = g.Count() })
                            .OrderByDescending(g => g.Count)
                            .FirstOrDefault();

                        // Store the most rated rating in the dictionary
                        mostRatedRatings.Add(activity, mostRatedRating != null ? GetRatingName(mostRatedRating.Rating) : "No data");
                    }
                    else
                    {
                        // If there are no responses for the activity, set "No data"
                        mostRatedRatings.Add(activity, "No data");
                    }
                }

                // Pass the dictionary to the view
                return View(mostRatedRatings);


            }
            else
            {
                ViewBag.NoStatsAvailable = true;
            }

            return View();
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--; 
            }
            return age;
        }

        private string GetRatingName(int rating)
        {
            switch (rating)
            {
                case 5:
                    return "Strongly Agree";
                case 4:
                    return "Agree";
                case 3:
                    return "Neutral";
                case 2:
                    return "Disagree";
                case 1:
                    return "Strongly Disagree";
                default:
                    return "Unknown";
            }
        }

        private int GetActivityRating(UserSurveyPage userSurveyPage, string activity)
        {
            switch (activity)
            {
                case "Average ratings of Watching TV":
                    return GetRatingValue(userSurveyPage.WatchTVRating);
                case "Average ratings of Listening to Radio":
                    return GetRatingValue(userSurveyPage.ListenToRadioRating);
                case "Average ratings of Eating Out":
                    return GetRatingValue(userSurveyPage.EatOutRating);
                case "Average ratings of Watching Movies":
                    return GetRatingValue(userSurveyPage.WatchMoviesRating);
                default:
                    return 0; // If the activity is not recognized
            }
        }

        private int GetRatingValue(string rating)
        {
            switch (rating)
            {
                case "Strongly Agree":
                    return 5;
                case "Agree":
                    return 4;
                case "Neutral":
                    return 3;
                case "Disagree":
                    return 2;
                case "Strongly Disagree":
                    return 1;
                default:
                    return 0; // If the rating is not recognized
            }
        }


        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    UserSurveyPage userSurveyPage = db.UserSurveyPages.Find(id);
        //    if (userSurveyPage == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(userSurveyPage);
        //}

        // GET: UserSurveyPages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserSurveyPage/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserSurveyPage userSurveyPage)
        {
            if (ModelState.IsValid)
            {
                // Save the user survey data to the database
                db.UserSurveyPages.Add(userSurveyPage);
                db.SaveChanges();

                // Redirect to a thank you page or any other page as needed
                return RedirectToAction("ThankYou");
            }

            // If the model state is not valid, return to the create view with errors
            return View(userSurveyPage);
        }

        // GET: UserSurveyPage/ThankYou
        public ActionResult ThankYou()
        {
            return View();
        }
        // GET: UserSurveyPages/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    UserSurveyPage userSurveyPage = db.UserSurveyPages.Find(id);
        //    if (userSurveyPage == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(userSurveyPage);
        //}

        //// POST: UserSurveyPages/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UserId,Fullname,Email,DateOfBirth,Contact,Pizza,Pasta,PapAndWors,Other,Activity,Rating")] UserSurveyPage userSurveyPage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(userSurveyPage).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(userSurveyPage);
        //}

        // GET: UserSurveyPages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserSurveyPage userSurveyPage = db.UserSurveyPages.Find(id);
            if (userSurveyPage == null)
            {
                return HttpNotFound();
            }
            return View(userSurveyPage);
        }

        // POST: UserSurveyPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserSurveyPage userSurveyPage = db.UserSurveyPages.Find(id);
            db.UserSurveyPages.Remove(userSurveyPage);
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
