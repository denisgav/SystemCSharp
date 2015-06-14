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

/*****************************************************************************

  sc_bit_ids.h -- Report ids for the datatypes/bit code.

  Original Author: Martin Janssen, Synopsys, Inc., 2002-01-17

 *****************************************************************************/

/*****************************************************************************

  MODIFICATION LOG - modifiers, enter your name, affiliation, date and
  changes you are making here.

      Name, Affiliation, Date:
  Description of Modification:
    
 *****************************************************************************/

namespace sc_dt
{
    public static class sc_bit_ids
    {
        public const string SC_ID_LENGTH_MISMATCH_ = "length mismatch in bit/logic vector assignment";
        public const string SC_ID_INCOMPATIBLE_TYPES_ = "incompatible types";
        public const string SC_ID_CANNOT_CONVERT_ = "cannot perform conversion";
        public const string SC_ID_INCOMPATIBLE_VECTORS_ = "incompatible vectors";
        public const string SC_ID_VALUE_NOT_VALID_ = "value is not valid";
        public const string SC_ID_ZERO_LENGTH_ = "zero length";
        public const string SC_ID_VECTOR_CONTAINS_LOGIC_VALUE_ = "vector contains 4-value logic";
        public const string SC_ID_SC_BV_CANNOT_CONTAIN_X_AND_Z_ = "sc_bv cannot contain values X and Z";
        public const string SC_ID_VECTOR_TOO_LONG_ = "vector is too long: truncated";
        public const string SC_ID_VECTOR_TOO_SHORT_ = "vector is too short: 0-padded";
        public const string SC_ID_WRONG_VALUE_ = "wrong value";
        public const string SC_ID_LOGIC_Z_TO_BOOL_ = "sc_logic value 'Z' cannot be converted to bool";
        public const string SC_ID_LOGIC_X_TO_BOOL_ = "sc_logic value 'X' cannot be converted to bool";
    }
}