using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Disly.Areas.Admin.Models
{
    public class EventsViewModel : CoreViewModel
    {
        public EventsList List { get; set; }
        public EventModel Item { get; set; }
    }
}