﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyTaskManager.Annotations;
using MyTaskManager.Models;
using MyTaskManager.Tools.Managers;
using СSharp_Task4.Tools;

namespace MyTaskManager.ViewModels
{
    class TaskManagerViewModel: INotifyPropertyChanged
    {
        
        private ObservableCollection<MyProcess> _processes;
        private ObservableCollection<ProcessThread> _threads;
        private ObservableCollection<ProcessModule> _modules;
        private Thread _workingThread;
        private Thread _workingThreadMod;
        private readonly CancellationToken _token;
        private readonly CancellationTokenSource _tokenSource;



        private string _processName;
        private int _processId;
        private bool _isActive;
        private double _cpuPercent;
        private double _memoryPersent;
        private int _threadNum;
        private string _user;
        private string _filePath;
        private DateTime _startTime;

        private int _threadId;
        private bool _threadActive;
        private DateTime _threadTime;
        private string _threadPath;

        private string _moduleName;
        private string _fileName;

        private RelayCommand<object> _deleteProcessCommand;
        private RelayCommand<object> _openFolderCommand;

        private MyProcess _selectedItem;

        public bool IsItemSelected => SelectedItem != null;

        public MyProcess SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnPropertyChanged($"IsItemSelected");
            }
        }

        public ObservableCollection<MyProcess> Processes
        {
            get => _processes;
            private set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProcessThread> Threads
        {
            get => _threads;
            private set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProcessModule> Modules
        {
            get => _modules;
            private set
            {
                _modules = value;
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

            _threads = new ObservableCollection<ProcessThread>();

            Process proc = Process.GetProcesses()[0];
            ProcessThreadCollection processThreads = proc.Threads;

            List<ProcessThread> pth = new List<ProcessThread>();
            foreach (ProcessThread thread in processThreads)
            {
                pth.Add(thread);
            }
            Threads = new ObservableCollection<ProcessThread>(pth);


            _modules = new ObservableCollection<ProcessModule>();
            ProcessModuleCollection processModules = proc.Modules;

            List<ProcessModule> pm = new List<ProcessModule>();
            foreach (ProcessModule module in processModules)
            {
                pm.Add(module);
            }
            Modules = new ObservableCollection<ProcessModule>(pm);



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

        public int ThreadId
        {
            get
            {
                return _threadId;
            }
            set
            {
                _threadId = value;
            }
        }

        public bool ThreadActive
        {
            get
            {
                return _threadActive;
            }
            set
            {
                _threadActive = value;
            }
        }

        public string ThreadPath
        {
            get
            {
                return _threadPath;
            }
            set
            {
                _threadPath = value;
            }
        }

        public DateTime TreadTime
        {
            get
            {
                return _threadTime;
            }
            set
            {
                _threadTime = value;
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
                        String mypath = SelectedItem.FilePath;
                    Process.Start("explorer.exe", "/select, "+mypath);
                }
                    catch (Exception ex)
                    {
                        MessageBox.Show("access denied");
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
                    Process proc = Process.GetProcessById(SelectedItem.ProcessId);
                   proc.Kill();
                    
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
            _workingThreadMod = new Thread(WorkingThreadProcessMod);
            _workingThreadMod.Start();
           
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

        private void WorkingThreadProcessMod()
        {
            int i = 0;
            while (!_token.IsCancellationRequested)
            {
                if (SelectedItem != null)
                {
                    Process proc = Process.GetProcessById(SelectedItem.ProcessId);
                    
                    try
                    {
                        ProcessThreadCollection processThreads = proc.Threads;
                        List<ProcessThread> pth = new List<ProcessThread>();
                        foreach (ProcessThread thread in processThreads)
                        {
                            pth.Add(thread);
                        }

                        Threads = new ObservableCollection<ProcessThread>(pth);


                    }
                    catch (Exception e)
                    {
                        Threads = new ObservableCollection<ProcessThread>();
                    }

                    
                    try
                    {
                        ProcessModuleCollection processModules = proc.Modules;
                        List<ProcessModule> pm = new List<ProcessModule>();
                        foreach (ProcessModule module in processModules)
                        {
                            pm.Add(module);
                        }

                        Modules = new ObservableCollection<ProcessModule>(pm);

                    }
                    catch (Exception e)
                    {
                        Modules = new ObservableCollection<ProcessModule>();
                    }

                   
                   
                }

                Thread.Sleep(2000);

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

            _workingThreadMod.Join(2000);
            _workingThreadMod.Abort();
            _workingThreadMod = null;


        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
