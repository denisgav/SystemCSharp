/*****************************************************************************

  The following code is derived, directly or indirectly, from the SystemC
  source code Copyright (c) 1996-2014 by all Contributors.
  All Rights reserved.

  The contents of this file are subject to the restrictions and limitations
  set forth in the SystemC Open Source License (the "License");
  You may not use this file except in compliance with such restrictions and
  limitations. You may obtain instructions on how to receive a copy of the
  License at http://www.accellera.org/. Software distributed by Contributors
  under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF
  ANY KIND, either express or implied. See the License for the specific
  language governing rights and limitations under the License.

 *****************************************************************************/


using System.Collections.Generic;

namespace sc_core
{
    // ----------------------------------------------------------------------------
    //  ENUM : sc_numrep
    //
    //  Enumeration of number representations for character string conversion.
    // ----------------------------------------------------------------------------
    public enum sc_numrep
    {
        SC_NOBASE = 0,
        SC_BIN = 2,
        SC_OCT = 8,
        SC_DEC = 10,
        SC_HEX = 16,
        SC_BIN_US,
        SC_BIN_SM,
        SC_OCT_US,
        SC_OCT_SM,
        SC_HEX_US,
        SC_HEX_SM,
        SC_CSD
    };


    public static class sc_constants
    {
        // Sign of a number:
        public const int SC_NEG = -1;     // Negative number
        public const int SC_ZERO = 0;     // Zero
        public const int SC_POS = 1;     // Positive number
        public const int SC_NOSIGN = 2;     // Uninitialized sc_signed number

        public const int SC_DIGIT_SIZE = BITS_PER_BYTE * sizeof(uint);

        public const uint SC_DIGIT_ZERO = (uint)0;
        public const uint SC_DIGIT_ONE = (uint)1;
        public const uint SC_DIGIT_TWO = (uint)2;

        // Attributes of a byte.
        public const int BITS_PER_BYTE = 8;
        public const int BYTE_RADIX = 256;
        public const int BYTE_MASK = 255;

        // LOG2_BITS_PER_BYTE = log2(BITS_PER_BYTE), assuming that
        // BITS_PER_BYTE is a power of 2.
        public const int LOG2_BITS_PER_BYTE = 3;

        // Attributes of the unsigned long. These definitions are used mainly in
        // the functions that are aware of the internal representation of
        // digits, e.g., get/set_packed_rep().
        public const int BYTES_PER_DIGIT_TYPE = 4;
        public const int BITS_PER_DIGIT_TYPE = 32;

        // Attributes of a digit, i.e., unsigned long less the overflow bits.
        public const int BYTES_PER_DIGIT = 4;
        public const int BITS_PER_DIGIT = 30;
        public const ulong DIGIT_RADIX = (1ul << BITS_PER_DIGIT);
        public const ulong DIGIT_MASK = (DIGIT_RADIX - 1);
        // Make sure that BYTES_PER_DIGIT = ceil(BITS_PER_DIGIT / BITS_PER_BYTE).

        // Similar attributes for the half of a digit. Note that
        // HALF_DIGIT_RADIX is equal to the square root of DIGIT_RADIX. These
        // definitions are used mainly in the multiplication routines.
        public const int BITS_PER_HALF_DIGIT = (BITS_PER_DIGIT / 2);
        public const ulong HALF_DIGIT_RADIX = (1ul << BITS_PER_HALF_DIGIT);
        public const ulong HALF_DIGIT_MASK = (HALF_DIGIT_RADIX - 1);

        // Bits per ...
        // will be deleted in the future. Use numeric_limits instead
        public const int BITS_PER_CHAR = 8;
        public const int BITS_PER_INT = (sizeof(int) * BITS_PER_CHAR);
        public const int BITS_PER_LONG = (sizeof(long) * BITS_PER_CHAR);
        public const int BITS_PER_INT64 = (sizeof(long) * BITS_PER_CHAR);
        public const int BITS_PER_UINT = (sizeof(uint) * BITS_PER_CHAR);
        public const int BITS_PER_ULONG = (sizeof(ulong) * BITS_PER_CHAR);
        public const int BITS_PER_UINT64 = (sizeof(ulong) * BITS_PER_CHAR);

