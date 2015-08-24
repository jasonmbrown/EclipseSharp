using System;
using System.Collections.Generic;
using Extensions;

namespace Server.Database {
    public class Class {

        #region Declares
        public String           Name            { get; set; }
        public Int32[]          Statistic       = new Int32[(Int32)Enumerations.Stats.Stat_Count];
        public Int32            MaleSprite      { get; set; }
        public Int32            FemaleSprite    { get; set; }
        #endregion

        #region Constructors
        public Class() {
            this.Name           = String.Empty;
            this.MaleSprite     = new Int32();
            this.FemaleSprite   = new Int32();
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

}
