using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;

namespace Jobs.Models
{
    public class Job
    {
        public int Id { get; set; }
        [DisplayName("اسم الوظيفة")]
        public String JobTitle { get; set; }
        [DisplayName("وصف الوظيفة")]
        [AllowHtml]
        public String JobContent { get; set; }
        [DisplayName("صورة الوظيفة")]
        public string JobImage { get; set; }
        public string UserId { get; set; }

        public int CategoryId { get; set; }
        public virtual Category category { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}