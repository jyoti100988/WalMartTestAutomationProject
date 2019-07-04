using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Walmart.Framework.Pages
{
    public interface IPage
    {
        string Title { get; }

        Uri Url { get; }

        string Location { get; set; }

        T NavigateTo<T>(string path ) where T : IPage;

        void WaitLoading();

        void AcceptDialog();

        void CancelDialog();

        void BrowserRefresh();

        void Quit();
    }
}
