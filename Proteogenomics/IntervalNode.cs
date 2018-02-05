﻿using System.Collections.Generic;
using System.Linq;

namespace Proteogenomics
{
    public class IntervalNode
    {

        #region Public Properties

        public long Center;

        public IntervalNode LeftNode { get; set; }

        public IntervalNode RightNode { get; set; }

        public List<Interval> IntervalsCenter { get; set; } = new List<Interval>();

        #endregion Public Properties

        #region Constructors

        public IntervalNode()
        {

        }

        public IntervalNode(List<Interval> intervals)
        {
            Build(intervals);
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Build an interval tree
        /// </summary>
        /// <param name="intervals"></param>
        public void Build(List<Interval> intervals)
        {
            if (intervals.Count == 0)
            {
                Center = 0;
                return;
            }

            // Calculate median point
            Center = Interval.GetMedian(intervals);

            // Split intervals to the left, right, and intersecting "center"
            List<Interval> left = new List<Interval>();
            List<Interval> right = new List<Interval>();
            List<Interval> intersecting = new List<Interval>();

            foreach (Interval interval in intervals)
            {
                if (interval.OneBasedEnd < Center) left.Add(interval);
                if (interval.OneBasedStart > Center) right.Add(interval);
                else intersecting.Add(interval);
            }

            IntervalsCenter = intersecting;

            // Recurse
            if (left.Count > 0) LeftNode = new IntervalNode(left);
            if (right.Count > 0) RightNode = new IntervalNode(right);
        }

        /// <summary>
        /// Perform an interval intersection query on the node
        /// </summary>
        /// <param name="queryInterval"></param>
        /// <returns></returns>
        public List<Interval> Query(Interval queryInterval)
        {
            List<Interval> results = IntervalsCenter.Where(i => i.Overlaps(queryInterval)).ToList();
            if (queryInterval.OneBasedStart < Center && LeftNode != null) results.AddRange(LeftNode.Query(queryInterval));
            if (queryInterval.OneBasedStart > Center && RightNode != null) results.AddRange(RightNode.Query(queryInterval));
            return results;
        }

        /// <summary>
        /// Perform a stabbing query on the node
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public List<Interval> Stab(long point)
        {
            List<Interval> results = IntervalsCenter.Where(i => i.Overlaps(point)).ToList();
            if (point < Center && LeftNode != null) results.AddRange(LeftNode.Stab(point));
            if (point > Center && RightNode != null) results.AddRange(RightNode.Stab(point));
            return results;
        }

        #endregion Public Methods

    }
}
