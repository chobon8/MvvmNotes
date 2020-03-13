using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using MvvmNotes.Annotations;
using MvvmNotes.Models;
using MvvmNotes.Views;
using Xamarin.Forms;

namespace MvvmNotes.ViewModels
{
    public class NotesPageViewModel: BaseViewModel
    {
        public ObservableCollection<Note> Notes { get; set; }

        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set => SetProperty(ref _selectedNote, value);
        }

        public NotesPageViewModel()
        {
            Notes = new ObservableCollection<Note>();
        }

        private Command _addNoteCommand;

        public Command AddNoteCommand => _addNoteCommand ?? (_addNoteCommand = new Command(AddNote, () => IsNotBusy));

        private Command _loadNotesCommand;

        public Command LoadNotesCommand => _loadNotesCommand ?? (_loadNotesCommand = new Command(LoadNotes,()=> IsNotBusy));

        private Command<object> _selectCommand;
        public Command<object> SelectCommand => _selectCommand ?? (_selectCommand = new Command<object>(SelectNote, (obj) => IsNotBusy));


        private async void LoadNotes()
        {
            if (IsBusy) return;
            
            IsBusy = true;
            LoadNotesCommand.ChangeCanExecute();

            Notes.Clear();

            var notes = await App.Database.GetNotesAsync();

            foreach (var note in notes)
            {
                Notes.Add(note);
            }

            IsBusy = false;
            LoadNotesCommand.ChangeCanExecute();
        }

        private async void SelectNote(object eveArgs)
        {
            if (eveArgs is SelectedItemChangedEventArgs arg && arg.SelectedItem is Note note)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new NoteEntryPage(note));
            }

            SelectedNote = null;
        }

        private async void AddNote()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new NoteEntryPage());
        }
    }
}
