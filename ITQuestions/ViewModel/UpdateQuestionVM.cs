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
    public partial class UpdateQuestionVM : ObservableObject
    {

        private ITQuestion _originalQuestion;

        private string _updateQuestion;
        public string UpdateQuestion
        {
            get => _updateQuestion;
            set
            {
                _updateQuestion = value;
                OnPropertyChanged();
            }
        }

        private string _updateAnswer;
        public string UpdateAnswer
        {
            get => _updateAnswer;
            set
            {
                _updateAnswer = value;
                OnPropertyChanged();
            }
        }

        public UpdateQuestionVM(ITQuestion question)
        {
            _originalQuestion = question;
            UpdateQuestion = question.Question;  
            UpdateAnswer = question.Answer;
        }

        [RelayCommand]
        private async Task UpdateQuestionAsync()
        {
            ITQuestion _updatedQuestion = _originalQuestion;
            _updatedQuestion.Question = UpdateQuestion;
            _updatedQuestion.Answer = UpdateAnswer;

            await DatabaseService.Instance.UpdateQuestionAsync(_updatedQuestion);

            // Refresh the main list in the background
            if (Application.Current.MainWindow.DataContext is MainWindowVM mainVM)
            {
                await mainVM.LoadQuestionsAsync();
            }
        }
    }
}
