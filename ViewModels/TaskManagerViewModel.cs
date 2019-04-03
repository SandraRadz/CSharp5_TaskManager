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
using MyTaskManager.Annotations;
using MyTaskManager.Models;
using MyTaskManager.Tools.Managers;

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
           // foreach (Process process in Process.GetProcesses())
           // {
           //     Console.WriteLine("ID: {0}  Name: {1}", process.Id, process.ProcessName);
           //}
            _processes = new ObservableCollection<MyProcess>(StationManager.DataStorage.ProcessesList);
            Console.WriteLine("hello");
            Console.WriteLine(_processes.Count);
           _processName="";
           _processId=0;
           _isActive = false;
           _cpuPercent=0;
           _memoryPersent=0;
           _threadNum=0;
           _user="";
          _filePath="";
           _startTime=DateTime.Today;
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
  

    private void StartWorkingThread()
        {
            _workingThread = new Thread(WorkingThreadProcess);
            _workingThread.Start();
        }

        private void StartBackgroundWorker()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += BackgroundWorkerProcess;
            _backgroundWorker.ProgressChanged += BackgroundWorkerOnProgressChanged;
            _backgroundWorker.RunWorkerAsync();
        }
        

        private void StartBarckgroundTask()
        {
            _backgroundTask = Task.Factory.StartNew(BackgroundTaskProcess, TaskCreationOptions.LongRunning);
        }

        private void WorkingThreadProcess()
        {
           /* int i = 0;
            while (!_token.IsCancellationRequested)
            {
                var users = _processes.ToList();
                users.Add(new Process("FirstNAme" + i, "LastNAme" + i, "Email" + i, "Login" + i, "Password" + i));
                LoaderManager.Instance.ShowLoader();
                Process = new ObservableCollection<Process>(users);
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }
                if (_token.IsCancellationRequested)
                    break;
                LoaderManager.Instance.HideLoader();
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }
                if (_token.IsCancellationRequested)
                    break;
                i++;
            }*/
        }

        private void BackgroundWorkerProcess(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            /*var worker = (BackgroundWorker)sender;
            int i = 0;
            while (!worker.CancellationPending)
            {
                var users = _processes.ToList();
                users.Add(new User("FirstNAme" + i, "LastNAme" + i, "Email" + i, "Login" + i, "Password" + i));
                worker.ReportProgress(10, users);
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);
                    if (worker.CancellationPending)
                    {
                        doWorkEventArgs.Cancel = true;
                        _tokenSource.Cancel();
                        break;
                    }
                }
                i++;
            }*/
        }
        private async void BackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            LoaderManager.Instance.ShowLoader();
            await Task.Run(() =>
            {
                Processes = new ObservableCollection<MyProcess>((List<MyProcess>)progressChangedEventArgs.UserState);
                Thread.Sleep(2000);
            });
            LoaderManager.Instance.HideLoader();
        }

        private void BackgroundTaskProcess()
        {
            /*int i = 0;
            while (!_token.IsCancellationRequested)
            {
                var users = _processes.ToList();
                users.Add(new User("FirstNAme" + i, "LastNAme" + i, "Email" + i, "Login" + i, "Password" + i));
                LoaderManager.Instance.ShowLoader();
                Users = new ObservableCollection<User>(users);
                for (int j = 0; j < 3; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }
                if (_token.IsCancellationRequested)
                    break;
                LoaderManager.Instance.HideLoader();
                for (int j = 0; j < 10; j++)
                {
                    Thread.Sleep(500);
                    if (_token.IsCancellationRequested)
                        break;
                }
                if (_token.IsCancellationRequested)
                    break;
                i++;
            }*/
        }

        internal void StopWorkingThread()
        {
            _tokenSource.Cancel();
            _workingThread.Join(2000);
            _workingThread.Abort();
            _workingThread = null;
        }

        internal void StopBackgroundWorker()
        {
            _backgroundWorker.CancelAsync();
            for (int i = 0; i < 4; i++)
            {
                if (_token.IsCancellationRequested)
                    break;
                Thread.Sleep(500);
            }
            _backgroundWorker.Dispose();
            _backgroundWorker = null;
        }

        internal void StopBackgroundTask()
        {
            _tokenSource.Cancel();
            _backgroundTask.Wait(2000);
            _backgroundTask.Dispose();
            _backgroundTask = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