        // Digits per ...
        public const int DIGITS_PER_CHAR = 1;
        public const int DIGITS_PER_INT = ((BITS_PER_INT + 29) / 30);
        public const int DIGITS_PER_LONG = ((BITS_PER_LONG + 29) / 30);
        public const int DIGITS_PER_INT64 = ((BITS_PER_INT64 + 29) / 30);
        public const int DIGITS_PER_UINT = ((BITS_PER_UINT + 29) / 30);
        public const int DIGITS_PER_ULONG = ((BITS_PER_ULONG + 29) / 30);
        public const int DIGITS_PER_UINT64 = ((BITS_PER_UINT64 + 29) / 30);

        public const string SC_BOOST_COMPILER = "Unknown ISO C++ Compiler";
        public const string SC_BOOST_STDLIB = "Unknown ISO standard library";
        public const int _WIN32_WINNT = 0x0400;
        public const int SC_INTWIDTH = 64;
        public const string SC_KERNEL_EVENT_PREFIX = "$$$$kernel_event$$$$_";
        public const int SC_HAS_PHASE_CALLBACKS_ = 1;
        public const string MSGNL = "\n             ";
        public const string CODENL = "\n  ";
        public const int QUICKTHREADS_STKALIGN = 64;
        public const int QUICKTHREADS_O7 = 16;
        public const int QUICKTHREADS_I6 = 14;
        public const int QUICKTHREADS_I5 = 13;
        public const int QUICKTHREADS_I4 = 12;
        public const int QUICKTHREADS_I3 = 11;
        public const int QUICKTHREADS_I2 = 10;
        public const int QUICKTHREADS_I1 = 9;
        public const int QUICKTHREADS_RPC = 14;
        public const int QUICKTHREADS_POP0 = 13;
        public const int QUICKTHREADS_PC = 12;
        public const int QUICKTHREADS_POP1 = 11;
        public const int QUICKTHREADS_RBP = 10;
        public const int QUICKTHREADS_R12 = 8;
        public const int QUICKTHREADS_R13 = 7;
        public const int QUICKTHREADS_R14 = 6;
        public const int QUICKTHREADS_R15 = 5;
        public const int QUICKTHREADS_RBX = 4;
        public const int QUICKTHREADS_RCX = 3;
        public const int QUICKTHREADS_RDX = 2;
        public const int QUICKTHREADS_RDI = 1;
        public const int QUICKTHREADS_RSI = 0;
        public const int QUICKTHREADS_EBX = 0;
        public const int QUICKTHREADS_EDI = 1;
        public const int QUICKTHREADS_ESI = 2;
        public const int QUICKTHREADS_EBP = 3;
        public const int QUICKTHREADS_POP2 = 6;
        public const int QUICKTHREADS_POPE = 8;
        public const int QUICKTHREADS_ARG0 = 9;
        public const int QUICKTHREADS_ARG1 = 10;
        public const int QUICKTHREADS_ARG2 = 11;
        public const int PPC_STACK_INCR = 16;
        public const int PPC_LINKAGE_AREA = 24;
        public const int PPC_CR_SAVE = 4;
        public const int PPC_LR_SAVE = 8;
        public const int QUICKTHREADS_BLOCKI_CR_SAVE = 8;
        public const int QUICKTHREADS_ARGS_MD = 0;
        public const int SYSTEMC_VERSION = 20140417;
        public const string SC_VERSION_ORIGINATOR = "Accellera";
        public const int SC_VERSION_MAJOR = 2;
        public const int SC_VERSION_MINOR = 3;
        public const int SC_VERSION_PATCH = 1;
        public const int SC_IS_PRERELEASE = 0;
        public const string SC_VERSION_RELEASE_DATE = "SYSTEMC_VERSION";
        public const string SC_VERSION_PRERELEASE = "pub_rev";
        public const int SC_BOOST_MPL_CFG_GCC = 0;
        public const int SC_SIMCONTEXT_TRACING_ = 1;
        public const int SC_DEFAULT_STACK_SIZE_ = 0x50000;
        public const string PRIu64 = "I64u";
        public const int SC_MAXTIME_ALLOWED_ = 1;
        public const int SC_DISABLE_COPYRIGHT_MESSAGE = 0;
        public const int SC_MAX_NUM_DELTA_CYCLES = 10000;



    }
}