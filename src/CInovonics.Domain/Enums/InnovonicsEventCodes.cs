using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CInovonics.Domain.Enums
{
    public enum InnovonicsEventCodes
    {
        //Endpoint Event Codes
        Unknown,
        Alarm1,
        Alarm1Cleared,
        Alarm2,
        Alarm2Cleared,
        Alarm3,
        Alarm3Cleared,
        Alarm4,
        Alarm4Cleared,
        DeviceInactive,
        DeviceInactiveCleared,
        TamperActivated,
        TamperActivatedCleared,
        EOLTamperActivated,
        EOLTamperActivatedCleared,
        LowBattery,
        LowBatteryCleared,
        MaintenanceRequired,
        MaintenanceRequiredCleared,
        DeviceReset,
        EndpointConfigurationFail,
        EndpointonfigurationSuccess,

        // Repeater Event Codes
        RepeaterPowerLoss,
        RepeaterPowerLossCleared,
        RepeaterReset,
        RepeaterTamper,
        RepeaterTamperClear,
        RepeaterLowBattery,
        RepeaterLowBatterClear,
        RepeaterJam,
        RepeaterJamClear,
        RepeaterInactive,
        RepeaterInactiveClear,
        RepeaterConfigurationFail,
        RepeaterConfigurationSuccess,

        //Systm Event Codes
        ACGReset,
        ACGTamper,
        ACGTamperClear,
        ACGJammed,
        ACGJamClear,
        ACGInactive,
        ACGInactiveClear,
        ACGConfigurationFail,
        ACGConfigurationSuccess,
        ACGCRCCheckFail = 61,
        ACGFirmwareUpdateSuccess = 71,
        ACGFirmwareUpdateFail = 72,
        ACGBatteryFailed = 91,
        ACGBatteryLow = 92,
        ACGBatteryOK = 93,
        ACGBatteryNotInstalled = 94,
        ACGShutdownImminent = 96,
        ACGFirmwareUpgradePending = 97,
        ACGIPProcessorCRCInvalid = 99,
        ACGRebootRequested = 100,
        ACGFileSystemFailure1 = 101,
        ACGFileSystemFailure2 = 109,
        FileSystemReset1 = 111,
        FileSystem2 = 113,
        FileSystem3 = 114,
        FileTransalationSuccess = 112,
        DHCPAddressRenewed = 116,
        DHCPDown = 118,
        IPAddressExpired = 119,
        ACGUnexpectedReset = 124,
        ACGHello = 125,
        IPClientDisconnected = 126,
        StaticIP = 127,
        TimeServerUnreachable = 128,
        NORFMessages = 131,
        TimeAdjustedNTP = 132,
        LicenseInvalid = 135
    }
}
