﻿using CMD;

namespace SpritzGUI
{
    internal class PreRunTask
    {
        public readonly Options options;

        public PreRunTask(Options options)
        {
            this.options = options;
        }

        public string DisplayName { get; set; }
    }
}