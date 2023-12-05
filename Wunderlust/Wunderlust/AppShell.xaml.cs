using System;
using System.Collections.Generic;
using Wunderlust.ViewModels;
using Wunderlust.Views;
using Xamarin.Forms;
namespace Wunderlust
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public bool tomyprofile = true;
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("toprofile", typeof(LentaPage));
            Routing.RegisterRoute("tocreate", typeof(LentaPage));
            Routing.RegisterRoute("tolenta", typeof(LentaPage));
            Routing.RegisterRoute("tonote", typeof(NotePage));
            //CurrentItem = lentaitem;
            login();
            MessagingCenter.Subscribe<Page>(this, "to_another_profile", (sender)=> { tomyprofile = false; });
        }
        async void login()
        {
            await Navigation.PushAsync(new LoginPage());
        }
        private async void TabBar_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var tabbar = sender as TabBar;

            if (e.PropertyName == "CurrentItem")
            {
                if ((tabbar.CurrentItem == tabbar.Items[0] || tabbar.CurrentItem == tabbar.Items[1]) && (string)Application.Current.Properties["authorized"] == "false")
                {//block pages for not authorized user
                    await DisplayAlert("Предупреждение", "Авторизуйтесь", "OK");
                    await Shell.Current.GoToAsync("//tolenta");
                }
                else if (tabbar.CurrentItem == tabbar.Items[0] && (string)Application.Current.Properties["authorized"] == "true")
                {//update profile content
                    if (tomyprofile)
                    {
                        Application.Current.Properties["my_profile"] = true;
                        MessagingCenter.Send<Page>(this, "update_profile_lenta");
                    }
                    else
                    {
                        Application.Current.Properties["my_profile"] = false;
                        MessagingCenter.Send<Page>(this, "update_profile_lenta_another");
                    }
                    //MessagingCenter.Send<Page>(this, "open_profile");
                    tomyprofile = true;
                }
                else if((string)Application.Current.Properties["authorized"] != "expected" && tabbar.CurrentItem == tabbar.Items[2])
                {//reload lenta
                    MessagingCenter.Send<Page>(this, "update_lenta");
                }
            }
        }
    }
}
