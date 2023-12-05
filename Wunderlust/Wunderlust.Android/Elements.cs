using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wunderlust.Droid;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

[assembly: Dependency(typeof(PhotoPickerService))]
[assembly: Dependency(typeof(MultyPhotoPickerService))]
namespace Wunderlust.Droid
{
    public class PhotoPickerService : IPhotoPickerService
    {
        public Task<List<Stream>> GetImageStreamAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.PickImageId);
            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<List<Stream>>();
            // Return Task object
            return MainActivity.Instance.PickImageTaskCompletionSource.Task;
        }
    }
    public class MultyPhotoPickerService : IMultyplePhotoPickerService
    {
        public Task<List<Stream>> GetImageStreamAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            intent.PutExtra(Intent.ExtraAllowMultiple, true);
            // Start the picture-picker activity (resumes in MainActivity.cs)
            MainActivity.Instance.StartActivityForResult(
                Intent.CreateChooser(intent, "Select Picture"),
                MainActivity.MultyplePickImageId);
            // Save the TaskCompletionSource object as a MainActivity property
            MainActivity.Instance.PickImageTaskCompletionSource = new TaskCompletionSource<List<Stream>>();
            // Return Task object
            return MainActivity.Instance.PickImageTaskCompletionSource.Task;
        }
    }
}