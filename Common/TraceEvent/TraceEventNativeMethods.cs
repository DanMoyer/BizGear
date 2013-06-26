//     Copyright (c) Microsoft Corporation.  All rights reserved.
// This file is best viewed using outline mode (Ctrl-M Ctrl-O)
//
// This program uses code hyperlinks available as part of the HyperAddin Visual Studio plug-in.
// It is available from http://www.codeplex.com/hyperAddin 
// 
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security;
using System.Diagnostics;
using Diagnostics.Tracing.Parsers;
using System.Collections.Generic;

// This moduleFile contains Internal PINVOKE declarations and has no public API surface. 
namespace Diagnostics.Tracing.Parsers
{
    // TODO use safehandles. 
    #region Private Classes

    /// <summary>
    /// TraceEventNativeMethods contains the PINVOKE declarations needed
    /// to get at the Win32 TraceEvent infrastructure.  It is effectively
    /// a port of evntrace.h to C# declarations.  
    /// </summary>
    // [SecuritySafeCritical]
    // TODO FIX NOW making this public is dubious (did it for enabling Debug Privilege for attaching profiler)
    public unsafe static class TraceEventNativeMethods
    {
        #region TimeZone type from winbase.h

        /// <summary>
        ///	Time zone info.  Used as one field of TRACE_EVENT_LOGFILE, below.
        ///	Total struct size is 0xac.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 0xac, CharSet = CharSet.Unicode)]
        internal struct TIME_ZONE_INFORMATION
        {
            public uint bias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string standardName;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
            public UInt16[] standardDate;
            public uint standardBias;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string daylightName;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
            public UInt16[] daylightDate;
            public uint daylightBias;
        }

        #endregion TimeZone type from winbase.h

        #region ETW tracing types from evntrace.h

        //	Delegates for use with ETW EVENT_TRACE_LOGFILEW struct.
        //	These are the callbacks that ETW will call while processing a moduleFile
        //	so that we can process each line of the trace moduleFile.
        internal delegate bool EventTraceBufferCallback(
            [In] IntPtr logfile); // Really a EVENT_TRACE_LOGFILEW, but more efficient to marshal manually);
        internal delegate void EventTraceEventCallback(
            [In] EVENT_RECORD* rawData);

        internal const ulong INVALID_HANDLE_VALUE = unchecked((ulong)(-1));

        internal const uint EVENT_TRACE_REAL_TIME_MODE = 0x00000100;

        //  EVENT_TRACE_LOGFILE.LogFileMode should be set to PROCESS_TRACE_MODE_EVENT_RECORD 
        //  to consume events using EventRecordCallback
        internal const uint PROCESS_TRACE_MODE_EVENT_RECORD = 0x10000000;
        internal const uint PROCESS_TRACE_MODE_REAL_TIME = 0x00000100;
        internal const uint PROCESS_TRACE_MODE_RAW_TIMESTAMP = 0x00001000;

        internal const uint EVENT_TRACE_FILE_MODE_NONE = 0x00000000;
        internal const uint EVENT_TRACE_FILE_MODE_SEQUENTIAL = 0x00000001;
        internal const uint EVENT_TRACE_FILE_MODE_CIRCULAR = 0x00000002;
        internal const uint EVENT_TRACE_FILE_MODE_APPEND = 0x00000004;
        internal const uint EVENT_TRACE_FILE_MODE_NEWFILE = 0x00000008;


        internal const uint EVENT_TRACE_CONTROL_QUERY = 0;
        internal const uint EVENT_TRACE_CONTROL_STOP = 1;
        internal const uint EVENT_TRACE_CONTROL_UPDATE = 2;
        internal const uint EVENT_TRACE_CONTROL_FLUSH = 3;

        internal const uint WNODE_FLAG_TRACED_GUID = 0x00020000;
        internal const uint EVENT_TRACE_SYSTEM_LOGGER_MODE = 0x02000000;

        /// <summary>
        /// EventTraceHeader structure used by EVENT_TRACE_PROPERTIES
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct WNODE_HEADER
        {
            public UInt32 BufferSize;
            public UInt32 ProviderId;
            public UInt64 HistoricalContext;
            public UInt64 TimeStamp;
            public Guid Guid;
            public UInt32 ClientContext;  // Determines the time stamp resolution
            public UInt32 Flags;
        }

