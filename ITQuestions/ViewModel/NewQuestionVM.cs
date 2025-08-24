using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.DB;
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
        //private readonly LocalITQuestionRepository _local = LocalITQuestionRepository.Instance;

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
            };

            NewQuestion = string.Empty;
            NewAnswer = string.Empty;

            // Save locally
            using (var db = new DBContext())
            {
                db.ITQuestions.Add(q);
                await db.SaveChangesAsync();
            }

            // Push remote
            //await DatabaseService.Instance.AddQuestionAsync(q);

            // Refresh UI
            if (Application.Current.MainWindow.DataContext is MainWindowVM mainVM)
            {
                await mainVM.LoadQuestionsAsync();
            }
        }

        /// <summary>
        /// Method to get data from remote database.
        /// </summary>
        /// <param name="q">Question in remote database.</param>
        //public async Task AddQuestionAsync(ITQuestion q)
        //{
        //    using var db = new DBContext();
        //    if (string.IsNullOrEmpty(q.FirebaseKey))
        //        q.FirebaseKey = Guid.NewGuid().ToString();

        //    db.ITQuestions.Add(q);
        //    await db.SaveChangesAsync();

        //}
    }
}
