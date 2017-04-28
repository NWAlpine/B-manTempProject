using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_manTempProj
{
    class Program
    {
        internal enum DataType
        {
            NewData,
            Error,
        };
        public enum SensorId
        {
            Temp,
            Humidity,
            HeatIndex,
            Light,
        }

        static void Main(string[] args)
        {
            DataSimulator ds = new DataSimulator(500, 2000);
            ds.NewDataEvent += ProcessData;

            // sync method
            //ds.Start();

            // async method
            Task begin = ds.StartAsync();

            Console.WriteLine("Running....");

            // continue doing other work
            while (true)
            {
                // Hi Bruce, you can have the app do other work
                // while the data generator is running.
                // or let it just run.
            }
        }

        private static void ProcessData(object o, DataSimulator.DataEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Message Type: ");
            sb.Append((DataType)e.MessageType);
            sb.Append("\t Device Pod: ");
            sb.Append(e.DevicePod);
            sb.Append("\t Sensor: ");
            sb.Append((SensorId)e.SensorId);
            sb.Append("\t Value: ");
            sb.Append(e.SensorValue);
            sb.Append("\n");

            Console.WriteLine(sb.ToString());

            //Console.WriteLine(e.MessageType);
            //Console.WriteLine(e.DevicePod);
            //Console.WriteLine(e.SensorId);
            //Console.WriteLine(e.SensorValue);
            //Console.WriteLine();
        }
    }

    public class DataSimulator
    {
        public delegate void DataHandler(object o, DataEventArgs e);
        public event DataHandler NewDataEvent;

        public class DataEventArgs : EventArgs
        {
            // MessageType: 0 = data, 1 = error, 2 = heartbeat?
            // which sensor pod device is data from
            // sensor ID: T = temp, H = humidity, I = HeatIndex, L = Light
            // value of sensor
            public int MessageType
            { get; internal set; }
            public int DevicePod
            { get; internal set; }
            public int SensorId
            { get; internal set; }
            public int SensorValue
            { get; internal set; }
        }

        /* schema
         * DataType
         * NewData = 0
         * Error = 1
         * 
         * SensorId
         * Temp = 0
         * Humidity = 1
         * Heat Index = 2
         * Light = 3
         */

        private int _min, _max;

        // ctor, request the boundarys for random data
        public DataSimulator(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public void Start()
        {
            // sync
            DataEventArgs dea = new DataEventArgs();
            Random rnd = new Random();

            while (true)
            {
                System.Threading.Thread.Sleep(rnd.Next(_min, _max));
                GenerateData(dea);
                if (NewDataEvent != null)
                {
                    NewDataEvent(this, dea);
                }
            }
        }

        public async Task StartAsync()
        {
            // async
            DataEventArgs dea = new DataEventArgs();
            while (true)
            {
                await GenerateDataAsync(dea);
                if (NewDataEvent != null)
                {
                    NewDataEvent(this, dea);
                }
            }
        }

        private async Task GenerateDataAsync(DataEventArgs eventArg)
        {
            Random rnd = new Random();

            eventArg.MessageType = rnd.Next(0, 2);
            eventArg.DevicePod = rnd.Next(0, 3);
            eventArg.SensorId = rnd.Next(0, 4);
            eventArg.SensorValue = rnd.Next(0, 50);

            // wait for the delay before returning
            await Task.Delay(rnd.Next(_min, _max));
        }

        private void GenerateData(DataEventArgs eventArg)
        {
            Random rnd = new Random();

            // pick random numbers based on property limits
            eventArg.MessageType = rnd.Next(0, 2);
            eventArg.DevicePod = rnd.Next(0, 3);
            eventArg.SensorId = rnd.Next(0, 4);
            eventArg.SensorValue = rnd.Next(0, 51);
        }
    }
}
