﻿using System;

namespace Extensions {
    public static class Enumerations {

        public enum Vitals {
            HP,
            MP,
            Vital_Count
        }

        public enum Stats {
            Strength,
            Endurance,
            Intelligence,
            Agility,
            Willpower,
            Stat_Count
        }

        public enum Gender {
            Male,
            Female
        }

        public enum Direction {
            Up,
            Down,
            Left,
            Right,
            Direction_Count
        }

        public enum Ranks {
            None,
            Developer,
            Administrator
        }

        public enum MessageType {
            Map,
            World,
            System,
            Error,
            Emote
        }
    }
}
