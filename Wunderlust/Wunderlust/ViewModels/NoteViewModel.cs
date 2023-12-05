using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Wunderlust.Models;
using Xamarin.Forms;

namespace Wunderlust.ViewModels
{
    public class NoteViewModel : BaseViewModel
    {
        public NoteViewModel()
        {
            Title = "Запись";
        }
    }
}