        /// <summary>
        /// EVENT_TRACE_PROPERTIES is a structure used by StartTrace, ControlTrace
        /// however it can not be used directly in the defintion of these functions
        /// because extra information has to be hung off the end of the structure
        /// before beinng passed.  (LofFileNameOffset, LoggerNameOffset)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_TRACE_PROPERTIES
        {
            public WNODE_HEADER Wnode;      // Timer Resolution determined by the Wnode.ClientContext.  
            public UInt32 BufferSize;
            public UInt32 MinimumBuffers;
            public UInt32 MaximumBuffers;
            public UInt32 MaximumFileSize;
            public UInt32 LogFileMode;
            public UInt32 FlushTimer;
            public UInt32 EnableFlags;
            public Int32 AgeLimit;
            public UInt32 NumberOfBuffers;
            public UInt32 FreeBuffers;
            public UInt32 EventsLost;
            public UInt32 BuffersWritten;
            public UInt32 LogBuffersLost;
            public UInt32 RealTimeBuffersLost;
            public IntPtr LoggerThreadId;
            public UInt32 LogFileNameOffset;
            public UInt32 LoggerNameOffset;
        }

        //	TraceMessage flags
        //	These flags are overlaid into the node USHORT in the EVENT_TRACE.header.version field.
        //	These items are packed in order in the packet (MofBuffer), as indicated by the flags.
        //	I don't know what PerfTimestamp is (size?) or holds.
        internal enum TraceMessageFlags : int
        {
            Sequence = 0x01,
            Guid = 0x02,
            ComponentId = 0x04,
            Timestamp = 0x08,
            PerformanceTimestamp = 0x10,
            SystemInfo = 0x20,
            FlagMask = 0xffff,
        }

        /// <summary>
        ///	EventTraceHeader and structure used to defined EVENT_TRACE (the main packet)
        ///	I have simplified from the original struct definitions.  I have
        ///	omitted alternate union-fields which we don't use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_TRACE_HEADER
        {
            public ushort Size;
            public ushort FieldTypeFlags;	// holds our MarkerFlags too
            public byte Type;
            public byte Level;
            public ushort Version;
            public int ThreadId;
            public int ProcessId;
            public long TimeStamp;          // Offset 0x10 
            public Guid Guid;
            //	no access to GuidPtr, union'd with guid field
            //	no access to ClientContext & MatchAnyKeywords, ProcessorTime, 
            //	union'd with kernelTime,userTime
            public int KernelTime;         // Offset 0x28
            public int UserTime;
        }

        /// <summary>
        /// EVENT_TRACE is the structure that represents a single 'packet'
        /// of data repesenting a single event.  
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_TRACE
        {
            public EVENT_TRACE_HEADER Header;
            public uint InstanceId;
            public uint ParentInstanceId;
            public Guid ParentGuid;
            public IntPtr MofData; // PVOID
            public int MofLength;
            public ETW_BUFFER_CONTEXT BufferContext;
        }

        /// <summary>
        /// TRACE_LOGFILE_HEADER is a header used to define EVENT_TRACE_LOGFILEW.
        ///	Total struct size is 0x110.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct TRACE_LOGFILE_HEADER
        {
            public uint BufferSize;
            public uint Version;            // This is for the operating system it was collected on.  Major, Minor, SubVerMajor, subVerMinor
            public uint ProviderVersion;
            public uint NumberOfProcessors;
            public long EndTime;            // 0x10
            public uint TimerResolution;
            public uint MaximumFileSize;
            public uint LogFileMode;        // 0x20
            public uint BuffersWritten;
            public uint StartBuffers;
            public uint PointerSize;
            public uint EventsLost;         // 0x30
            public uint CpuSpeedInMHz;
            public IntPtr LoggerName;	// string, but not CoTaskMemAlloc'd
            public IntPtr LogFileName;	// string, but not CoTaskMemAlloc'd
            public TIME_ZONE_INFORMATION TimeZone;   // 0x40         0xac size
            public long BootTime;
            public long PerfFreq;
            public long StartTime;
            public uint ReservedFlags;
            public uint BuffersLost;        // 0x10C?        
        }

