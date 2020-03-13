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
    public partial class NotesPage : ContentPage
    {
        private NotesPageViewModel ViewModel => BindingContext as NotesPageViewModel;

        public NotesPage()
        {
            InitializeComponent();
            this.BindingContext = new NotesPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy)
                return;
            ViewModel.LoadNotesCommand.Execute(null);
        }
    }
}