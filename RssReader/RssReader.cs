using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RssReader
{
    public class RssReader : IRssReader
    {
        private const string FEED_ADDRESSS = "http://feeds.bbci.co.uk/news/rss.xml";

        private Dictionary<int, RssStory> _storyLookup;
        // Does this actually need to be thread safe?
        private ThreadSafeQueue _storyIndex;
        private List<RssStory> _stories;
        private XElement _feed;

        // Using a counter to generate unique indexes. If RssReader is expected to
        // read more than two billion articles in its lifespan, then we will 
        // need some other solution
        private int curIndex;

        // Constructors
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="feedAddress">Address of RSS news feed</param>
        public RssReader(string feedAddress)
        {
            //_feed = XElement.Load(FEED_ADDRESSS).Element("channel");

            // Taking from the value given to the constructor instead
            _feed = XElement.Load(feedAddress).Element("channel");

            _storyLookup = new Dictionary<int, RssStory>();
            _storyIndex = new ThreadSafeQueue();
            _stories = new List<RssStory>();
            curIndex = 0;
        }

        #endregion

        /// <summary>
        ///     Replaces _stories list with a sublist where stories match the predicate given.
        /// </summary>
        /// <param name="predicate">
        ///     Some expression which uses an RssStory and returns a bool such as
        ///     r => searcher.SearchItHard(r.Title)
        /// </param>
        public void FilterStories(Expression<Func<RssStory,bool>> predicate)
        {
            int startingCount = _stories.Count;
            var result = _stories.AsQueryable();
            _stories = result.Where(predicate).ToList();
            Console.WriteLine("{0} matching stories found out of {1}.\n", _stories.Count, startingCount);
        }


        /**********************
        *  ALL STORIES MUST BE
        *  LOADED PRIOR TO USE.
        ***********************/
        public void LoadStories()
        {
            foreach (var item in _feed.Elements("item"))
            {
                // There was a story in CNN's feed which had no description element, creating checks for null elements
                string newTitle = "";
                if (item.Element("title") != null) newTitle = item.Element("title").Value;

                string newDescription = "";
                if (item.Element("description") != null) newDescription = item.Element("description").Value;

                string newLink = "";
                if (item.Element("link") != null) newLink = item.Element("link").Value;

                var newStory = new RssStory(newTitle, newDescription, newLink, curIndex);
                curIndex += 1;

                if (item.Element("pubDate") != null)
                {
                    newStory.Published = DateTime.Parse(item.Element("pubDate").Value);
                }
                else
                {
                    newStory.Published = new DateTime(); // Assign new DateTime if pubDate element does not exist
                }

                _stories.Add(newStory);

                if (_storyLookup.ContainsKey(newStory._index))
                {
                    // Story is already in the list.
                    // Not sure why this happens, but we should remove it.

                    // I don't expect this to occur, but it will be reported now.
                    Console.Error.WriteLine("Error: Index Collision on index " + newStory._index);
                    _stories.Remove(_stories.Last());
                }
                else
                {
                    _storyLookup.Add(newStory._index, newStory);
                }
            }
        }

        public IEnumerable<RssStory> GetTopStories()
        {
            var latestStories = _stories.OrderBy(s => s.Published).Reverse();
            var recentStories = new List<RssStory>();

            // Get the five most recent stories from the feed.
            //var fiveLatestStoryIndices = latestStories.TakeWhile(s => s.Published < DateTime.Now.AddMinutes(30)).Select(s => s._index);

            // I'm going to keep this consistent with how it was documented and named, instead of keeping to the time restriction attempted
            var fiveLatestStoryIndices = latestStories.Take(5).Select(s => s._index);

            foreach (int index in fiveLatestStoryIndices)
            {
                _storyIndex.Enqueue(index);
            }

            while (_storyIndex.Any())
            {
                try
                {
                    int currentIndex = _storyIndex.Pop(-1);
                    if (currentIndex != -1)
                    {
                        recentStories.Add(_storyLookup[currentIndex]);
                    }
                    else // Unexpectedly out of stories
                    {
                        Console.Error.WriteLine("Error: Unexpectedly ran out of stories.");
                        break;
                    }
                    
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Error: Exception while iterating through current stories.");
                    Console.Error.WriteLine("Error: " + e);
                    break;
                }
            }

            return recentStories;
        }
    }
}
