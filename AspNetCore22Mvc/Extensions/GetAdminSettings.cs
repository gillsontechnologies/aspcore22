using AspNetCore22Mvc.Models.CustomModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore22Mvc.Extensions
{

    public static class GetAdminSettings
    {
        public static Admin_Settings GetSettings()
        {
            using (StreamReader r = new StreamReader(@"App_Data/adminSettings.json"))
            {
                string json = r.ReadToEnd();
                var applicationdbcontext = JsonConvert.DeserializeObject<Admin_Settings>(json);
                return applicationdbcontext;
            }
        }
    }

}
