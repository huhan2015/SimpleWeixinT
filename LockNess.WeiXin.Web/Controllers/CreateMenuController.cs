using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LockNess.WeiXin.Web.Controllers
{
    public class CreateMenuController : Controller
    {
        [HttpGet]
        public ActionResult<string> CreateMenu([FromServices]IHostingEnvironment env)
        {
            FileStream fs1 = new FileStream(Path.Combine(env.WebRootPath, "menu.txt"), FileMode.Open);
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            StreamReader sr = new StreamReader(fs1, Encoding.GetEncoding("GBK"));
            string menu = sr.ReadToEnd();
            sr.Close();
            fs1.Close();

            return GetPage("https://api.weixin.qq.com/cgi-bin/menu/create?access_token=22_Uj1xo0XSKsncW9zeRtRF7vojYq_MHGEh6SljZbHyos_HjPE0w-0v8iZye6TSS93E7xJDrdb75vWHQWymi2vJXNI_PQ_SM2t3HX9NFiH99D59aicOQR24q7VPQPGZ4WwpoLwAJZmjb1wxfZiARHUfABABRE", menu);
        }
        private string GetPage(string url, string menu)
        {
            using (HttpClient client = new HttpClient())
            {
                //if (headers != null)
                //{
                //    foreach (var header in headers)
                //        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                //}
                using (HttpContent httpContent = new StringContent(menu, Encoding.UTF8))
                {
                    //if (contentType != null)
                    //    httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}