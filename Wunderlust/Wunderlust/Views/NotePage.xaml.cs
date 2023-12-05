using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Entities;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wunderlust.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotePage : ContentPage
    {
        public static Note Note { get; set; }
        public static User Author { get; set; }
        public static User.UserJsonData AuthorJson { get; set; }
        public static string from;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Author.JsonData != null)
                AuthorJson = JsonConvert.DeserializeObject<User.UserJsonData>(Author.JsonData);
            else
                AuthorJson = new User.UserJsonData { Description = "", ImageUrls = new string[0] };
            if(AuthorJson.ImageUrls.Length>0)
                author_icon.Source = AuthorJson.ImageUrls[0];
            images.HeightRequest = Note.ImageUrls.Length > 0 ? 200 : 0;
            images.ItemsSource = Note.ImageUrls.ToList();
            title.Text = Note.Title;
            date.Text = Note.CreateDate.ToShortDateString();
            text.Text = Note.Content;
            author_name.Text = Author.Surname + " " + Author.Name;
        }
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            await Navigation.PopAsync();
        }
        public NotePage()
        {
            InitializeComponent();
        }
        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//to" + from);
        }

        private async void Profile_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties["authorized"] == "false")
            {
                await DisplayAlert("Предупреждение", "Авторизуйтесь", "OK");
                return;
            }
            if (Author.Id != ((User)(Application.Current.Properties["user"])).Id)
            {
                Application.Current.Properties["another_user"] = Author;
                MessagingCenter.Send<Page>(this, "to_another_profile");
            }
            await Shell.Current.GoToAsync("//toprofile");
        }
    }
}