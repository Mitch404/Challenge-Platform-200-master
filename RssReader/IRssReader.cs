using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader
{
    // Ready for DI.
    
    // This wasn't being implemented by anything, RssReader now implements it
    public interface IRssReader
    {
        void LoadStories();
        IEnumerable<RssStory> GetTopStories();
    }
}