        /// <summary>
        ///	EVENT_TRACE_LOGFILEW Main struct passed to OpenTrace() to be filled in.
        /// It represents the collection of ETW events as a whole.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct EVENT_TRACE_LOGFILEW
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string LogFileName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string LoggerName;
            public Int64 CurrentTime;
            public uint BuffersRead;
            public uint LogFileMode;
            // EVENT_TRACE for the current event.  Nulled-out when we are opening files.
            // [FieldOffset(0x18)] 
            public EVENT_TRACE CurrentEvent;
            // [FieldOffset(0x70)]
            public TRACE_LOGFILE_HEADER LogfileHeader;
            // callback before each buffer is read
            // [FieldOffset(0x180)]
            public EventTraceBufferCallback BufferCallback;
            public Int32 BufferSize;
            public Int32 Filled;
            public Int32 EventsLost;
            // callback for every 'event', each line of the trace moduleFile
            // [FieldOffset(0x190)]
            public EventTraceEventCallback EventCallback;
            public Int32 IsKernelTrace;     // TRUE for kernel logfile
            public IntPtr Context;	        // reserved for internal use
        }
        #endregion // ETW tracing types

        #region Win8 ETW Support - Windows 8

        internal enum TRACE_INFO_CLASS
        {
            TraceGuidQueryList,
            TraceGuidQueryInfo,
            TraceGuidQueryProcess,
            TraceStackTracingInfo,                  // This is the last one supported on Win7
            // Win 8 
            TraceSystemTraceEnableFlagsInfo,        // Turns on kernel event logger
            TraceSampledProfileIntervalInfo,        // TRACE_PROFILE_INTERVAL (allows you to set the sampling interval) (Set, Get)

            TraceProfileSourceConfigInfo,           // int array, turns on all listed sources.  (Set)
            TraceProfileSourceListInfo,             // PROFILE_SOURCE_INFO linked list (converts names to source numbers) (Get)

            // Used to collect extra info on other events (currently only context switch).  
            TracePmcEventListInfo,                  // CLASSIC_EVENT_ID array (Works like TraceStackTracingInfo)
            TracePmcCounterListInfo,                // int array
            MaxTraceSetInfoClass
        };

        internal struct CLASSIC_EVENT_ID
        {
            public Guid EventGuid;
            public byte Type;
            public fixed byte Reserved[7];
        };

        internal struct TRACE_PROFILE_INTERVAL       // Used for TraceSampledProfileIntervalInfo
        {
            public int Source;
            public int Interval;
        };

