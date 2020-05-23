using System;

namespace ManagedDoom
{
    public class ToggleMenuItem : MenuItem
    {
        private string name;
        private int itemX;
        private int itemY;

        private string[] states;
        private int stateX;

        private int stateNumber;

        public ToggleMenuItem(
            string name,
            int skullX, int skullY,
            int itemX, int itemY,
            string state1, string state2,
            int stateX,
            int firstState)
            : base(skullX, skullY, null)
        {
            this.name = name;
            this.itemX = itemX;
            this.itemY = itemY;

            this.states = new[] { state1, state2 };
            this.stateX = stateX;

            stateNumber = firstState;
        }

        public string Name => name;
        public int ItemX => itemX;
        public int ItemY => itemY;

        public string State => states[stateNumber];
        public int StateX => stateX;
    }
}
