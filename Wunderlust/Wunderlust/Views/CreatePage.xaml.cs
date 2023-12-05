using Client;
using Client.Entities;
using Client.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Wunderlust.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static System.Net.WebRequestMethods;

namespace Wunderlust.Views
{
    public partial class CreatePage : ContentPage
    {
        private List<Stream> image_stream;
        private List<byte[]> image_bytes = new List<byte[]>();
        public CreatePage()
        {
            InitializeComponent();
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
            create_butt.IsEnabled = false;
            if(Helps.Check(create_title.Text, create_text.Text))
            {
                await DisplayAlert("Предупреждение", "Статья должна содержать заголовок и текст", "OK");
                create_butt.IsEnabled = true;
                return;
            }
            string s;
            string[] images = new string[image_bytes.Count];
            try
            {
                for (int i = 0; i < image_bytes.Count; i++)
                    images[i] = await ImgurHelper.UploadImage(new MemoryStream(image_bytes.ElementAt(i)));
                Note note = new Note
                {
                    CreateDate = DateTime.Now,
                    Title = create_title.Text,
                    Content = create_text.Text,
                    OwnerId = (Guid)(((User)Application.Current.Properties["user"]).Id),
                    ImageUrls = images != null ? images : new string[] { }
                };
                s = await ApiClient.Notes.Create(note, (string)Application.Current.Properties["usertoken"]);
                //Console.WriteLine(s);
            }
            catch (Exception ex)
            {
                if(ex.Message == "Failed to connect to /80.76.32.227:5055")
                    await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                else
                    await DisplayAlert("Предупреждение", ex.Message, "OK");
                create_butt.IsEnabled = true;
                return;
            }
            await Shell.Current.GoToAsync("//tolenta");
            image_view.Children.Clear();
            create_text.Text = "";
            create_title.Text = "";
            image_bytes = new List<byte[]>();
            create_butt.IsEnabled = true;
        }

        private async void Add_Images(object sender, EventArgs e)
        {
            image_stream = await DependencyService.Get<IMultyplePhotoPickerService>().GetImageStreamAsync();
            if (image_stream != null)
            {
                foreach (var image in image_stream)
                {
                    image_bytes.Add(image.ReadAllBytes());
                    image_view.Children.Add(new ImageButton
                    {
                        WidthRequest = 50,
                        HeightRequest = 50,
                        Source = ImageSource.FromStream(() =>
                        {
                            return new MemoryStream(image_bytes.Last());
                        }),
                        Aspect = Aspect.AspectFill
                    });
                    (image_view.Children.Last() as ImageButton).Clicked += Remove_Image;
                }
            }
        }
        private void Remove_Image(object sender, EventArgs e)
        {
            int num = image_view.Children.IndexOf(sender);
            image_bytes.RemoveAt(num);
            image_view.Children.RemoveAt(num);
        }
    }
}