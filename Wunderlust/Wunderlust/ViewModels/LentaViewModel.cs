using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Wunderlust.Models;
using Wunderlust.Views;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using Client;
using static Wunderlust.Views.LentaPage;
using Client.Entities;
using System.Linq;
using System.Collections.Generic;
namespace Wunderlust.ViewModels
{
    public class LentaViewModel : BaseViewModel
    {
        public InfiniteScrollCollection<Lenta_item> Items { get; set; }
        public static List<Note> notes = new List<Note>();
        private DateTime firstdate = DateTime.Now;
        private bool load_more = true;
        public LentaViewModel()
        {
            MessagingCenter.Subscribe<Page>(this, "update_lenta", (sender) => { 
                firstdate = DateTime.Now;
                load_more = true;
                notes.Clear();
                Items.Clear();
                Items.LoadMoreAsync();
            });
            Title = "Лента";
            Items = new InfiniteScrollCollection<Lenta_item>
            {
                OnLoadMore = async () =>
                {
                    if (load_more)
                    {
                        var items = new InfiniteScrollCollection<Lenta_item>();
                        List<Note> note;
                        try
                        {
                            if (Items.Count > 0)
                                note = (await ApiClient.Notes.GetNotesBeforeDate(firstdate, (string)Application.Current.Properties["usertoken"])).ToList();
                            else
                                note = (await ApiClient.Notes.GetFirstTenNotes((string)Application.Current.Properties["usertoken"])).ToList();
                            note.Reverse();
                            foreach (var item in note)
                            {
                                items.Add(new Lenta_item(item));
                                notes.Add(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        if (items.Count > 0)
                            firstdate = items.Last().Note.CreateDate;
                        if (items.Count < 10)
                            load_more = false;
                        return items;
                    }
                    else { return null; }
                }                
            };
            // load the initial data
            Items.LoadMoreAsync();
        }
    }
}