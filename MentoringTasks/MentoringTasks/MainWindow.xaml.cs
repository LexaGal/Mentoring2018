using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MentoringTasks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        readonly Dictionary<Task, Label> _taskLabels = new Dictionary<Task, Label>();
        readonly Task[] _tasks = new Task[5];

        public async void Start()
        {
            var rnd = new Random();
            for (var i = 0; i < 5; i++)
            {
                var next = rnd.Next(2000, 10000);
                var j = i;
                _tasks[i] = Task.Run(async () =>
                {
                    await Task.Delay(next);
                    await UpdateControl(next.ToString(), _tasks[j]);
                });
                AddControlForTask(i);                                             
            }
            await Task.WhenAll(_tasks);
        }

        private void AddControlForTask(int i)
        {
            var label = new Label {Content = $"Task {i + 1}"};
            var status = new Label {Content = "Started"};
            Panel.Children.Add(label);
            Panel.Children.Add(status);
            _taskLabels.Add(_tasks[i], status);
        }

        private async Task UpdateControl(string time, Task task)
        {
            await Dispatcher.BeginInvoke(new Action(() => _taskLabels[task].Content = $"Finished in {time}"));
        }
    }
}