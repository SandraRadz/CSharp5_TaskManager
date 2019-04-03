using MyTaskManager.ViewModels;

namespace MyTaskManager
{
    public partial class TaskManager
    {
        public TaskManager()
        {
            InitializeComponent();
            DataContext = new TaskManagerViewModel();
        }
    }
}