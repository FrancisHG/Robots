using Robots.Controllers;
using Robots.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Robots.Controllers
{
    /// <summary>
    /// Collection of Robots (Consider implementing ICollection)
    /// </summary>
    public class RobotsCollection
    {
        #region Properties
        /// <summary>
        /// Persist the Robots across the entire application
        /// (Warning: Only works on a single instance of server, farms will need more sophisticated caching)
        /// </summary>
        internal List<RobotModel> Robots = new List<RobotModel>();
        internal int[,,] FallOffGrid { get; set; }
        internal int GridMaxX { get; set; }
        internal int GridMaxY { get; set; }
        #endregion

        #region Public - IO

        public void InitializeGrid(int gridMaxX, int gridMaxY)
        {
            GridMaxX = gridMaxX;
            GridMaxY = gridMaxY;
            FallOffGrid = new int[GridMaxX + 1, GridMaxY + 1, Configuration.maxNumberOfHeadings];
        }

        /// <summary>
        /// Add a new Robot
        /// </summary>
        /// <param name="inputString">Raw input string to parse</param>
        public void AddRobot(string initialConfig, string actions)
        {
            string[] values = Configuration
                .InputParser.Split(initialConfig)
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .ToArray();
            
            int startingX = Configuration.Defaults.GridBottomXBoundary;
            bool foundX = false;

            if (values.Length > Configuration.xPositionIndex)
                foundX = int.TryParse(values[Configuration.xPositionIndex], out startingX);

            int startingY = Configuration.Defaults.GridBottomYBoundary;
            bool foundY = false;

            if (values.Length > Configuration.yPositionIndex)
                foundY = int.TryParse(values[Configuration.yPositionIndex], out startingY);

            RobotHeading startingHeading = Configuration.Defaults.DefaultRobotHeading;
            bool foundHeading = false;

            if (values.Length > Configuration.startingHeadingIndex)
                foundHeading = Enum.TryParse(values[Configuration.startingHeadingIndex], out startingHeading);

            if (!foundHeading && !foundY && values.Length > Configuration.yPositionIndex)
                foundHeading = Enum.TryParse(values[Configuration.yPositionIndex], out startingHeading);

            PositionTracking currentTrackingObject = new PositionTracking()
            {
                X = startingX,
                Y = startingY,
                Heading = startingHeading
            };

            foreach (char rawAction in actions.ToCharArray())
            {
                RobotAction action;
                if (!Enum.TryParse(rawAction.ToString(), out action))
                    continue;

                switch (action)
                {
                    case RobotAction.L: RotateLeft(currentTrackingObject); break;
                    case RobotAction.R: RotateRight(currentTrackingObject); break;
                    case RobotAction.F: MoveForward(currentTrackingObject); break;
                }

                // Robot is lost, discard all following commands
                // Attempting to execute further commands can exceed capacity of FallOffGrid, resulting in error
                if (currentTrackingObject.IsFallout) break;
            }

            Robots.Add(new RobotModel()
            {
                StartingXPosition = startingX,
                StartingYPosition = startingY,
                StartingRobotHeading = startingHeading,
                IsFallout = currentTrackingObject.IsFallout,
                CurrentXPosition = currentTrackingObject.X,
                CurrentYPosition = currentTrackingObject.Y,
                CurrentRobotHeading = currentTrackingObject.Heading
            });
        }

        public string GetOutput()
        {
            IEnumerable<char> rawData =
                Robots.SelectMany(x =>
                String.Format(Configuration.outputFormat,
                x.CurrentXPosition,
                x.CurrentYPosition,
                x.CurrentRobotHeading.ToString(),
                x.IsFallout
                ? " " + Configuration.lostText
                : String.Empty,
                System.Environment.NewLine));

            return String.Concat(rawData);
        }
        #endregion

        #region Private logic
        private void RotateLeft(PositionTracking currentTrackingObject)
        {
            switch (currentTrackingObject.Heading)
            {
                case RobotHeading.N: currentTrackingObject.Heading = RobotHeading.W; break;
                case RobotHeading.W: currentTrackingObject.Heading = RobotHeading.S; break;
                case RobotHeading.S: currentTrackingObject.Heading = RobotHeading.E; break;
                default:
                case RobotHeading.E: currentTrackingObject.Heading = RobotHeading.N; break;
            }
        }

        private void RotateRight(PositionTracking currentTrackingObject)
        {
            switch (currentTrackingObject.Heading)
            {
                case RobotHeading.N: currentTrackingObject.Heading = RobotHeading.E; break;
                case RobotHeading.E: currentTrackingObject.Heading = RobotHeading.S; break;
                case RobotHeading.S: currentTrackingObject.Heading = RobotHeading.W; break;
                default:
                case RobotHeading.W: currentTrackingObject.Heading = RobotHeading.N; break;
            }
        }

        private void MoveForward(PositionTracking currentTrackingObject)
        {
            // This move command will be discarded if another Robot has fallen off already
            switch (currentTrackingObject.Heading)
            {
                case RobotHeading.N: MoveNorth(currentTrackingObject); break;
                case RobotHeading.E: MoveEast(currentTrackingObject); break;
                case RobotHeading.S: MoveSouth(currentTrackingObject); break;
                case RobotHeading.W: MoveWest(currentTrackingObject); break;
            }
        }

        private void MoveNorth(PositionTracking currentTrackingObject)
        {
            if (currentTrackingObject.Y + 1 > GridMaxY)
            {
                FallOutCheck(currentTrackingObject);
            }
            else
            {
                currentTrackingObject.Y++;
            }
        }

        private void MoveSouth(PositionTracking currentTrackingObject)
        {
            if (currentTrackingObject.Y - 1 < Configuration.Defaults.GridBottomYBoundary)
            {
                FallOutCheck(currentTrackingObject);
            }
            else
            {
                currentTrackingObject.Y--;
            }
        }

        private void MoveEast(PositionTracking currentTrackingObject)
        {
            if (currentTrackingObject.X + 1 > GridMaxX)
            {
                FallOutCheck(currentTrackingObject);
            }
            else
            {
                currentTrackingObject.X++;
            }
        }

        private void MoveWest(PositionTracking currentTrackingObject)
        {
            if (currentTrackingObject.X - 1 < Configuration.Defaults.GridBottomXBoundary)
            {
                FallOutCheck(currentTrackingObject);
            }
            else
            {
                currentTrackingObject.X--;
            }
        }

        private void FallOutCheck(PositionTracking currentTrackingObject)
        {
            // Pickup the scent, discard this action
            if (FallOffGrid[
                currentTrackingObject.X,
                currentTrackingObject.Y,
                (int)currentTrackingObject.Heading] > 0)
            return;

            // Robot has fallen out of grid, leave a scent
            currentTrackingObject.IsFallout = true;

            FallOffGrid[
                currentTrackingObject.X,
                currentTrackingObject.Y,
                (int)currentTrackingObject.Heading]
                = FallOffGrid[
                    currentTrackingObject.X,
                currentTrackingObject.Y,
                (int)currentTrackingObject.Heading]++;
        }
        #endregion
    }

    internal class PositionTracking
    {
        internal int X { get; set; }
        internal int Y { get; set; }
        internal RobotHeading Heading { get; set; }
        internal bool IsFallout { get; set; }
    }
}