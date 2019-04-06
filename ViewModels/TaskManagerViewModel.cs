using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTaskManager.Annotations;
using MyTaskManager.Models;
using MyTaskManager.Tools.DataStorage;
using MyTaskManager.Tools.Managers;
using СSharp_Task4.Tools;

namespace MyTaskManager.ViewModels
{
    class TaskManagerViewModel: INotifyPropertyChanged
    {
        
        private ObservableCollection<MyProcess> _processes;
        private Thread _workingThread;
        private CancellationToken _token;
        private CancellationTokenSource _tokenSource;
        private BackgroundWorker _backgroundWorker;
        private Task _backgroundTask;

        private string _processName;
        private int _processId;
        private bool _isActive;
        private double _cpuPercent;
        private double _memoryPersent;
        private int _threadNum;
        private string _user;
        private string _filePath;
        private DateTime _startTime;

        private RelayCommand<object> _deleteProcessCommand;
        private RelayCommand<object> _openFolderCommand;


        public MyProcess SelectedItem { get; set; }

        public ObservableCollection<MyProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }


        internal TaskManagerViewModel()
        {
            _processes = new ObservableCollection<MyProcess>();
            foreach (Process process in Process.GetProcesses())
            {
                _processes.Add(new MyProcess(process));
            }

            //StationManager.Initialize(new MyDataStor(_processes.ToList()));

            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            StartWorkingThread();
            StationManager.StopThreads += StopWorkingThread;
        }

        public string ProcessName
        {
            get
            {
                return _processName;
            }
            set
            {
                _processName = value;
            }
        }

        public int ProcessId
        {
            get
            {
                return _processId;
            }
            set
            {
                _processId = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        public double CpuPercent
        {
            get
            {
                return _cpuPercent;
            }
            set
            {
                _cpuPercent = value;
            }
        }

        public double MemoryPersent
        {
            get
            {
                return _memoryPersent;
            }
            set
            {
                _memoryPersent = value;
            }
        }

        public int ThreadNum
        {
            get
            {
                return _threadNum;
            }
            set
            {
                _threadNum = value;
            }
        }

        public string User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }


        public RelayCommand<object> DeleteCommand
        {
            get
            {
                return _deleteProcessCommand ?? (_deleteProcessCommand = new RelayCommand<object>(
                           DeleteProcess));
            }
        }

        public RelayCommand<object> OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? (_openFolderCommand = new RelayCommand<object>(
                           OpenFolderProcess));
            }
        }


        private async void OpenFolderProcess(object o)
        {
            LoaderManager.Instance.ShowLoader();
            await Task.Run(() =>
            {
                    try
                    {
                    Process.Start("explorer.exe", "/select, C:\\Users\\oleks\\Downloads");
                }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    Thread.Sleep(500);

            });
            LoaderManager.Instance.HideLoader();
        }

        private async void DeleteProcess(object o)
        {
            LoaderManager.Instance.ShowLoader();
            await Task.Run(() =>
            {
                try
                {
                    foreach (Process proc in Process.GetProcessesByName(SelectedItem.ProcessName))
                    {
                        proc.Kill();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Choose a process");
                }
                List<MyProcess> pr = new List<MyProcess>();
                foreach (Process proc in Process.GetProcesses())
                {
                    pr.Add(new MyProcess(proc));
                }
                Processes = new ObservableCollection<MyProcess>(pr);
                Thread.Sleep(500);

            });
            LoaderManager.Instance.HideLoader();
        }

        private void StartWorkingThread()
        {
            _workingThread = new Thread(WorkingThreadProcess);
            _workingThread.Start();
        }

       

       
        private void WorkingThreadProcess()
        {
            int i = 0;
            while (!_token.IsCancellationRequested)
            {
                List<MyProcess> pr = new List<MyProcess>(); 
                foreach (Process proc in Process.GetProcesses())
                {
                    pr.Add(new MyProcess(proc));
                }
                Processes = new ObservableCollection<MyProcess>(pr);
            
                if (_token.IsCancellationRequested)
                    break;

           
                 Thread.Sleep(4000);
                
                if (_token.IsCancellationRequested)
                    break;
                i++;
            }
        }

      

        internal void StopWorkingThread()
        {
            _tokenSource.Cancel();
            _workingThread.Join(2000);
            _workingThread.Abort();
            _workingThread = null;
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
