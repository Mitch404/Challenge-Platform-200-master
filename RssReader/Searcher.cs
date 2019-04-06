using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader
{
    /// <summary>
    /// Ensures that only stuff being searched for is returned.
    /// </summary>
    public class Searcher
    {
        public bool not = false;
        public String s = "";

        /// <summary>
        /// Searched for articles with or without keyword based on arguments
        /// </summary>
        /// <param name="args">args[0] is the keyword searched for. 
        ///     args[1] == "true" will return articles without that keyword, 
        ///             != "true" will return articles with that keyword.
        /// </param>
        public Searcher(string[] args)
        {
            try
            {
                s = args[0] ?? " ";
                not = args[1].ToLower() == "true" ? true : false;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Console.Error.WriteLine("Error: " + exception);
                s = " ";
                not = false;
            }
            catch (Exception exception)
            {
                // more detailed message when something goes wrong in debug mode
#if DEBUG
                Console.Error.WriteLine("Error: Could not parse search parameters. There are " + args.Length + " parameters");
                Console.Error.WriteLine("Error: " + exception + "\n");
#else
                Console.WriteLine(args[0]);
#endif
                s = " ";
                not = false;
            }
            finally
            {
                s = s != " " ? s : "";
            }
        }

        /// <summary>
        /// Check and see if input matches conditions.
        /// </summary>
        /// <param name="toSearch"></param>
        /// <returns>Returns true if string matches, false otherwise.</returns>
        public bool SearchItHard(string toSearch)
        {
            return (toSearch.Contains(s) && !not) || (!toSearch.Contains(s) && not);
            /*
            bool matches = (toSearch.Contains(s) && !not) || (!toSearch.Contains(s) && not);

            if(matches == false)
            {
                throw new Exception("Doesn't match!");
            }

            return true;
            */
        }
    }
}
