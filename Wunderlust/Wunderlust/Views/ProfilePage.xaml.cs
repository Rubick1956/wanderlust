using Client;
using Client.Entities;
using Client.Helpers;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Wunderlust.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
namespace Wunderlust.Views
{
    public partial class ProfilePage : ContentPage
    {
        private List<Stream> image_stream;
        private byte[] image_bytes;
        private User User { get; set; }
        private User.UserJsonData UserJsonData { get; set; }
        private ImageSource image_backup;
        private bool from_galery = false;
        public ProfilePage()
        {
            InitializeComponent();
            lenta.Refreshing += Refresh;
        }
        private void Refresh(object sender, EventArgs e)
        {
            MessagingCenter.Send<Page>(this, "update_profile_lenta_default");
            (sender as ListView).IsRefreshing = false;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(!from_galery)
                Set_Profile();
            from_galery = false;
        }
        private void Set_Profile()
        {
            if ((string)Application.Current.Properties["authorized"] == "true")
            {
                if ((bool)Application.Current.Properties["my_profile"])
                {
                    User = (User)Application.Current.Properties["user"];
                    edit_butt.IsVisible = true;
                }
                else
                {
                    User = (User)Application.Current.Properties["another_user"];
                    edit_butt.IsVisible = false;
                }
                if (User.JsonData == null)
                    UserJsonData = new User.UserJsonData { Description = "", ImageUrls = new string[] { } };
                else
                    UserJsonData = JsonConvert.DeserializeObject<User.UserJsonData>(User.JsonData);
                if (UserJsonData.ImageUrls.Length > 0)
                    profile_icon.Source = UserJsonData.ImageUrls[0];
                else
                    profile_icon.Source = "default_icon";
                name.Text = User.Name + ' ' + User.Surname;
                profile_info.Text = UserJsonData.Description;
                edit_profile_info.Text = UserJsonData.Description;
                edit_fname.Text = User.Name;
                edit_sname.Text = User.Surname;
                edit_login.Text = User.Login;
                edit_email.Text = User.Email;
                edit_phone.Text = User.Phone;
                Title = "Профиль";
                edit_butt.Text = "Редактировать";
                edit.IsEnabled = false;
                edit.IsVisible = false;
                lenta.IsEnabled = true;
                lenta.IsVisible = true;
                profile_icon.IsEnabled = false;
            }
            else
            {
                profile_icon.Source = "default_icon";
                name.Text = "";
                profile_info.Text = ""; 
                Title = "Профиль";
                edit_butt.Text = "Редактировать";
                edit_butt.IsVisible = false;
                edit.IsEnabled = false;
                edit.IsVisible = false;
                lenta.IsEnabled = false;
                lenta.IsVisible = false;
                profile_icon.IsEnabled = false;
            }
            pass.Text = "";
        }
        private void Edit_Profile(object sender, EventArgs e)
        {
            if (edit_butt.Text == "Редактировать")
            {
                edit.ScrollToAsync(0, 0, false);
                Title = "Редактировать профиль";
                edit_butt.Text = "Отменить";
                lenta.IsEnabled = false;
                lenta.IsVisible = false;
                edit.IsEnabled = true;
                edit.IsVisible = true;
                profile_icon.IsEnabled = true;
            }
            else
            {
                Set_Profile();
                Title = "Профиль";
                edit_butt.Text = "Редактировать";
                edit.IsEnabled = false;
                edit.IsVisible = false;
                lenta.IsEnabled = true;
                lenta.IsVisible = true;
                profile_icon.IsEnabled = false;
                if (image_backup != null)
                    profile_icon.Source = image_backup;
                image_backup = null;
            }
        }
        private async void Save(object sender, EventArgs e)
        {
            save_butt.IsEnabled = false;
            if (Helps.Check(edit_login.Text, edit_fname.Text, edit_sname.Text, edit_email.Text, edit_phone.Text))
            {
                await DisplayAlert("Предупреждение", "Заполните обязательные поля", "OK");
                save_butt.IsEnabled = true;
                return;
            }
            string token = "", imageUrl = "";
            try
            {
                token = await ApiClient.Auth.Authorize(User.Login, pass.Text);
                if (image_bytes != null)
                {
                    imageUrl = await ImgurHelper.UploadImage(new MemoryStream(image_bytes));
                    if (UserJsonData == null)
                        UserJsonData = new User.UserJsonData { Description = "", ImageUrls = new string[0] };
                    if (UserJsonData.ImageUrls.Length < 1)
                        UserJsonData.ImageUrls = new string[1];
                    UserJsonData.ImageUrls[0] = imageUrl;
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                if (ex.Message == "BadRequest")
                    await DisplayAlert("Предупреждение", "Неверный пароль", "OK");
                else
                    await DisplayAlert("Предупреждение", ex.Message, "OK");
                save_butt.IsEnabled = true;
                return;
            }
            catch (System.Net.WebException)
            {
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                save_butt.IsEnabled = true;
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                save_butt.IsEnabled = true;
                return;
            }
            User.Login = edit_login.Text;
            User.Email = edit_email.Text;
            User.Name = edit_fname.Text;
            User.Phone = edit_phone.Text;
            User.Surname = edit_sname.Text;
            User.Password = pass.Text;
            User.JsonData = JsonConvert.SerializeObject(new User.UserJsonData
            {
                Description = edit_profile_info.Text,
                ImageUrls = UserJsonData.ImageUrls.Length > 0 ? new string[] { UserJsonData.ImageUrls[0] } : new string[] { }
            });
            try
            {
                await ApiClient.Users.Update(User, (string)Application.Current.Properties["usertoken"]);
            }
            //catch (System.Net.Http.HttpRequestException ex){ }
            catch (System.Net.WebException)
            {
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                save_butt.IsEnabled = true;
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                save_butt.IsEnabled = true;
                return;
            }
            Application.Current.Properties["user"] = User;
            Set_Profile();
            Title = "Профиль";
            edit_butt.Text = "Редактировать";
            edit.IsEnabled = false;
            edit.IsVisible = false;
            lenta.IsEnabled = true;
            lenta.IsVisible = true;
            save_butt.IsEnabled = true;
            image_backup = null;
        }
        private async void Set_Image(object sender, EventArgs e)
        {
            image_stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (image_stream != null)
            {
                image_backup = (sender as ImageButton).Source;
                image_bytes = image_stream.ElementAt(0).ReadAllBytes();
                (sender as ImageButton).Source = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(image_bytes);
                });
            }
            from_galery = true;
        }
        private async void Logout(object sender, EventArgs e)
        {
            Application.Current.Properties["authorized"] = "logouted";
            Application.Current.Properties["usertoken"] = "";
            Application.Current.Properties["user"] = new User();
            await Navigation.PushAsync(new LoginPage());
            Edit_Profile(sender, e);
            Preferences.Remove("login");
            Preferences.Remove("token");
        }
        private async void Delete_Profile(object sender, EventArgs e)
        {
            string token = "";
            try
            {
                token = await ApiClient.Auth.Authorize(User.Login, pass.Text);
                await ApiClient.Users.Delete((Guid)((User)(Application.Current.Properties["user"])).Id, (string)Application.Current.Properties["usertoken"]);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                if (ex.Message == "BadRequest")
                    await DisplayAlert("Предупреждение", "Неверный пароль", "OK");
                else
                    await DisplayAlert("Предупреждение", ex.Message, "OK");
                return;
            }
            catch (System.Net.WebException)
            {
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                return;
            }
            Logout(sender, e);
            Preferences.Remove("login");
            Preferences.Remove("token");
        }
        private async void note_selected(object sender, ItemTappedEventArgs e)
        {
            User user;
            try
            {
                user = await ApiClient.Users.GetUserById(ProfileViewModel.notes[e.ItemIndex].OwnerId, (string)Application.Current.Properties["usertoken"]);
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                return;
            }
            NotePage.Note = ProfileViewModel.notes[e.ItemIndex];
            NotePage.Author = user;
            NotePage.from = "profile";
            await Shell.Current.GoToAsync("tonote");
        }
    }
}