﻿//  Copyright 2011 U.S. Department of Defense

//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at

//      http://www.apache.org/licenses/LICENSE-2.0

//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.


using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using vwarDAL;
using System.Collections.Generic;
using System.IO;
using System.Drawing;


namespace Website
{
    /// <summary>
    /// Summary description for Common
    /// </summary>
    public static class Common
    {
        const int MAX_CHARS_PER_LINE = 27;
        const int MAX_TOTAL_CHARS = 50;

        public static string GetFullUserName(object userName)
        {
            if (userName != null)
            {
                var user = Membership.GetUser(userName.ToString());
                if (user != null && !String.IsNullOrEmpty(user.Comment))
                {
                    return user.Comment.Replace('|', ' ');
                }
                return userName.ToString();
            }
            return "";
        }
        //Formats the 
        public static string FormatDescription(string desc)
        {
            string newval = (desc.Length > 50) ? desc.Substring(0, 50) + "..." : desc;
            for (int i = 0; i < newval.Length; i++)
            {
                if (i % MAX_CHARS_PER_LINE == 0)
                {
                    newval.Insert(i, "\n");
                }
            }
            return newval;
        }
        //formats URL of screenshot image stored at ~/content/{id}/{screenshot}
        public static string FormatScreenshotImage(object contentObjectID, object screenshot)
        {
            return String.Format("~/Public/Model.ashx?pid={0}&file={1}", contentObjectID, screenshot);
        }
        public static string FormatEditUrl(object contentObjectID)
        {
            string rv = "";
            if (contentObjectID != null)
            {
                rv = "~/Users/Upload.aspx?ContentObjectID=" + contentObjectID.ToString();
            }
            return rv;
        }
        //returns full path of zip file located at ~/content/{id}/{location}
        public static string FormatZipFilePath(object contentObjectID, object location)
        {
            string rv = "";
            if (contentObjectID != null && location != null)
            {
                string virtualPath = "~/Content/" + contentObjectID + "/" + location;
                rv = HttpContext.Current.Server.MapPath(virtualPath);
            }
            return rv;
        }

