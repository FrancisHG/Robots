using Robots.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Robots.Models
{
    internal class RobotModel
    {
        #region Properties: Current
        /// <summary>
        /// This Robot's current X Position
        /// </summary>
        internal int CurrentXPosition { get; set; }
        /// <summary>
        /// This Robot's current X Position
        /// </summary>
        internal int CurrentYPosition { get; set; }

        internal RobotHeading CurrentRobotHeading { get; set; }
        #endregion

        #region Properties: Starting
        /// <summary>
        /// This Robot's starting X Position
        /// </summary>
        internal int StartingXPosition { get; set; }

        /// <summary>
        /// This Robot's starting X Position
        /// </summary>
        internal int StartingYPosition { get; set; }

        /// <summary>
        /// This Robot's starting X Position
        /// </summary>
        internal RobotHeading StartingRobotHeading { get; set; }
        #endregion

        #region Properties: IsLost
        /// <summary>
        /// Robot went off the grid
        /// </summary>
        internal bool IsFallout { get; set; }
        #endregion
    }
}