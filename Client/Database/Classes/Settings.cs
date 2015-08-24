using System;

namespace Client.Database {
    public class Settings {
        #region Declarations
        public String           GameName    { get; set; }
        public GraphicOptions   Graphics    = new GraphicOptions();
        public NetworkOptions   Network     = new NetworkOptions();
        #endregion

        #region Constructors
        public Settings() {
            this.GameName       = String.Empty;
        }
        #endregion
    }
    public class GraphicOptions {
        #region Declarations
        public Int32            ResolutionX { get; set; }
        public Int32            ResolutionY { get; set; }
        public Boolean          Fullscreen  { get; set; }
        public String           Theme       { get; set; }
        #endregion

        #region Constructors
        public GraphicOptions() {
            this.ResolutionX    = new Int32();
            this.ResolutionY    = new Int32();
            this.Fullscreen     = new Boolean();
        }
        #endregion
    }
    public class NetworkOptions {
        #region Declarations
        public String           IPAddress   { get; set; }
        public Int32            Port        { get; set; }
        #endregion

        #region Constructors
        public NetworkOptions() {
            this.IPAddress      = String.Empty;
            this.Port           = new Int32();
        }
        #endregion
    }
}
