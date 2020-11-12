using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Common.Logging;

namespace indigoCardIssuingWeb
{
    /// <summary>
    /// Summary description for LogoHandler
    /// </summary>
    public class LogoHandler : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogoHandler));
        private static byte[] imageBytes = null;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpeg";

                Stream strm = DisplayImage();
            
            byte[] buffer = new byte[2048];
            int byteSeq = strm.Read(buffer, 0, 2048);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 2048);
            }
        }

        public Stream DisplayImage()
        {
            try
            {
                imageBytes = null;

                if (imageBytes == null)
                {
                    string logo = ConfigurationManager.AppSettings["TitleLogoLocation"].ToString();

                    if (String.IsNullOrWhiteSpace(logo))
                        log.Warn("Logo location not set. Please add logo location to settings.");
                    else
                    {
                        System.IO.FileInfo logoFile = new System.IO.FileInfo(logo);

                        if (logoFile.Exists)
                        {
                            imageBytes = System.IO.File.ReadAllBytes(logoFile.FullName);
                        }
                        else
                            log.Warn("Logo file does exist at location:\t" + logo);
                    }
                }
                
            }
            catch (Exception ex)
            {
                log.Warn("Problem loading title logo", ex);                
            }
            finally
            {
                if (imageBytes == null)
                    imageBytes = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("~/images") + "/veneka_logo2.png");
            }

            return new MemoryStream(imageBytes);
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}