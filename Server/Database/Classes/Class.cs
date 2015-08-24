using System;
using System.Collections.Generic;
using Extensions;

namespace Server.Database {
    public class Class {

        #region Declares
        public String           Name            { get; set; }
        public Int32[]          Statistic       = new Int32[(Int32)Enumerations.Stats.Stat_Count];
        public List<Int32>      MaleSprite      = new List<Int32>();
        public List<Int32>      FemaleSprite    = new List<Int32>();
        #endregion

        #region Constructors
        public Class() {
            this.Name           = String.Empty;
        }
        #endregion

        #region Methods
        public Int32 GetMaxVital(Enumerations.Vitals vital) {
            switch (vital) {

                case Enumerations.Vitals.HP:
                    return 100 + (this.Statistic[(Int32)Enumerations.Stats.Endurance] * 5) + 2;

                case Enumerations.Vitals.MP:
                    return 30 + (this.Statistic[(Int32)Enumerations.Stats.Intelligence] * 10) + 2;

            }
            return 0;
        }
        #endregion
    }

    public class StartItem {

        #region Declares
        public Int32            Id              { get; set; }
        public Int32            Amount          { get; set; }
        #endregion

        #region Constructors
        public StartItem() {
            this.Id             = new Int32();
            this.Amount         = new Int32();
        }
        #endregion
    }
}
