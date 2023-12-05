using Client;
using Client.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wunderlust.Models;
using Wunderlust.ViewModels;
using Wunderlust.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wunderlust.Views
{
    public partial class LentaPage : ContentPage
    {
        public LentaPage()
        {
            InitializeComponent();
            lenta_view.Refreshing += Refresh;
        }
        private void Refresh(object sender, EventArgs e)
        {
            MessagingCenter.Send<Page>(this, "update_lenta");
            (sender as ListView).IsRefreshing = false;
        }
        private async void note_selected(object sender, ItemTappedEventArgs e)
        {
            User user;
            try
            {
                user = await ApiClient.Users.GetUserById(LentaViewModel.notes[e.ItemIndex].OwnerId, (string)Application.Current.Properties["usertoken"]);
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
            NotePage.Note = LentaViewModel.notes[e.ItemIndex];
            NotePage.Author = user;
            NotePage.from = "lenta";
            await Shell.Current.GoToAsync("tonote");
        }
    }
}