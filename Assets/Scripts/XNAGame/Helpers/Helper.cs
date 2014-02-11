using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
//using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


namespace PushBlock.Helpers
{
    static class Helper
    {
        public static   Game Game { get; set; }
        
        public static ContentManager Content { get; set; }
             

        public static Texture2D GetGrayscaleTexture(Texture2D sourceTexture)
        {
            UnityEngine.Color[] sourcePixels = sourceTexture.UnityTexture.GetPixels();

            UnityEngine.Color[] grayscalePixels = new UnityEngine.Color[sourceTexture.Width * sourceTexture.Height];

            for (int i = 0; i < sourcePixels.Length; i++)
            {
                float grayscale = 
                    (sourcePixels[i].r * 0.3f 
                    + sourcePixels[i].g * 0.59f 
                    + sourcePixels[i].b * 0.11f);

                grayscalePixels[i].r = grayscale;
                grayscalePixels[i].g = grayscale;
                grayscalePixels[i].b = grayscale;
                grayscalePixels[i].a = sourcePixels[i].a;
            }

            UnityEngine.Texture2D grayscaleUnityTexture = new UnityEngine.Texture2D(
                sourceTexture.Width, 
                sourceTexture.Height,
                sourceTexture.UnityTexture.format,
                false);

            grayscaleUnityTexture.SetPixels(grayscalePixels);
            grayscaleUnityTexture.Apply();
            Texture2D grayscaleTexture = new Texture2D(grayscaleUnityTexture);
            
            return grayscaleTexture;
        }

        public static Texture2D GetHorizontallyFlippedTexture(Texture2D sourceTexture)
        {
            UnityEngine.Color[] sourcePixels = sourceTexture.UnityTexture.GetPixels();
            UnityEngine.Color[] flippedPixels = new UnityEngine.Color[sourceTexture.Width * sourceTexture.Height];

            for (int n = 0; n < sourcePixels.Length; n++)
            {
                int mod = n % sourceTexture.Width;
                int x = ((sourceTexture.Width - mod - 1) + n) - mod;
                flippedPixels[n] = sourcePixels[x];
            }
            

//            Color[] flippedPixels = new Color[sourceTexture.Width * sourceTexture.Height];

//            for (int x = 0; x < sourceTexture.Height; x++)
//            {
//                for (int y = 0; y < sourceTexture.Width; y++)
//                {
//                    flippedPixels[x * sourceTexture.Width + (sourceTexture.Width - 1) - y] = sourcePixels[x * sourceTexture.Width + y];
//                }
//            }

            UnityEngine.Texture2D flippedUnityTexture = new UnityEngine.Texture2D(
                sourceTexture.Width,
                sourceTexture.Height,
                sourceTexture.UnityTexture.format,
                false);

            flippedUnityTexture.SetPixels(flippedPixels);
            flippedUnityTexture.Apply();
            Texture2D flippedTexture = new Texture2D(flippedUnityTexture);
           
            return flippedTexture;
        }

        public static uint ColorToUInt(UnityEngine.Color32 color)
        {
            return (uint)((color.a << 24) | (color.r << 16) |
                          (color.g << 8) | (color.b << 0));
        }

        public static uint[] ReverseUnityTextureDataToUVOrder(uint[] sourceArray, int width)
        {
            var flippedArray = new uint[sourceArray.Length];
            
            for (int i = 0, j = sourceArray.Length - width; i < sourceArray.Length; i += width, j -= width) 
            {
                for (int k = 0; k < width; ++k) 
                {
                    flippedArray[i + k] = sourceArray[j + k];
                }
            }

            return flippedArray;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public static void SendEmail(string from, string to, string subject, string body)
        {
            //try
            //{
            //    MailMessage mail = new MailMessage();
            //    mail.From = new MailAddress(from);
            //    mail.To.Add(new MailAddress(to));
            //    mail.Body = body;
            //    mail.Subject = subject;

            //    ServicePointManager.ServerCertificateValidationCallback =
            //        delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //        { return true; };

            //    SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            //    smtp.EnableSsl = true;
            //    string login = "shamihulau@gmail.com";
            //    string password = "Airema23Airema";
            //    NetworkCredential mailAuthentication = new NetworkCredential(login, password);
            //    smtp.Credentials = (System.Net.ICredentialsByHost)mailAuthentication;
            //    smtp.SendAsync(mail, null);
            //}
            //catch (Exception e)
            //{
            //    UnityEngine.Debug.Log(e);
            //}
        }  

      
    }
}