        //bind to ImageUrl of screenshot image - stored at ~/styles/images/SubmitterLogos/{id}.{gif/jpg/jpeg/png}
        public static string FormatSubmitterLogoImage(object contentObjectID)
        {
            string rv = "";
            if (contentObjectID != null)
            {
                //try to create path to logo image - return "" if not found
                rv = "~/styles/images/SubmitterLogos/" + contentObjectID + ".jpg";
                if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(rv)))
                {
                    rv = "~/styles/images/SubmitterLogos/" + contentObjectID + ".jpeg";
                    if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(rv)))
                    {
                        rv = "~/styles/images/SubmitterLogos/" + contentObjectID + ".png";
                        if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(rv)))
                        {
                            rv = "~/styles/images/SubmitterLogos/" + contentObjectID + ".gif";
                            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath(rv)))
                            {
                                rv = "";
                            }
                        }
                    }
                }
            }
            return rv;
        }
        public static int CalculateAverageRating(object r)
        {

            /*var Session = HttpContext.Current.Session;
            if (Session["DAL"] == null)
            {
                var factory = new DataAccessFactory();
                Session["DAL"] = factory.CreateDataRepositorProxy();
            }
            vwarDAL.IDataRepository dal = Session["DAL"] as IDataRepository;
            var co = dal.GetContentObjectById(contentObjectId.ToString(), false,true);*/
            IEnumerable<Review> reviews = (IEnumerable<Review>)r;
            int rating = 0;
            foreach (var review in reviews)
            {
                rating += review.Rating;
            }
            if (reviews.Count() > 0)
            {
                return rating / reviews.Count();
            }
            return rating;
        }
        //bind to visible property of logo image
        public static Boolean ShowSubmitterLogoImage(object contentObjectID)
        {
            Boolean rv = false;
            if (FormatSubmitterLogoImage(contentObjectID) != "")
            {
                rv = true;
            }
            return rv;
        }

        public static byte[] GetByteArrayFromFileUpload(FileUpload fu)
        {
            byte[] rv = null;

            if (fu.PostedFile != null && !string.IsNullOrEmpty(fu.PostedFile.FileName))
            {
                int imageLength = fu.PostedFile.ContentLength;
                rv = new byte[imageLength];
                fu.PostedFile.InputStream.Read(rv, 0, imageLength);
            }

            return rv;
        }

        public static bool IsValidLogoImageContentType(string contentType)
        {
            bool rv = false;

            if (!string.IsNullOrEmpty(contentType))
            {

                switch (contentType.ToLower().Trim())
                {
                    case "image/gif":
                    case "image/jpeg":
                    case "image/bmp":
                    case "image/png":
                        rv = true;
                        break;
                }

            }

            return rv;
        }

        public static bool ImageConvertAbortCallback()
        {
            return false;
        }
        public static Stream GenerateThumbnail(System.IO.Stream s, System.Drawing.Imaging.ImageFormat format)
        {

            MemoryStream ms = new MemoryStream();
            new Bitmap(s).GetThumbnailImage(Int32.Parse(ConfigurationManager.AppSettings["ThumbnailImage_Width"]),
                                            Int32.Parse(ConfigurationManager.AppSettings["ThumbnailImage_Height"]),
                                            ImageConvertAbortCallback,
                                            System.IntPtr.Zero)
                         .Save(ms, format);

            return ms;

        }
   

    //write the json file to the response;
    public static void WriteJSONtoResponse(Stream stream, HttpContext context)
    {
        HttpResponse _response = context.Response;
        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, (int)stream.Length);
        Utility_3D _3d = new Utility_3D();
        _3d.Initialize(Website.Config.ConversionLibarayLocation);
        Utility_3D.Model_Packager pack = new Utility_3D.Model_Packager();
        Utility_3D.ConvertedModel model = pack.Convert(new MemoryStream(buffer), "ThisShouldAlwaysBeZip.zip", "json");
        Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(model.data);
        _response.ContentType = "application/octet-stream";
        foreach (Ionic.Zip.ZipEntry ze in zip)
        {
            if (Path.GetExtension(ze.FileName) == ".json")
            {
                MemoryStream mem = new MemoryStream();
                ze.Extract(mem);
                byte[] jsonbuffer = new byte[mem.Length];
                mem.Seek(0, SeekOrigin.Begin);
                mem.Read(jsonbuffer, 0, (int)mem.Length);
                _response.BinaryWrite(jsonbuffer);
                return;
            }
        }
    }
    public static void WriteContentFileToResponse(string Pid, string filename, HttpContext context, vwarDAL.IDataRepository vd)
    {
        Stream data = null;//= vd.GetContentFile(Pid, co.DisplayFileId);
        if (data == null) data = vd.GetContentFile(Pid, filename);
        context.Response.ContentType = "application/octet-stream";

        byte[] buffer = new byte[data.Length];
        data.Seek(0, SeekOrigin.Begin);
        data.Read(buffer, 0, (int)data.Length);
        context.Response.BinaryWrite(buffer);
    }
    public static void WriteLocalFileToResponse(string filename, HttpContext context)
    {

        Stream data = new FileStream(filename, FileMode.Open);
        context.Response.ContentType = "application/octet-stream";
        byte[] buffer = new byte[data.Length];
        data.Seek(0, SeekOrigin.Begin);
        data.Read(buffer, 0, (int)data.Length);
        context.Response.BinaryWrite(buffer);
        data.Close();
    }
    public static string GetPidFromURL(HttpContext context)
    {
        //Get the PID for the request
        string Pid = context.Request.QueryString["pid"];
        if (Pid == null)
            Pid = context.Request.QueryString["ContentObjectID"];
        return Pid;
    }
    }
}
