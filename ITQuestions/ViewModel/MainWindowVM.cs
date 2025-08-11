using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.Model;
using ITQuestions.Service;
using ITQuestions.View;
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
        private readonly DatabaseService _databaseService = new DatabaseService();
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
            var data = await _databaseService.GetQuestionsAsync();
            Questions.Clear();
            foreach (var q in data) //.Skip(1)) // 1st element is null, will solve later, for now, use Skip(1)-skip one elemetn, this time first
            {   
                if (q == null)
                    { continue; }
                
                Questions.Add(q);
            }
        }

        [RelayCommand]
        private void OpenNewQuestionWindow()
        {
            var window = new NewQuestion();
            window.ShowDialog(); // No reload here — reload happens from SubmitQuestionCommand
        }
    }
}
