using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp9
{
    public interface IRefreshablePage
    {
        Task ReloadTasksAsync();
    }
}
