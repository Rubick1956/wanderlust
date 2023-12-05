using Client.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace Wunderlust
{
    public static class Helps
    {
        public static bool Check(params string[] args)
        {
            foreach (string s in args)
                if (s == "" || s == null)
                    return true;
            return false;
        }
    }
    public interface IPhotoPickerService
    {
        Task<List<Stream>> GetImageStreamAsync();
    }
    public interface IMultyplePhotoPickerService
    {
        Task<List<Stream>> GetImageStreamAsync();
    }
    public class MediaFile
    {
        public string MimeType { get; set; }
        public Stream FileStream { get; set; }
    }
    public class Lenta_item
    {
        public Note Note { get; set; }
        public string Short_content { get; set; }
        public int Size { get; set; }
        public string Isource { get; set; }
        public Lenta_item(Note note)
        {
            Note = note;
            if(Note.Content.Length > 100)
                Short_content = Note.Content.Substring(0, 100) + "...";
            else 
                Short_content = Note.Content;
            if (Note.ImageUrls.Length > 0)
            {
                Isource = Note.ImageUrls[0];
                Size = 200;
            }
            else
                Size = 0;
        }
    };
    public static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}