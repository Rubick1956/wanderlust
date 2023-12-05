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
using Xamarin.Essentials;

namespace Wunderlust.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<Page>(this, "registered", async (sender) => { await Navigation.PopAsync(); });
            if (Preferences.ContainsKey("login") && (string)Application.Current.Properties["authorized"] != "logouted")
                Auto_Login();
        }
        private async void Auto_Login()
        {
            User user;
            string token = Preferences.Get("token", ""), login = Preferences.Get("login", "");
            try
            {
                user = await ApiClient.Users.GetUserByLogin(login, token);
            }
            catch
            {
                return;
            }
            Application.Current.Properties["authorized"] = "true";
            Application.Current.Properties["user"] = user;
            Application.Current.Properties["my_profile"] = true;
            Application.Current.Properties["usertoken"] = token;
            MessagingCenter.Send<Page>(this, "update_profile_lenta");
            await Navigation.PopAsync();
        }
        private async void Register(object Sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new RegisterPage());
        }
        private async void Login_Clicked(object Sender, EventArgs e)
        {
            login_butt.IsEnabled = false;
            string token ="";
            try
            {
                token = await ApiClient.Auth.Authorize(logentry.Text, pasentry.Text);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                if(ex.Message== "BadRequest")
                    await DisplayAlert("Предупреждение", "Введите данные для входа", "OK");
                else if(ex.Message== "InternalServerError")
                    await DisplayAlert("Предупреждение", "Неверный логин или пароль", "OK");
                login_butt.IsEnabled = true;
                return;
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                login_butt.IsEnabled = true;
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                login_butt.IsEnabled = true;
                return;
            }
            Application.Current.Properties["authorized"] = "true";
            Application.Current.Properties["usertoken"] = token;
            User user = await ApiClient.Users.GetUserByLogin(logentry.Text, token);
            Application.Current.Properties["user"] = user;
            Application.Current.Properties["my_profile"] = true;
            //MessagingCenter.Send<Page>(this, "open_profile");
            MessagingCenter.Send<Page>(this, "update_profile_lenta");
            await Navigation.PopAsync();
            Preferences.Set("login", user.Login);
            Preferences.Set("token", token);
            login_butt.IsEnabled = true;
        }
        private async void Login_Anonim(object Sender, EventArgs e)
        {
            string token = "";
            try
            {
                token = await ApiClient.Auth.Authorize("_anonim_user_", "bezpasa");
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex.Message);
                await DisplayAlert("Предупреждение", "Требуется подключение к интернету", "OK");
                login_butt.IsEnabled = true;
                return;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Предупреждение", ex.Message, "OK");
                login_butt.IsEnabled = true;
                return;
            }
            Application.Current.Properties["authorized"] = "false";
            Application.Current.Properties["my_profile"] = true;
            Application.Current.Properties["usertoken"] = token;
            Application.Current.Properties["user"] = new User();
            //MessagingCenter.Send<Page>(this, "open_profile");
            MessagingCenter.Send<Page>(this, "update_profile_lenta");
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            Login_Anonim(new object(), new EventArgs());
            return true;
        }
    }
}