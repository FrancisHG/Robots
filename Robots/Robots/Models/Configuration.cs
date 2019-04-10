using Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;

namespace Robots.Controllers
{
    internal class Configuration
    {
        #region Defaults
        internal class Defaults
        {
            // Grids usually start at 0...
            internal const int GridBottomXBoundary = 0;

            // Grids usually start at 0...
            internal const int GridBottomYBoundary = 0;

            internal const RobotHeading DefaultRobotHeading = RobotHeading.N;
        }
        #endregion

        #region IO / Parsing
        internal const int xPositionIndex = 0;
        internal const int yPositionIndex = 1;
        internal const int startingHeadingIndex = 2;

        internal const string outputFormat = "{0} {1} {2}{3}{4}";

        internal const string lostText = "LOST";
        internal const int maxNumberOfHeadings = 4;

        /// <summary>
        /// Pattern used to split the input string
        /// </summary>
        internal const string RegexParseInput = @"(\s)";

        /// <summary>
        /// Singleton of InputParser regex object
        /// </summary>
        internal static Regex InputParser = new Regex(RegexParseInput);
        #endregion
    }
}