using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AiForms.Dialogs;
using AiForms.Dialogs.Abstractions;
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
        
        public static bool IsFirsStart = true;
        public static EventBlockData EventBlockData = new EventBlockData();

        public static string[] months =
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь", "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь",
            "Декабрь"
        };

        public static bool? isSelf = null;

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
        public static async Task StartProgressBar(string title = "Загрузка", double opacity = 0.6)
        {
            // Loading settings
            Configurations.LoadingConfig = new LoadingConfig {
                IndicatorColor = Color.FromHex(MobileSettings.color),
                OverlayColor = Color.Black,
                Opacity = opacity,
                DefaultMessage = title,
            };

            await Loading.Instance.StartAsync(async progress =>{
                // some heavy process.
                for (var i = 0; i < 100; i++)
                {
                    await Task.Delay(70);
                    // can send progress to the dialog with the IProgress.
                    // progress.Report((i + 1) * 0.01d);
                }
            });
        }
    }
}