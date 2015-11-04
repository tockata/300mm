﻿namespace PhotoContest.App.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper.QueryableExtensions;

    using Common;

    using Data.Contracts;

    using Microsoft.AspNet.Identity;

    using Models.Notification;

    using PagedList;

    [Authorize]
    public class NotificationsController : BaseController
    {
        public NotificationsController(IPhotoContestData data)
            : base(data)
        {
        }

        public ActionResult Index(int? page)
        {
            var userId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(userId);

            var notifications = user.Notifications
                .AsQueryable()
                .OrderByDescending(n => n.CreatedOn)
                .ProjectTo<NotificationViewModel>()
                .ToPagedList(page ?? GlobalConstants.DefaultStartPage, GlobalConstants.DefaultPageSize);


            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int id)
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            var notification = this.Data.Notifications.Find(id);
            string loggedUserId = this.User.Identity.GetUserId();

            if (notification == null)
            {
                throw new ArgumentException("Notification not found!");
            }

            if (loggedUserId != notification.RecipientId)
            {
                throw new ArgumentException("You are not the owner of this notification!");
            }

            notification.IsRead = true;
            this.Data.SaveChanges();

            return Content(string.Empty);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAllAsRead()
        {
            if (!this.Request.IsAjaxRequest())
            {
                throw new InvalidOperationException("Invalid operation!");
            }

            var loggedUserId = this.User.Identity.GetUserId();

            this.Data.Notifications.All()
                .Where(n => n.RecipientId == loggedUserId && n.IsRead == false)
                .ToList().ForEach(n => n.IsRead = true);

            this.Data.SaveChanges();

            return Content(string.Empty);
        }
    }
}