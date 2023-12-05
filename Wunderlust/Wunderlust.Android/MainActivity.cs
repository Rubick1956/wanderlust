using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using AndroidX.AppCompat.App;
using Android.Content;  
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Xamarin.Essentials;
namespace Wunderlust.Droid
{
    [Activity(Label = "Wanderlust", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            Window.SetStatusBarColor(Android.Graphics.Color.Argb(1, 128, 158, 89));
            Instance = this;
        }
        public static readonly int PickImageId = 1000;
        public static readonly int MultyplePickImageId = 1001;
        public TaskCompletionSource<List<Stream>> PickImageTaskCompletionSource { set; get; }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    Android.Net.Uri uri = intent.Data;
                    Stream stream = ContentResolver.OpenInputStream(uri);
                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(new List<Stream>() { stream });
                }
                else
                    PickImageTaskCompletionSource.SetResult(null);
            }
            else if (requestCode == MultyplePickImageId)
            {
                if ((resultCode == Result.Ok) && (intent != null))
                {
                    ClipData clipData = intent.ClipData;
                    List<Stream> streams = new List<Stream>();
                    Android.Net.Uri uri;
                    if (clipData != null && clipData.ItemCount > 0)
                    {
                        for (int i = 0; i < clipData.ItemCount; i++)
                        {
                            uri = intent.ClipData.GetItemAt(i).Uri;
                            streams.Add(ContentResolver.OpenInputStream(uri));
                        }
                    }
                    else
                    {
                        uri = intent.Data;
                        streams.Add (ContentResolver.OpenInputStream(uri));
                    }
                    // Set the Stream as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(streams);
                }
                else
                    PickImageTaskCompletionSource.SetResult(null);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}