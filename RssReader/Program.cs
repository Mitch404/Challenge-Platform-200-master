using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RssReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var reader = new RssReader("http://rss.cnn.com/rss/edition.rss");
            reader.LoadStories();
            Searcher searcher = new Searcher(args);

            // Commented section implements what was initially attempted
            // I'm unsure if filtering the top five or top five of filtered stories is what
            // we want, so both are below.

            /*
            // creating a filtered list of the top five results.
            var stories = reader.GetTopStories();

            foreach (var story in stories.Where(r => searcher.SearchItHard(r.Title)))
            {
                story.ToString();
            }

            Console.WriteLine("**********************************************************************************");
            Console.WriteLine();
            */

            // This section will retrieve the five most recent stories which match
            reader.FilterStories(r => searcher.SearchItHard(r.Title));
            var filteredStories = reader.GetTopStories();

            foreach (var story in filteredStories)
            {
                story.ToString();
            }

            Console.ReadLine();
        }
    }

    public class RssStory
    {
        public int _index { get; set; }

        private string _title;
        public string Title { get => _title; set => _title = value; }

        private string _description;
        public string Description { get => _description; set => _description = value; }

        private Uri _link;
        public Uri Link { get => _link; set => _link = value; }

        public DateTime Published { get; set; }

        // Changed the string to a literal with @
        // also escaped the period in the host section to stop it from matching any character
        Regex regex = new Regex(@"(?<scheme>[a-z]{3,5})://(?<host>[a-z0-9_-]+(\.[a-z0-9_-]+)*)/(?<path>.*)");

        // alternate regex used for testing
        //Regex regex = new Regex(@"(?<scheme>\w+)://(?<host>[^/]+)/(?<path>.*)");

        public RssStory(string title, string description, string link, int index)
        {
            Title = title;
            Description = description;
            _index = index;

            Match match = regex.Match(link);

            if (match.Success)
            {
                Link = new UriBuilder(match.Groups["scheme"].Value, match.Groups["host"].Value, -1, match.Groups["path"].Value).Uri;
            }
            else
            {
                Console.Error.WriteLine("Error: Failed to match URI: {0}", link);
                Link = new Uri(link); // If we fail to match, hopefully Uri constructor will succeed
            }

            // 10,000 should be sufficient to avoid collisions.
            //_index = new Random().Next(10000);

            // Does index need to be random? It seems to only need to be distinct.
            // RssReader will now generate indicies for RssStory.
        }

        /// <summary>
        /// Writes RssStory attributes to console
        /// </summary>
        /// <returns>empty string</returns>
        public override string ToString()
        {
            Console.WriteLine("Title: " + Title);
            Console.WriteLine("Description: " + Description);
            Console.WriteLine("Published On: " + Published);
            Console.WriteLine("Link: " + Link);
            Console.WriteLine();

            return string.Empty;
        }
    }
}
