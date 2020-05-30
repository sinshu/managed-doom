using System;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class AnimationInfo
    {
        private AnimationType type;
        private int period;
        private int count;
        private int x;
        private int y;
        private int data;

        public AnimationInfo(AnimationType type, int period, int count, int x, int y)
        {
            this.type = type;
            this.period = period;
            this.count = count;
            this.x = x;
            this.y = y;
        }

        public AnimationInfo(AnimationType type, int period, int count, int x, int y, int data)
        {
            this.type = type;
            this.period = period;
            this.count = count;
            this.x = x;
            this.y = y;
            this.data = data;
        }

        public AnimationType Type => type;
        public int Period => period;
        public int Count => count;
        public int X => x;
        public int Y => y;
        public int Data => data;

        public static readonly IReadOnlyList<IReadOnlyList<AnimationInfo>> Episodes = new AnimationInfo[][]
        {
            new AnimationInfo[]
            {
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 224, 104),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 184, 160),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 112, 136),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 72, 112),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 88, 96),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 64, 48),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 192, 40),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 136, 16),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 80, 16),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 64, 24)
            },

            new AnimationInfo[]
            {
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 1),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 2),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 3),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 4),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 5),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 6),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 7),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 3, 192, 144, 8),
                new AnimationInfo(AnimationType.Level, GameConstants.TicRate / 3, 1, 128, 136, 8)
            },

            new AnimationInfo[]
            {
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 104, 168),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 40, 136),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 160, 96),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 104, 80),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 3, 3, 120, 32),
                new AnimationInfo(AnimationType.Always, GameConstants.TicRate / 4, 3, 40, 0)
            }
        };
    }
}
