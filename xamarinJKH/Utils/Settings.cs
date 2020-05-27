using System;
using System.Collections.Generic;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.Utils
{
    public static class Settings
    {
        public static MobileSettings MobileSettings = new MobileSettings();
        public static LoginResult Person = new LoginResult();
        public static List<NamedValue> TypeApp = new List<NamedValue>();
        public static HashSet<Page> AppPAge = new HashSet<Page>();
        public static string UpdateKey = "";
        public static string DateUniq = "";
        public static EventBlockData EventBlockData = new EventBlockData();

        public static string[] months =
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь",
            "Декабрь"
        };

        public static AnnouncementInfo getNotif(String id)
        {
            foreach (var each in EventBlockData.Announcements)
            {
                if (each.ID.Equals(id))
                {
                    return each;
                }
            }

            return null;
        }

        public static AdditionalService GetAdditionalService(int id)
        {
            foreach (var each in EventBlockData.AdditionalServices)
            {
                if (each.ID == id)
                {
                    return each;
                }
            }

            return null;
        }

        public static PollInfo GetPollInfo(int id)
        {
            foreach (var each in EventBlockData.Polls)
            {
                if (each.ID == id)
                {
                    return each;
                }
            }

            return null;
        }
    }
}