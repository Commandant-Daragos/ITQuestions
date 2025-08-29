using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.DB;
using ITQuestions.Enum;
using ITQuestions.Model;
using System.Windows;

namespace ITQuestions.ViewModel
{
    public partial class NewQuestionVM : ObservableObject
    {
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

        /// <summary>
        /// Command for Submit buttorn to add new question from app.
        /// </summary>
        [RelayCommand]
        public async Task AddQuestionAsync()
        {
            var q = new ITQuestion
            {
                Question = NewQuestion,
                Answer = NewAnswer,
                SyncStatus = SyncStatus.Add,
            };

            NewQuestion = string.Empty;
            NewAnswer = string.Empty;

            using (var db = new DBContext())
            {
                db.ITQuestions.Add(q);
                await db.SaveChangesAsync();
            }

            if (Application.Current.MainWindow.DataContext is MainWindowVM mainVM)
            {
                await mainVM.LoadQuestionsAsync();
            }
        }
    }
}
