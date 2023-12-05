using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wunderlust.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Client;
using Client.Entities;
using System.IO;
using Newtonsoft.Json;
using Client.Helpers;
using System.Runtime.InteropServices.ComTypes;
using Xamarin.Essentials;
namespace Wunderlust.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RegisterPage : ContentPage
	{
        private List<Stream> image_stream;
        private byte[] image_bytes;
		public RegisterPage ()
		{
			InitializeComponent ();
            BindingContext = new RegisterViewModel();
        }
        private async void Back_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
        private async void Set_Image(object sender, EventArgs e)
        {
            image_stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (image_stream != null)
            {
                image_bytes = image_stream.ElementAt(0).ReadAllBytes();
                (sender as ImageButton).Source = ImageSource.FromStream(() =>
                {
                    return new MemoryStream(image_bytes);
                });
            }
        }
        private async void Register_Clicked(object sender, EventArgs e)
        {
            if (Helps.Check(fname_entry.Text, sname_entry.Text, login_entry.Text, email_entry.Text, phone_entry.Text, pass_entry.Text, pass_confirm.Text))
            {
                await DisplayAlert("Предупреждение", "Введите данные в обязательные поля", "OK");
                return;
            }
            if (pass_entry.Text == pass_confirm.Text)
            {
                register_butt.IsEnabled = false;
                string imageUrl = "";
                User user;
                try
                {
                    if(image_bytes != null)
                        imageUrl = await ImgurHelper.UploadImage(new MemoryStream(image_bytes));
                    user = new User
                    {
                        Name = fname_entry.Text,
                        Surname = sname_entry.Text,
                        Login = login_entry.Text,
                        Email = email_entry.Text,
                        Phone = phone_entry.Text,
                        Password = pass_entry.Text,
                        Role = "User",
                        JsonData = JsonConvert.SerializeObject(new User.UserJsonData { Description = profile_info_entry.Text, ImageUrls =
                        image_bytes != null ? new string[] { await ImgurHelper.UploadImage(new MemoryStream(image_bytes)) }:new string[] { }
                        })
                    };
                    await ApiClient.Users.Create(user);
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    if(ex.Message == "InternalServerError")
                        await DisplayAlert("Предупреждение", "Пользователь с таким логином уже существует", "OK");
                    else
                        await DisplayAlert("Предупреждение", "Server " + ex.Message, "OK");
                    register_butt.IsEnabled = true;
                    return;
                }
                catch (System.Net.WebException ex)
                {
                    await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                    register_butt.IsEnabled = true;
                    return;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Предупреждение", ex.Message, "OK");
                    register_butt.IsEnabled = true;
                    return;
                }
                Application.Current.Properties["authorized"] = "true";
                string token = await ApiClient.Auth.Authorize(user.Login, user.Password);
                Application.Current.Properties["usertoken"] = token;
                user = await ApiClient.Users.GetUserByLogin(user.Login, token);
                Application.Current.Properties["user"] = user;
                Application.Current.Properties["my_profile"] = true;
                //MessagingCenter.Send<Page>(this, "open_profile");
                MessagingCenter.Send<Page>(this, "update_profile_lenta");
                await Navigation.PopModalAsync(false);
                MessagingCenter.Send<Page>(this, "registered");
                Preferences.Set("login", user.Login);
                Preferences.Set("token", token);
                register_butt.IsEnabled = true;
            }
            else
            {
                await DisplayAlert("Предупреждение", "Пароли должны совпадать", "OK");
                return;
            }
        }

    }
}