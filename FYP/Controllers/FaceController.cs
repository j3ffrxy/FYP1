using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FYP.Models;
//457961bc-15dc-46b4-9717-c21615521646
namespace FYP.Controllers
{
    public class FaceController : Microsoft.AspNetCore.Mvc.Controller
    {
        private IHostingEnvironment _env;
        const string FACE_SUBSCRIPTION_KEY = "d37e4f21659f4c829da6d4b09a1fc24a";
        const string FACE_ENDPOINT = "https://facialrecg.cognitiveservices.azure.com/";

        private const string login = @"SELECT * FROM Users WHERE nric = '{0}'";
        private const string login_face = @"SELECT * FROM Users WHERE Person_Id = '{0}'";
        


        private const string ROLE_COL = "role";
        private const string NAME_COL = "nric";

        private const string REDIRECT_CNTR = "User";
        private const string REDIRECT_ACTN = "Index";

        public FaceController(IHostingEnvironment environment)
        {
            _env = environment;
        }

        public IActionResult Index()
        {

            return View("Index");

        }
        public IActionResult Next()
        {
            return View();
        }
        public IActionResult Fail()
        {
            TempData["Message"] = "There is no face associated with any user in our database. Please try again.";
            TempData["MsgType"] = "warning";
            return View();
        }

        public IActionResult Pass()
        {
            return View();
        }


        public string FaceLogin(IFormFile upimage)
        {
            string fullpath = Path.Combine(_env.WebRootPath, "Logins/user.jpeg");
            using (FileStream fs = new FileStream(fullpath, FileMode.Create))
            {
                upimage.CopyTo(fs);
                fs.Close();
            }

            string imagePath = @"/Logins/user.jpeg";


            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", FACE_SUBSCRIPTION_KEY);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            string requestParameters2 = "returnFaceId=true&returnFaceRectangle=false";
            string uri = FACE_ENDPOINT + "/detect?" + requestParameters;
            string uri2 = FACE_ENDPOINT + "/detect?" + requestParameters2;
            string uriIdentify = FACE_ENDPOINT + "/identify";
            var fileInfo = _env.WebRootFileProvider.GetFileInfo(imagePath);
            var byteData = GetImageAsByteArray(fileInfo.PhysicalPath);
            string contentStringFace = string.Empty;


            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                // Execute the REST API call.
                var response = client.PostAsync(uri2, content).Result;


                // Get the JSON response.
                contentStringFace = response.Content.ReadAsStringAsync().Result;

            }
            string faceId = contentStringFace.Substring(12, 36);
            string str = "{\"personGroupId\": \"supervisor\", \"faceIds\": [\"" + faceId + "\"]}";
            var buffer = System.Text.Encoding.UTF8.GetBytes(str);
            using (ByteArrayContent content2 = new ByteArrayContent(buffer))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".

                content2.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Execute the REST API call.
                var response2 = client.PostAsync(uriIdentify, content2).Result;


                // Get the JSON response.
                contentStringFace = response2.Content.ReadAsStringAsync().Result;

            }


            string personId = contentStringFace;
            TempData["json"] = personId;
            TempData.Keep("json");

            return personId;

        }
        [HttpPost]
        public IActionResult Next(string PersonId)
        {

            if (!AuthenticateUserFace(PersonId, out ClaimsPrincipal principal))
            {

                ViewData["Message"] = "You are not an authorised user. Please try again.";
                ViewData["MsgType"] = "warning";
                return View("Index");

            }
            else
            {
                HttpContext.SignInAsync(
                   CookieAuthenticationDefaults.AuthenticationScheme,
                   principal);
                if (TempData["returnUrl"] != null)
                {
                    string returnUrl = TempData["returnUrl"].ToString();
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                }

                return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);


            }
        }




        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private bool AuthenticateUserFace(string personId, out ClaimsPrincipal principal)
        {
            principal = null;

            DataTable ds = DBUtl.GetTable(login_face, personId);
            if (ds.Rows.Count == 1)
            {
                principal =
                   new ClaimsPrincipal(
                      new ClaimsIdentity(
                         new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier,personId ),
                        new Claim(ClaimTypes.Name, ds.Rows[0][NAME_COL].ToString()),
                        new Claim(ClaimTypes.Role, ds.Rows[0][ROLE_COL].ToString())
                         }, "Basic"
                      )
                   );
                return true;
            }
            return false;
        }

        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            string INDENT_STRING = "    ";
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < json.Length; i++)
            {
                var ch = json[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && json[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}