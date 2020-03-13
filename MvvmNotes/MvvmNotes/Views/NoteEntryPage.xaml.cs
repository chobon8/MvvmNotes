using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmNotes.Models;
using MvvmNotes.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MvvmNotes.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoteEntryPage : ContentPage
    {
        public NoteEntryPageViewModel ViewModel => BindingContext as NoteEntryPageViewModel; 

        public NoteEntryPage()
        {
            InitializeComponent();
            if(BindingContext == null)
                BindingContext = new NoteEntryPageViewModel();
        }

        public NoteEntryPage(Note note):this()
        {
            BindingContext = new NoteEntryPageViewModel(note);
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            note.Date = DateTime.Now;

            await App.Database.SaveNoteAsync(note);

            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            await App.Database.DeleteNoteAsync(note);

            await Navigation.PopAsync();
        }
    }
}