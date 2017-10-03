using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImportFRMP
{
    internal sealed class TaskClass
    {
        private Thread taskThread;
        private Boolean threadEnabled;
        private TimeSpan timeToStart;
        private ThreadStart workMetod;

        /// <summary>
        /// Активность потока
        /// </summary>
        public Boolean ThreadEnabled
        {
            get { return this.threadEnabled; }
            set { this.threadEnabled = value; }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="method">Метод для вызова</param>
        /// <param name="timeToStart">Время запуска</param>
        public TaskClass(ThreadStart method, TimeSpan timeToStart)
        {
            this.taskThread = new Thread(WorkMeth);

            this.workMetod = method;
            this.timeToStart = timeToStart;
        }

        /// <summary>
        /// Запуск потока
        /// </summary>
        public void Start()
        {
            this.threadEnabled = true;
            this.taskThread.Start();
        }

        /// <summary>
        /// Остановка потока
        /// </summary>
        public void Stop()
        {
            this.threadEnabled = false;

            try
            {
                this.taskThread.Join(5000);
            }
            catch
            {
                try
                {
                    this.taskThread.Abort();
                }
                catch { }
            }
        }

        private void WorkMeth()
        {
            while (this.threadEnabled)
            {
                if (this.workMetod != null)
                    this.workMetod();

                var nowTime = DateTime.Now.TimeOfDay;
                var seconds = 0.0;
                if (nowTime <= this.timeToStart)
                    seconds = (this.timeToStart - nowTime).TotalSeconds;
                else
                    seconds = 24 * 60 * 60 - (nowTime - this.timeToStart).TotalSeconds;

                Thread.Sleep((int)seconds * 1000);
            }
        }
    }
}
