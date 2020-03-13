using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using MvvmNotes.Annotations;
using MvvmNotes.Models;
using Xamarin.Forms;

namespace MvvmNotes.ViewModels
{
    public class NoteEntryPageViewModel: BaseViewModel
    {

        private readonly Note _note;

        public NoteEntryPageViewModel()
        {
            _note = new Note();
        }

        public NoteEntryPageViewModel(Note note)
        {
            _note = note;
        }

        public string Text
        {
            get => _note.Text;
            set
            {
                _note.Text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        private Command _saveCommand;

        public Command SaveCommand =>
            _saveCommand ?? (_saveCommand = new Command(Save, () => IsNotBusy));

        private Command _deleteCommand;

        public Command DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new Command(Delete, () => IsNotBusy));


        private async void Save()
        {
            _note.Date = DateTime.Now;

            await App.Database.SaveNoteAsync(_note);

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void Delete()
        {
            await App.Database.DeleteNoteAsync(_note);

            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
