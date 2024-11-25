using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DailyTaskMaker.Infrastructure.CommonUtilityClasses
{
    public static class UserUtility
    {
        

       
        public static string ? GetUserEmail(IHttpContextAccessor httpContextAccessor)
        {
            var emailClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "preferred_username");
            return emailClaim?.Value;

        }

        public static string ConvertToXml(object obj)
        {
            Type type = obj.GetType();
            XmlSerializer serializer = new XmlSerializer(type);
            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }
    }
}
