﻿namespace PhotoContest.App.Controllers
{
    using System.Web.Mvc;

    using Data.Contracts;

    public class PicturesController : BaseController
    {
        public PicturesController(IPhotoContestData data)
            : base(data)
        {
        }

        // GET: Pictures/{pictureId}
        // Returned model type: DetailsPictureViewModel
        public ActionResult Index(int id)
        {
            return View();
        }
    }
}