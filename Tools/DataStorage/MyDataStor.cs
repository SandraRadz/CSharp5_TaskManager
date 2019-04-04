using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyTaskManager.Models;

namespace MyTaskManager.Tools.DataStorage
{
    class MyDataStor: IDataStorage
    {
        public List<MyProcess> _processes;

        public void AddProcess(MyProcess process)
        {
            throw new NotImplementedException();
        }

        public List<MyProcess> ProcessesList
        {
            get
            {
                return _processes;

            }
            set
            {
                _processes = value;
            }
        }


        internal MyDataStor()
        {
            _processes = new List<MyProcess>();
            foreach (Process process in Process.GetProcesses())
            {
                _processes.Add(new MyProcess(process));
            }

        }

        internal MyDataStor(List<MyProcess> proc)
        {
            _processes = proc;

        }

    }
}

