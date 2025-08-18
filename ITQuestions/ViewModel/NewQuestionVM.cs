using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.Model;
using ITQuestions.Service;
using ITQuestions.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ITQuestions.ViewModel
{
    public partial class NewQuestionVM : ObservableObject
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        private string _newQuestion;
        public string NewQuestion
        {
            get => _newQuestion;
            set
            {
                _newQuestion = value;
                OnPropertyChanged();
            }
        }

        private string _newAnswer;
        public string NewAnswer
        {
            get => _newAnswer;
            set
            {
                _newAnswer = value;
                OnPropertyChanged();
            }
        }

        //public NewQuestionVM() {}

        [RelayCommand]
        private async Task SubmitQuestion()
        {
            await _databaseService.AddQuestionAsync(NewQuestion, NewAnswer);
            NewQuestion = string.Empty;
            NewAnswer = string.Empty;

            // Refresh the main list in the background
            if (Application.Current.MainWindow.DataContext is MainWindowVM mainVM)
            {
                await mainVM.LoadQuestionsAsync();
            }
        }
    }
}
