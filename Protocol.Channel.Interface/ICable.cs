using Hydrology.Entity;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Protocol.Channel.Interface
{
    public interface ICable : IChannel
    {
        SerialPort Port { get; set; }

        /// <summary>
        /// 初始化北斗卫星监视串口
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        void Init(string portName, int baudRate);
        /// <summary>
        /// 打开串口
        /// </summary>
        bool Open();

        event EventHandler<COUTEventArgs> COUTCompleted;
    }
}
