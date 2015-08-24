using System;


namespace Server.Database {
    public class TempPlayer {
        #region Declarations
        public Boolean InGame { get; set; }
        #endregion

        #region Constructors
        public TempPlayer() {
            this.InGame = false;
        }
        #endregion
    }
}
