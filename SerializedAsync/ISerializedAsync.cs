using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MusicFileManager.SerializedAsync
{
    public interface ISerializedAsync
    {
        void AddSerialzedAsync(ISerializedAsync serializedAsync);
        void DoSerializedAsync(object argument, DoWorkEventArgs e = null);
    }
}
