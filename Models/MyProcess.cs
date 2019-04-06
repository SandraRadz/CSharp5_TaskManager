using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTaskManager.Models
{
    class MyProcess
    {
        private string _processName;
        private int _processId;
        private bool _isActive;
        private double _cpuPercent;
        private double _memoryPersent;
        private int _threadNum;
        private string _user;
        private string _filePath;
        private DateTime _startTime;

        public MyProcess(Process process)
        {
            _processName = process.ProcessName;
            _processId = process.Id;
            _isActive = process.Responding;
            _cpuPercent = 0;
            _memoryPersent = process.PrivateMemorySize64 / 1024;
            _threadNum = process.Threads.Count;
            _user = process.MachineName;
            try
            {

                _filePath = process.MainModule.FileName;
            }
            catch (Exception e)
            {
                _filePath = "Access Denied";
            }
            
            try
            {
                _startTime = process.StartTime;
            }
            catch (Exception e)
            {
               _filePath = "Access Denied";
            }

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
    }
}
