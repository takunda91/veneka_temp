using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veneka.Indigo.DesktopApp.Device.Printer.Zebra
{
    public enum ZebraStatuses
    {
        Initializing,
        Idle,
        Standby,
        Ready,
        InsertCard,
        CardReady,
        Canceling,
        AlarmHandling,
        Printing_ReceiveOk,
        Printing_ReceiveError,
        Printing_Parse,
        Printing_InProgress,
        Printing_DoneOk,
        Printing_DoneError,
        Printing_CancelledByUser,
        Printing_CancelledByError,
        Printing_CleaningUp,
        Magnetic_Encoding,
        Magnetic_Verifying,
        Magnetic_Reading,
        Magnetic_ReaderError,
        Magnetic_ReadError,
        Magnetic_ReadEinError,
        Magnetic_WriteError
    }



    public class ZebraPrinterStatuses
    {
        private static Dictionary<ZebraStatuses, string> _statusString = new Dictionary<ZebraStatuses, string>()
        {
            { ZebraStatuses.Initializing, "initializing" },
            { ZebraStatuses.Idle, "idle" },
            { ZebraStatuses.Standby, "standby" },
            { ZebraStatuses.Ready, "ready" },
            { ZebraStatuses.InsertCard, "insert_card" },
            { ZebraStatuses.CardReady, "card_ready" },
            { ZebraStatuses.Canceling, "canceling" },
            { ZebraStatuses.AlarmHandling, "alarm_handling" },
            { ZebraStatuses.Printing_ReceiveOk, "receive_ok" },
            { ZebraStatuses.Printing_ReceiveError, "receive_error" },
            { ZebraStatuses.Printing_Parse, "parsed" },
            { ZebraStatuses.Printing_InProgress, "in_progress" },
            { ZebraStatuses.Printing_DoneOk, "done_ok" },
            { ZebraStatuses.Printing_DoneError, "done_error" },
            { ZebraStatuses.Printing_CancelledByUser, "cancelled_by_user" },
            { ZebraStatuses.Printing_CancelledByError, "cancelled_by_error" },
            { ZebraStatuses.Printing_CleaningUp, "cleaning_up" },
            { ZebraStatuses.Magnetic_Encoding, "encoding" },
            { ZebraStatuses.Magnetic_Verifying, "verifying" },
            { ZebraStatuses.Magnetic_Reading, "reading" },
            { ZebraStatuses.Magnetic_ReaderError, "reader_error" },
            { ZebraStatuses.Magnetic_ReadError, "read_error" },
            { ZebraStatuses.Magnetic_ReadEinError, "read_ein_error" },
            { ZebraStatuses.Magnetic_WriteError, "write_error" },

        };

        public static bool IsStatus(ZebraStatuses zebraStatus, string statusReceived)
        {
            return _statusString[zebraStatus].Equals(statusReceived, StringComparison.OrdinalIgnoreCase);
        }

        public static string Initializing
        {
            get
            {
                return _statusString[ZebraStatuses.Initializing];
            }
        }

        public static string Idle
        {
            get
            {
                return _statusString[ZebraStatuses.Idle];
            }
        }

        public static string Standby
        {
            get
            {
                return _statusString[ZebraStatuses.Standby];
            }
        }

        public static string Ready
        {
            get
            {
                return _statusString[ZebraStatuses.Ready];
            }
        }

        public static string InsertCard
        {
            get
            {
                return _statusString[ZebraStatuses.InsertCard];
            }
        }

        public static string CardReady
        {
            get
            {
                return _statusString[ZebraStatuses.CardReady];
            }
        }

        public static string Canceling
        {
            get
            {
                return _statusString[ZebraStatuses.Canceling];
            }
        }

        public static string AlarmHandling
        {
            get
            {
                return _statusString[ZebraStatuses.AlarmHandling];
            }
        }

        public static string Printing_ReceiveOk
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_ReceiveOk];
            }
        }

        public static string Printing_ReceiveError
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_ReceiveError];
            }
        }

        public static string Printing_Parse
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_Parse];
            }
        }

        public static string Printing_InProgress
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_InProgress];
            }
        }

        public static string Printing_DoneOk
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_DoneOk];
            }
        }

        public static string Printing_DoneError
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_DoneError];
            }
        }

        public static string Printing_CancelledByUser
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_CancelledByUser];
            }
        }

        public static string Printing_CancelledByError
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_CancelledByError];
            }
        }

        public static string Printing_CleaningUp
        {
            get
            {
                return _statusString[ZebraStatuses.Printing_CleaningUp];
            }
        }

        public static string Magnetic_Encoding
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_Encoding];
            }
        }

        public static string Magnetic_Verifying
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_Verifying];
            }
        }

        public static string Magnetic_Reading
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_Reading];
            }
        }

        public static string Magnetic_ReaderError
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_ReaderError];
            }
        }

        public static string Magnetic_ReadError
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_ReadError];
            }
        }

        public static string Magnetic_ReadEinError
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_ReadEinError];
            }
        }

        public static string Magnetic_WriteError
        {
            get
            {
                return _statusString[ZebraStatuses.Magnetic_WriteError];
            }
        }
    }
}