        internal struct PROFILE_SOURCE_INFO
        {
            public int NextEntryOffset;             // relative to the start of this structure, 0 indicates end.  
            public int Source;
            public int MinInterval;
            public int MaxInterval;
            public ulong Reserved;
            // char Description[ANYSIZE_ARRAY]; 
        };

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static internal extern int TraceSetInformation(
            [In] UInt64 traceHandle,
            [In] TRACE_INFO_CLASS InformationClass,
            [In] void* TraceInformation,
            [In] int InformationLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        static internal extern int TraceQueryInformation(
            [In] UInt64 traceHandle,
            [In] TRACE_INFO_CLASS InformationClass,
            [Out] void* TraceInformation,
            [In] int InformationLength,
            [In][Out] ref int ReturnLength);

        #endregion

        #region ETW tracing types from evntcons.h

        /*
        ntcons.h:#define EVENT_HEADER_FLAG_EXTENDED_INFO         0x0001
        ntcons.h:#define EVENT_HEADER_FLAG_PRIVATE_SESSION       0x0002
        ntcons.h:#define EVENT_HEADER_FLAG_STRING_ONLY           0x0004
        ntcons.h:#define EVENT_HEADER_FLAG_TRACE_MESSAGE         0x0008
        ntcons.h:#define EVENT_HEADER_FLAG_NO_CPUTIME            0x0010
        */

        internal const ushort EVENT_HEADER_FLAG_STRING_ONLY = 0x0004;
        internal const ushort EVENT_HEADER_FLAG_32_BIT_HEADER = 0x0020;
        internal const ushort EVENT_HEADER_FLAG_64_BIT_HEADER = 0x0040;
        internal const ushort EVENT_HEADER_FLAG_CLASSIC_HEADER = 0x0100;

        /// <summary>
        ///	EventTraceHeader and structure used to define EVENT_TRACE_LOGFILE (the main packet on Vista and above)
        ///	I have simplified from the original struct definitions.  I have
        ///	omitted alternate union-fields which we don't use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_HEADER
        {
            public ushort Size;
            public ushort HeaderType;
            public ushort Flags;            // offset: 0x4
            public ushort EventProperty;
            public int ThreadId;            // offset: 0x8
            public int ProcessId;           // offset: 0xc
            public long TimeStamp;          // offset: 0x10
            public Guid ProviderId;         // offset: 0x18
            public ushort Id;               // offset: 0x28
            public byte Version;            // offset: 0x2a
            public byte Channel;
            public byte Level;              // offset: 0x2c
            public byte Opcode;
            public ushort Task;
            public ulong Keyword;
            public int KernelTime;         // offset: 0x38
            public int UserTime;           // offset: 0x3C
            public Guid ActivityId;
        }

        /// <summary>
        ///	Provides context information about the event
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct ETW_BUFFER_CONTEXT
        {
            public byte ProcessorNumber;
            public byte Alignment;
            public ushort LoggerId;
        }

        /// <summary>
        ///	Defines the layout of an event that ETW delivers
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_RECORD
        {
            public EVENT_HEADER EventHeader;            //  size: 80
            public ETW_BUFFER_CONTEXT BufferContext;    //  size: 4
            public ushort ExtendedDataCount;
            public ushort UserDataLength;               //  offset: 86
            public EVENT_HEADER_EXTENDED_DATA_ITEM* ExtendedData;
            public IntPtr UserData;
            public IntPtr UserContext;
        }

        // Values for the ExtType field 
        internal const ushort EVENT_HEADER_EXT_TYPE_RELATED_ACTIVITYID = 0x0001;
        internal const ushort EVENT_HEADER_EXT_TYPE_SID = 0x0002;
        internal const ushort EVENT_HEADER_EXT_TYPE_TS_ID = 0x0003;
        internal const ushort EVENT_HEADER_EXT_TYPE_INSTANCE_INFO = 0x0004;
        internal const ushort EVENT_HEADER_EXT_TYPE_STACK_TRACE32 = 0x0005;
        internal const ushort EVENT_HEADER_EXT_TYPE_STACK_TRACE64 = 0x0006;

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_HEADER_EXTENDED_DATA_ITEM
        {
            public ushort Reserved1;
            public ushort ExtType;
            public ushort Reserved2;
            public ushort DataSize;
            public ulong DataPtr;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_EXTENDED_ITEM_STACK_TRACE32
        {
            public ulong MatchId;
            public fixed uint Address[1];       // Actually variable size
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct EVENT_EXTENDED_ITEM_STACK_TRACE64
        {
            public ulong MatchId;
            public fixed ulong Address[1];       // Actually variable size
        };

        [StructLayout(LayoutKind.Explicit)]
        unsafe internal struct EVENT_FILTER_DESCRIPTOR
        {
            [FieldOffset(0)]
            public byte* Ptr;          // Data
            [FieldOffset(8)]
            public int Size;
            [FieldOffset(12)]
            public int Type;
        };

        #endregion

        #region ETW tracing functions
        //	TRACEHANDLE handle type is a ULONG64 in evntrace.h.  Use UInt64 here.
        [DllImport("advapi32.dll",
            EntryPoint = "OpenTraceW",
            CharSet = CharSet.Unicode,
            SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static UInt64 OpenTrace(
            [In][Out] ref EVENT_TRACE_LOGFILEW logfile);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int ProcessTrace(
            [In] UInt64[] handleArray,
            [In] uint handleCount,
            [In] IntPtr StartTime,
            [In] IntPtr EndTime);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int CloseTrace(
            [In] UInt64 traceHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int QueryAllTraces(
            [In] IntPtr propertyArray,
            [In] int propertyArrayCount,
            [In][Out] ref int sessionCount);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int StartTraceW(
            [Out] out UInt64 sessionHandle,
            [In] string sessionName,
            EVENT_TRACE_PROPERTIES* properties);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int EnableTrace(
            [In] uint enable,
            [In] int enableFlag,
            [In] int enableLevel,
            [In] ref Guid controlGuid,
            [In] ulong sessionHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int EnableTraceEx(
            [In] ref Guid ProviderId,
            [In] Guid* SourceId,
            [In] ulong TraceHandle,
            [In] int IsEnabled,
            [In] byte Level,
            [In] ulong MatchAnyKeyword,
            [In] ulong MatchAllKeyword,
            [In] uint EnableProperty,
            [In] EVENT_FILTER_DESCRIPTOR* filterData);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int EnableTraceEx2(
            [In] ulong TraceHandle,
            [In] ref Guid ProviderId,
            [In] uint ControlCode,          // See EVENT_CONTROL_CODE_*
            [In] byte Level,
            [In] ulong MatchAnyKeyword,
            [In] ulong MatchAllKeyword,
            [In] uint Timeout,
            [In] ref ENABLE_TRACE_PARAMETERS EnableParameters);

        // Values for ENABLE_TRACE_PARAMETERS.Version
        internal const uint ENABLE_TRACE_PARAMETERS_VERSION = 1;

        // Values for ENABLE_TRACE_PARAMETERS.EnableProperty
        internal const uint EVENT_ENABLE_PROPERTY_SID = 0x00000001;
        internal const uint EVENT_ENABLE_PROPERTY_TS_ID = 0x00000002;
        internal const uint EVENT_ENABLE_PROPERTY_STACK_TRACE = 0x00000004;

        internal const uint EVENT_CONTROL_CODE_DISABLE_PROVIDER = 0;
        internal const uint EVENT_CONTROL_CODE_ENABLE_PROVIDER = 1;
        internal const uint EVENT_CONTROL_CODE_CAPTURE_STATE = 2;

        [StructLayout(LayoutKind.Sequential)]
        internal struct ENABLE_TRACE_PARAMETERS
        {
            public uint Version;
            public uint EnableProperty;
            public uint ControlFlags;
            public Guid SourceId;
            public EVENT_FILTER_DESCRIPTOR* EnableFilterDesc;
        };

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int ControlTrace(
            ulong sessionHandle,
            string sessionName,
            EVENT_TRACE_PROPERTIES* properties,
            uint controlCode);

        #endregion // ETW tracing functions

        #region ETW Tracing from KernelTraceControl.h (from XPERF distribution)

        /// <summary>
        /// Used in code:StartKernelTrace to indicate the kernel events that should have stack traces
        /// collected for them.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct STACK_TRACING_EVENT_ID
        {
            public Guid EventGuid;
            public byte Type;
            byte Reserved1;
            byte Reserved2;
            byte Reserved3;
            byte Reserved4;
            byte Reserved5;
            byte Reserved6;
            byte Reserved7;
        }

        [DllImport("KernelTraceControl.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int StartKernelTrace(
            out UInt64 TraceHandle,
            EVENT_TRACE_PROPERTIES* Properties,
            STACK_TRACING_EVENT_ID* StackTracingEventIds,       // Actually an array of  code:STACK_TRACING_EVENT_ID
            int cStackTracingEventIds);

        [DllImport("KernelTraceControl.dll", CharSet = CharSet.Unicode), SuppressUnmanagedCodeSecurityAttribute]
        internal extern static int CreateMergedTraceFile(
            string wszMergedFileName,
            string[] wszTraceFileNames,
            int cTraceFileNames,
            EVENT_TRACE_MERGE_EXTENDED_DATA dwExtendedDataFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);

        // Flags to save extended information to the ETW trace file
        [Flags]
        public enum EVENT_TRACE_MERGE_EXTENDED_DATA
        {
            NONE = 0x00,
            IMAGEID = 0x01,
            BUILDINFO = 0x02,
            VOLUME_MAPPING = 0x04,
            WINSAT = 0x08,
            EVENT_METADATA = 0x10,
        }
        #endregion

        #region Security Entry Points

        internal static uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        internal static uint STANDARD_RIGHTS_READ = 0x00020000;
        internal static uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        internal static uint TOKEN_DUPLICATE = 0x0002;
        internal static uint TOKEN_IMPERSONATE = 0x0004;
        internal static uint TOKEN_QUERY = 0x0008;
        internal static uint TOKEN_QUERY_SOURCE = 0x0010;
        internal static uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        internal static uint TOKEN_ADJUST_GROUPS = 0x0040;
        internal static uint TOKEN_ADJUST_SESSIONID = 0x0100;
        internal static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);


        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull = 2,
            TokenElevationTypeLimited = 3
        }

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups = 2,
            TokenPrivileges = 3,
            TokenOwner = 4,
            TokenPrimaryGroup = 5,
            TokenDefaultDacl = 6,
            TokenSource = 7,
            TokenType = 8,
            TokenImpersonationLevel = 9,
            TokenStatistics = 10,
            TokenRestrictedSids = 11,
            TokenSessionId = 12,
            TokenGroupsAndPrivileges = 13,
            TokenSessionReference = 14,
            TokenSandBoxInert = 15,
            TokenAuditPolicy = 16,
            TokenOrigin = 17,
            TokenElevationType = 18,
            TokenLinkedToken = 19,
            TokenElevation = 20,
            TokenHasRestrictions = 21,
            TokenAccessInformation = 22,
            TokenVirtualizationAllowed = 23,
            TokenVirtualizationEnabled = 24,
            TokenIntegrityLevel = 25,
            TokenUIAccess = 26,
            TokenMandatoryPolicy = 27,
            TokenLogonSid = 28,
            MaxTokenInfoClass = 29  // MaxTokenInfoClass should always be the last enum
        }

        [DllImport("advapi32.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken(
            [In] IntPtr ProcessHandle,
            [In] UInt32 DesiredAccess,
            [Out] out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            int TokenInformationLength,
            out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustTokenPrivileges(
           [In] IntPtr TokenHandle,
           [In, MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
           [In] ref TOKEN_PRIVILEGES NewState,
           [In] UInt32 BufferLength,
            // [Out] out TOKEN_PRIVILEGES PreviousState,
           [In] IntPtr NullParam,
           [In] IntPtr ReturnLength);

        // I explicitly DONT capture GetLastError information on this call because it is often used to
        // clean up and it is cleaner if GetLastError still points at the orginal error, and not the failure
        // in CloseHandle.  If we ever care about exact errors of CloseHandle, we can make another entry
        // point 
        [DllImport("kernel32.dll"), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern bool CloseHandle([In] IntPtr hHandle);

        [StructLayout(LayoutKind.Sequential)]
        internal struct TOKEN_PRIVILEGES      // taylored for the case where you only have 1. 
        {
            public UInt32 PrivilegeCount;
            public LUID Luid;
            public UInt32 Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LUID
        {
            public UInt32 LowPart;
            public Int32 HighPart;
        }

        // Constants for the Attributes field
        internal const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
        internal const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
        internal const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
        internal const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;

        // Constants for the Luid field 
        internal const uint SE_SYSTEM_PROFILE_PRIVILEGE = 11;
        internal const uint SE_DEBUG_PRIVILEGE = 20;

        #endregion

        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern void ZeroMemory(IntPtr handle, uint length);

        // TODO what is this for?
        internal static int GetHRForLastWin32Error()
        {
            int dwLastError = Marshal.GetLastWin32Error();
            if ((dwLastError & 0x80000000) == 0x80000000)
                return dwLastError;
            else
                return (dwLastError & 0x0000FFFF) | unchecked((int)0x80070000);
        }

        /// <summary>
        /// The Sample based profiling requires the SystemProfilePrivilege, This code turns it on.   
        /// </summary>
        internal static void SetSystemProfilePrivilege()
        {
            SetPrivilege(SE_SYSTEM_PROFILE_PRIVILEGE);
        }

        // TODO bit of a hack to make this public.  
        public static void SetDebugPrivilege()
        {
            SetPrivilege(SE_DEBUG_PRIVILEGE);
        }

        private static void SetPrivilege(uint privilege)
        {
            Process process = Process.GetCurrentProcess();
            IntPtr tokenHandle = IntPtr.Zero;
            bool success = OpenProcessToken(process.Handle, TOKEN_ADJUST_PRIVILEGES, out tokenHandle);
            if (!success)
                throw new Win32Exception();
            GC.KeepAlive(process);                      // TODO get on SafeHandles. 

            TOKEN_PRIVILEGES privileges = new TOKEN_PRIVILEGES();
            privileges.PrivilegeCount = 1;
            privileges.Luid.LowPart = privilege;
            privileges.Attributes = SE_PRIVILEGE_ENABLED;

            success = AdjustTokenPrivileges(tokenHandle, false, ref privileges, 0, IntPtr.Zero, IntPtr.Zero);
            CloseHandle(tokenHandle);
            if (!success)
                throw new Win32Exception();
        }

        public static bool? IsElevated()
        {
            if (Environment.OSVersion.Version.Major < 6)
                return true;

            Process process = Process.GetCurrentProcess();
            IntPtr tokenHandle = IntPtr.Zero;
            if (!OpenProcessToken(process.Handle, TOKEN_QUERY, out tokenHandle))
                return null;

            int tokenIsElevated = 0;
            int retSize;
            bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevation, (IntPtr)(&tokenIsElevated), 4, out retSize);
            CloseHandle(tokenHandle);
            if (!success)
                return null;

            GC.KeepAlive(process);                      // TODO get on SafeHandles. 
            return tokenIsElevated != 0;
        }

        // TODO why do we need this? 
        internal static int GetHRFromWin32(int dwErr)
        {
            return (int)((0 != dwErr) ? (0x80070000 | ((uint)dwErr & 0xffff)) : 0);
        }

        internal static bool CanSetCpuSamplingRate() { return false; }
        internal static bool SetCpuSamplingRate(int interval100ns) { return false; }
        internal static void EnableStackCaching(ulong traceHandle) { }

        internal struct TRACE_PROVIDER_INFO
        {
            public Guid ProviderGuid;
            public int SchemaSource;
            public int ProviderNameOffset;
        }

        internal struct PROVIDER_ENUMERATION_INFO
        {
            public int NumberOfProviders;
            public int Padding;
            // TRACE_PROVIDER_INFO TraceProviderInfoArray[ANYSIZE];
        };

        [DllImport("tdh.dll"), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int TdhEnumerateProviders(
            PROVIDER_ENUMERATION_INFO* pBuffer,
            ref int pBufferSize
        );

        internal enum TRACE_QUERY_INFO_CLASS
        {
            TraceGuidQueryList,
            TraceGuidQueryInfo,
            TraceGuidQueryProcess,
            TraceStackTracingInfo,
            MaxTraceSetInfoClass
        };

        internal struct TRACE_GUID_INFO
        {
            public int InstanceCount;
            public int Reserved;
        };

        internal struct TRACE_PROVIDER_INSTANCE_INFO
        {
            public int NextOffset;
            public int EnableCount;
            public int Pid;
            public int Flags;
        };

        internal struct TRACE_ENABLE_INFO
        {
            public int IsEnabled;
            public byte Level;
            public byte Reserved1;
            public ushort LoggerId;
            public ushort EnableProperty;
            public int Reserved2;
            public long MatchAnyKeyword;
            public long MatchAllKeyword;
        };

        [DllImport("advapi32.dll"), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int EnumerateTraceGuidsEx(
        TRACE_QUERY_INFO_CLASS TraceQueryInfoClass,
            void* InBuffer,
            int InBufferSize,
            void* OutBuffer,
            int OutBufferSize,
            ref int ReturnLength);

        internal enum EVENT_FIELD_TYPE
        {
            EventKeywordInformation = 0,
            EventLevelInformation = 1,
            EventChannelInformation = 2,
            EventTaskInformation = 3,
            EventOpcodeInformation = 4,
            EventInformationMax = 5,
        };

        internal struct PROVIDER_FIELD_INFOARRAY
        {
            public int NumberOfElements;
            public EVENT_FIELD_TYPE FieldType;
            // PROVIDER_FIELD_INFO FieldInfoArray[ANYSIZE_ARRAY];
        };

        internal struct PROVIDER_FIELD_INFO
        {
            public int NameOffset;
            public int DescriptionOffset;
            public long Value;
        };

        [DllImport("tdh.dll"), SuppressUnmanagedCodeSecurityAttribute]
        internal static extern int TdhEnumerateProviderFieldInformation(
            ref Guid guid,
            EVENT_FIELD_TYPE EventFieldType,
            PROVIDER_FIELD_INFOARRAY* pBuffer,
            ref int pBufferSize
        );

    } // end class
    #endregion
}
