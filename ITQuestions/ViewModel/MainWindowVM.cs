using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.Model;
using ITQuestions.Service;
using ITQuestions.View;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ITQuestions.ViewModel
{
    public partial class MainWindowVM : ObservableObject
    {
        public ObservableCollection<ITQuestion> Questions { get; set; } = new ObservableCollection<ITQuestion>();

        private ITQuestion _selectedQuestion;
        public ITQuestion SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                OnPropertyChanged();
            }
        }

        public MainWindowVM()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
            {
                await LoadQuestionsAsync();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public async Task LoadQuestionsAsync()
        {
            var data = await DatabaseService.Instance.GetQuestionsAsync();
            Questions.Clear();
            foreach (var q in data)
            {   
                if (q == null)
                    { continue; }
                
                Questions.Add(q);
            }
        }

        [RelayCommand]
        private void AddNewQuestionAsync()
        {
            var window = new NewQuestion();
            window.ShowDialog();
        }

        [RelayCommand]
        private void UpdateQuestionAsync(ITQuestion question)
        {

            UpdateQuestionVM updateViewModel = new UpdateQuestionVM(question);
            var window = new UpdateQuestion
            {
                DataContext = updateViewModel
            };
            window.ShowDialog();           
        }

        [RelayCommand]
        private async Task DeleteQuestionAsync(ITQuestion question)
        {
            await DatabaseService.Instance.DeleteQuestionAsync(question);
            await LoadQuestionsAsync(); // Refresh list
        }
    }
}
