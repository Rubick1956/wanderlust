using Client.Entities;
using Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Wunderlust.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using static Wunderlust.Views.ProfilePage;

namespace Wunderlust.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public InfiniteScrollCollection<Lenta_item> Items { get; set; }
        public static List<Note> notes = new List<Note>();
        private DateTime firstdate = DateTime.Now;
        private bool load_more = true;
        private bool author = true;
        private void Update(bool _author)
        {
            author = _author;
            firstdate = DateTime.Now;
            load_more = true;
            notes.Clear();
            Items.Clear();
            Items.LoadMoreAsync();
        }
        public ProfileViewModel()
        {
            MessagingCenter.Subscribe<Page>(this, "update_profile_lenta", (sender) => { Update(true); });
            MessagingCenter.Subscribe<Page>(this, "update_profile_lenta_another", (sender) => { Update(false); });
            MessagingCenter.Subscribe<Page>(this, "update_profile_lenta_default", (sender) => { Update(author); });
            Title = "Профиль";
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
                            note = (await ApiClient.Notes.GetNotesByAuthor((Guid)((User)Application.Current.Properties[author ? "user" : "another_user"]).Id, firstdate,
                                (string)Application.Current.Properties["usertoken"])).ToList();
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