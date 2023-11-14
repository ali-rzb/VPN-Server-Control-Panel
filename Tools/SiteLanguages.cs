using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Tools
{
    public class SiteLanguages
    {
        public static List<Language> AvailableLanguages = new List<Language>()
        {
            new Language {LangFullName = "Persian", LangCultureName = "fa"}
        };

        public static bool IsLanguageAvailable(string lang)
        {
            return AvailableLanguages.FirstOrDefault(l => l.LangCultureName.Equals(lang)) != null;
        }

        public static string GetDefaultLanguage()
        {
            return AvailableLanguages[0].LangCultureName;
        }

        public void SetLanguage(string lang)
        {
            try
            {
                if (!IsLanguageAvailable(lang))
                    lang = GetDefaultLanguage();

                var cultureInfo = new CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
                var langCookie = new HttpCookie("culture", lang) { Expires = DateTime.Now.AddYears(1) };
                HttpContext.Current.Response.Cookies.Add(langCookie);
            }
            catch (Exception)
            {

            }
        }

    }
    public class Language
    {
        public string LangFullName { get; set; }
        public string LangCultureName { get; set; }
    }
}
