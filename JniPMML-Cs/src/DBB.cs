/* Java >>> *
package com.WDataSci.JniPMML;

import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;

/* <<< Java */
/* C# >>> */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Runtime.CompilerServices;

using static com.WDataSci.WDS.Util;
using static com.WDataSci.WDS.JavaLikeExtensions;

namespace com.WDataSci.JniPMML
{


    /// <summary>
    /// DBB is short for Direct ByteBuffer, which on the Java side, is used as the internal data
    /// memory map which is being used to hand off data between C# and Java.
    ///  
    /// Despite the overhead on the both the C# and Java sides, the effective use is as a contiguous
    /// array of byte data.  The main block can be thought of as a rectangular region for a table of data
    /// where each column has the same width in bytes (variable length fields are a long's width pointing
    /// to variable space).  This is essentially the way HDF.PInvoke packs memory before bulk writing
    /// a compound dataset. Here, we add leading bytes for the essential layout and a block of space
    /// after the rectangular region with enough space for the variable length fields.
    /// 
    /// There are also three layouts:
    /// <ul>
    /// <li>WDSH</li> a header layout which includes a record for each column/member/field
    /// <li>WDSD</li> a record set layout associated with a header
    /// <li>WDSC</li> [TODO] a combined layout, with leading bytes, space for the header, and space for the record set
    /// </ul>
    ///
    /// The layout for the leading bytes is common across all three and takes 8*8=64 bytes:
    /// <table>
    /// <thead><tr>
    /// <th>  Start  </th><th>  Length   </th><th>   Purpose </th>
    /// </tr></thead>
    /// <tbody><tr>
    /// <td>     0        </td><td>   8            </td><td>   The layout style, 8 bytes for 4 UTF-16 characters for WDSH/WDSD/WDSC </td>
    /// </tr><tr>
    /// <td>     8        </td><td>   8            </td><td>   The total number of bytes required for this layout </td>
    /// </tr><tr>
    /// <td>     16        </td><td>   8            </td><td>   The number of leading bytes before the fixed length region begins </td>
    /// </tr><tr>
    /// <td>     24        </td><td>   8            </td><td>   The total number of bytes for the rectangular or fixed length region</td>
    /// </tr><tr>
    /// <td>     32        </td><td>   8            </td><td>   The total number of bytes for the variable length region</td>
    /// </tr><tr>
    /// <td>     40        </td><td>   8            </td><td>   The number of records (or rows) of data communicated</td>
    /// </tr><tr>
    /// <td>     48        </td><td>   8            </td><td>   The fixed number of bytes per record</td>
    /// </tr><tr>
    /// <td>     56        </td><td>   8            </td><td>   The maximum variable number of bytes each record may require</td>
    /// </tr></tbody>
    /// </table>
    ///      
    /// WDSH, the header layout needs to communicate just enough information to describe the core meta data for each
    /// column.  This includes its name, a possible second name (generally used to point to a 
    /// pre-mapped FieldName used in the PMML dictionary), and several additional meta data ints and longs.
    /// 40 fixed length bytes per column are for:
    /// <table>
    /// <thead><tr>
    /// <th>  Start  </th><th>  Length   </th><th>   Purpose </th>
    /// </tr></thead>
    /// <tbody><tr>
    /// <td>     0        </td><td>   8            </td><td>   Name variable length string pointer (long) </td>
    /// </tr><tr>
    /// <td>     8        </td><td>   8            </td><td>   Optional second name variable length string pointer (long), 0 if not used/mapped </td>
    /// </tr><tr>
    /// <td>     16      </td><td>    4       </td><td>    integer for data type code (see enum eDTyp)   </td>
    /// </tr><tr>
    /// <td>     20      </td><td>    8       </td><td>    long for number of bytes column data requires in the contiguous portion   
    ///                                                    (variable length fields will have no more than 8 for a pointer to the data)   </td>
    /// </tr><tr>
    /// <td>     28      </td><td>    8       </td><td>    long for maximum byte length of data   
    ///                                                    ( fixed length strings are packed directly, null padded,   
    ///                                                     variable length strings represent a long (IntPtr) to other memory)   </td>
    /// </tr><tr>
    /// <td>     36      </td><td>    4       </td><td>    (extra integer space, not used)   </td>
    /// </tr></tbody>
    /// </table>
    ///      
    /// For variable length strings, the rectangular space will hold the offset from the start of the leader bytes of the space holding the string. 
    /// For allocation purposes, each string could be expected to occupy 4+MaxByteLength.  When a string is greater than MaxByteLength,
    /// it will be trimmed to MaxByteLength-2 to null terminate, this may change, but any 2 (UTF-16) consecutive 0 bytes will be taken 
    /// as a null termination.
    /// 
    /// For further illustration, the WDSD layout including leading bytes is:
    /// <table>
    /// <thead><tr>
    /// <th>Block</th><th>Block Start</th><th>Block Length</th><th>Start</th><th>Length</th><th>Purpose</th>
    /// </tr></thead>
    /// <tbody><tr>
    /// <td>Header</td><td>0</td><td>LeadTotal=64</td><td>     0        </td><td>   8      </td><td>    UTF-16 bytes for WDSD </td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     8        </td><td>   8            </td><td>    LayoutTotal=LeadTotal+LayoutFLenTotal+LayoutVLenTotal </td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     16        </td><td>   8            </td><td>   LeadTotal</td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     24        </td><td>   8            </td><td>   LayoutFLenTotal=NRecords*RecordFLen</td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     32        </td><td>   8            </td><td>   LayoutVLenTotal=NRecords*RecordVLen</td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     40        </td><td>   8            </td><td>   NRecords</td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     48        </td><td>   8            </td><td>   RecordFLen</td>
    /// </tr><tr>
    /// <td>       </td><td> </td><td>     </td><td>     56        </td><td>   8            </td><td>   RecordVLen</td>
    /// </tr><tr>
    /// <td>FLen Region</td><td>LeadTotal</td><td>LayoutFLenTotal</td><td>     0        </td><td>   LayoutFLenTotal </td><td>     The densely populated rectangular region, RecordFLen bytes for each of the NRecords</td>
    /// <td>VLen Region</td><td>LeadTotal+LayoutVLenTotal</td><td>LayoutVLenTotal</td><td>     0        </td><td>   LayoutVLenTotal </td><td>   The region for variable length data, likely using less
    ///                                                                                                 than the total space.  Unless the total space required is calculated prior, the allocated space
    ///                                                                                                 needs to be NRecords*RecordVLen.</td>
    /// </tr></tbody>
    /// </table>
    /// 
    /// [TODO] For a WDSC layout, there is only one record, it's fixed length region is the space for the WDSH and the variable length region holds the corresponding WDSD.
    /// 
    /// </summary>

    /* <<< C# */
    public class DBB
    {

        //Direct ByteBuffer like data
        public byte[] data = null;
        /* Java >>> *
        //specific to Java, using Direct ByteBuffer wrap of this.data
        public ByteBuffer datawrap = null;
        /* <<< Java */
        Boolean bUsingByteBufferOnly = false;

        /* C# >>> */
        public byte[] datawrap = null; //just a place holder
        /* <<< C# */

        public Boolean bHasLeaders = false;
        public Boolean bHasFLenVLenSplit = false;
        public Boolean bIsReadOnly = false;

        public long offset = 0;
        public long Length = 0;
        public long flenoffset = 0;
        public long flenlength = 0;
        public long vlenoffset = 0;
        public long vlenlength = 0;

        //leader data
        public String LayoutStyle = null;
        public long nDBBRequiredBytes = 0;
        public long nDBBLeadingBytes = 0;
        public long nDBBFLenBytes = 0;
        public long nDBBVLenBytes = 0;
        public long nRecords = 0;
        public long nRecordFLenBytes = 0;
        public long nRecordVLenBytes = 0;


        public Boolean bIsBigEndian = true;
        public long ptr = 0;
        public long flenptr = 0;
        public long vlenptr = 0;

        public void Dispose()
        {
            this.data = null;
            this.datawrap = null;
        }

        /* >>> C# */
        public DBB() { }

        ~DBB()
        {
            this.Dispose();
        }
        /* <<< C# */

        public Boolean isValid()
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            /* Java >>> *
            if ( this.bUsingByteBufferOnly && this.data == null && !this.datawrap.isDirect() )
                throw new com.WDataSci.WDS.WDSException("Error, the wrapped ByteBuffer inside DBB must be direct");
            /* <<< Java */
            if ( this.bHasFLenVLenSplit && (this.Length < this.nDBBRequiredBytes) )
                throw new com.WDataSci.WDS.WDSException("Error, the wrapped Length inside DBB must be greater than nDDBRequiredBytes");
            if ( this.nDBBFLenBytes < this.nRecords * this.nRecordFLenBytes
                    || this.nDBBVLenBytes < this.nRecords * this.nRecordVLenBytes
                    || this.nDBBRequiredBytes < this.nDBBLeadingBytes + this.nDBBFLenBytes + this.nDBBVLenBytes )
                throw new com.WDataSci.WDS.WDSException("Error, the wrapped Length inside DBB must be greater than nDDBRequiredBytes");
            return true;
        }

        public void position(long ptr, long flenptr, long vlenptr)
        {
            this.ptr = ptr;
            this.flenptr = flenptr;
            this.vlenptr = vlenptr;
        }

        public void Reset()
        {
            this.data = null;
            this.datawrap = null;
            this.bUsingByteBufferOnly = false;
            this.bHasLeaders = false;
            this.bHasFLenVLenSplit = false;
            this.bIsReadOnly = false;

            this.offset = 0;
            this.Length = 0;
            this.flenoffset = 0;
            this.flenlength = 0;
            this.vlenoffset = 0;
            this.vlenlength = 0;

            this.LayoutStyle = null;
            this.nDBBRequiredBytes = 0;
            this.nDBBLeadingBytes = 0;
            this.nDBBFLenBytes = 0;
            this.nDBBVLenBytes = 0;
            this.nRecords = 0;
            this.nRecordFLenBytes = 0;
            this.nRecordVLenBytes = 0;

            this.bIsBigEndian = true;
            this.position(0, 0, 0);
        }

        private DBB __Wrap(ref byte[] arg)
        {
            this.data = arg;
            //Java this.datawrap = ByteBuffer.wrap(arg);
            //Java this.Length = arg.Length;
            //C#
            this.Length = arg.Length;
            return this;
        }


        public DBB Wrap(ref byte[] arg)
        {
            this.Reset();
            this.__Wrap(ref arg);
            return this;
        }


        /* Java >>> * 
        public DBB(ByteBuffer arg)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( arg.isDirect() && !arg.hasArray() ) {
                this.data = null;
                this.datawrap = arg;
                this.bUsingByteBufferOnly = true;
                this.offset = 0;
                this.Length = arg.capacity();
            }
            else if ( arg.hasArray() ) {
                this.datawrap = arg;
                this.data = arg.array();
                this.bUsingByteBufferOnly = false;
                this.offset = 0;
                this.Length = arg.capacity();
            }
            else
                throw new com.WDataSci.WDS.WDSException("Error DBB requires a Direct ByteBuffer with backing array");
        }
        /* <<< Java */

        public DBB(ref byte[] arg)
        {
            this.__Wrap(ref arg);
        }

        public DBB(DBB arg, Boolean bJustData)
        {
            this.data = arg.data;
            this.datawrap = arg.datawrap;
            this.bUsingByteBufferOnly = arg.bUsingByteBufferOnly;
            this.bIsReadOnly = arg.bIsReadOnly;
            this.offset = 0;
            this.Length = arg.Length;

            if ( bJustData ) {

                return;

            }
            else {

                this.bHasLeaders = arg.bHasLeaders;
                this.bHasFLenVLenSplit = arg.bHasFLenVLenSplit;

                this.flenoffset = arg.flenoffset;
                this.flenlength = arg.flenlength;
                this.vlenoffset = arg.vlenoffset;
                this.vlenlength = arg.vlenlength;

                this.LayoutStyle = arg.LayoutStyle;
                this.nDBBRequiredBytes = arg.nDBBRequiredBytes;
                this.nDBBLeadingBytes = arg.nDBBLeadingBytes;
                this.nDBBFLenBytes = arg.nDBBFLenBytes;
                this.nDBBVLenBytes = arg.nDBBVLenBytes;
                this.nRecords = arg.nRecords;
                this.nRecordFLenBytes = arg.nRecordFLenBytes;
                this.nRecordVLenBytes = arg.nRecordVLenBytes;

            }
        }


        public DBB(ref byte[] arg, int offset, Boolean bIsBigEndian)
        {
            this.__Wrap(ref arg);
            this.offset = offset;
            this.bIsBigEndian = bIsBigEndian;
        }

        public DBB(ref byte[] arg, int offset, int length, Boolean bIsBigEndian)
        {
            this.__Wrap(ref arg);
            this.offset = offset;
            this.Length = length;
            this.bIsBigEndian = bIsBigEndian;
        }

        public DBB cReadExistingLayout()
        //throws com.WDataSci.WDS.WDSException, Exception
        {
            this.isValid();
            this.position(0, 0, 0);
            this.LayoutStyle = this.GetLayerFLenString(0, 8);
            //formats using this leader set:
            // WDSH - Header layout
            // WDSD - RecordSet defined by WDSH
            // WDSC - Combined, the FLen region will contain a WDSH and the VLen region will contain a WDSD
            if ( !com.WDataSci.WDS.Util.bIn(LayoutStyle, "WDSC", "WDSH", "WDSD") )
                throw new com.WDataSci.WDS.WDSException("Error XDataBuffer LayoutStyle is not WDSH, WDSD, or WDSC");
            this.nDBBRequiredBytes = (long) this.GetLayerLong(0);
            this.nDBBLeadingBytes = (long) this.GetLayerLong(0);
            this.nDBBFLenBytes = (long) this.GetLayerLong(0);
            this.nDBBVLenBytes = (long) this.GetLayerLong(0);
            this.nRecords = (long) this.GetLayerLong(0);
            this.nRecordFLenBytes = (long) this.GetLayerLong(0);
            this.nRecordVLenBytes = (long) this.GetLayerLong(0);

            this.bHasFLenVLenSplit = true;
            this.flenoffset = this.offset + this.nDBBLeadingBytes;
            this.flenlength = this.nDBBFLenBytes;
            this.vlenoffset = this.flenoffset + this.nDBBFLenBytes;
            this.vlenlength = this.nDBBVLenBytes;

            this.position(0, 0, 0);

            return this;
        }

        /* Java >>> *
        public DBB cWrap(ByteBuffer arg)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( arg.isDirect() && !arg.hasArray() ) {
                this.data = null;
                this.datawrap = arg;
                this.bUsingByteBufferOnly = true;
            }
            else if ( arg.hasArray() ) {
                this.datawrap = arg;
                this.data = arg.array();
                this.bUsingByteBufferOnly = false;
            }
            else
                throw new com.WDataSci.WDS.WDSException("Error DBB requires a Direct ByteBuffer with backing array");
            this.offset = 0;
            this.bIsBigEndian = true;
            return this;
        }

        public DBB cWrap(ByteBuffer arg, int offset, Boolean bIsBigEndian)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( arg.isDirect() && !arg.hasArray() ) {
                this.data = null;
                this.datawrap = arg;
                this.bUsingByteBufferOnly = true;
            }
            else if ( arg.hasArray() ) {
                this.datawrap = arg;
                this.data = arg.array();
                this.bUsingByteBufferOnly = false;
            }
            else
                throw new com.WDataSci.WDS.WDSException("Error DBB requires a Direct ByteBuffer with backing array");
            this.data = arg.array();
            this.offset = offset;
            this.bIsBigEndian = bIsBigEndian;
            return this;
        }
        /* <<< Java */

        public DBB cWrap(ref byte[] arg, int offset, Boolean bIsBigEndian)
        {
            this.Reset();
            this.__Wrap(ref arg);
            this.offset = offset;
            this.bIsBigEndian = bIsBigEndian;
            return this;
        }

        public DBB cWrap(ref byte[] arg, int offset, int length, Boolean bIsBigEndian)
        {
            this.Reset();
            this.__Wrap(ref arg);
            this.offset = offset;
            this.Length = length;
            this.bIsBigEndian = bIsBigEndian;
            return this;
        }

        public DBB cAsReadOnly()
        {
            this.bIsReadOnly = true;
            return this;
        }

        public DBB cWithOffset(int offset)
        {
            this.offset = offset;
            return this;
        }

        public DBB cWithLength(int length)
        {
            this.Length = length;
            return this;
        }

        public DBB cAsBigEndian()
        {
            this.bIsBigEndian = true;
            return this;
        }

        public DBB cAsNotBigEndian()
        {
            this.bIsBigEndian = false;
            return this;
        }


        public DBB cAsSimple()
        //throws com.WDataSci.WDS.WDSException
        {
            this.LayoutStyle = "Simple";
            this.nRecords = 1;
            this.nRecordFLenBytes = this.Length;
            this.nRecordVLenBytes = 0;
            this.nDBBLeadingBytes = 0;
            this.nDBBFLenBytes = nRecords * nRecordFLenBytes;
            this.nDBBVLenBytes = 0;
            this.nDBBRequiredBytes = this.nDBBLeadingBytes + this.nDBBFLenBytes + this.nDBBVLenBytes;
            if ( this.nDBBRequiredBytes > this.Length )
                throw new com.WDataSci.WDS.WDSException("Error, not enough allocated space in internal byte[] for DBB");
            this.flenoffset = this.offset + this.nDBBLeadingBytes;
            this.flenlength = this.nDBBFLenBytes;
            this.vlenoffset = this.offset + this.nDBBLeadingBytes + this.nDBBFLenBytes;
            this.vlenlength = 0;
            return this;
        }


        public DBB cAsHDF5BulkCompoundDSWriteLayout(long nRecords, long nRecordFLenBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            this.LayoutStyle = "HDF5Bulk";
            this.nRecords = nRecords;
            this.nRecordFLenBytes = nRecordFLenBytes;
            this.nRecordVLenBytes = 0;
            this.nDBBLeadingBytes = 0;
            this.nDBBFLenBytes = nRecords * nRecordFLenBytes;
            this.nDBBVLenBytes = 0;
            this.nDBBRequiredBytes = this.nDBBLeadingBytes + this.nDBBFLenBytes + this.nDBBVLenBytes;
            if ( this.nDBBRequiredBytes > this.Length )
                throw new com.WDataSci.WDS.WDSException("Error, not enough allocated space in internal byte[] for DBB");

            this.flenoffset = this.offset + this.nDBBLeadingBytes;
            this.flenlength = this.nDBBFLenBytes;
            this.vlenoffset = this.offset + this.nDBBLeadingBytes + this.nDBBFLenBytes;
            this.vlenlength = 0;
            this.bIsBigEndian = false;
            return this;
        }

        public DBB cAsUsualLayout(String LayoutStyle, long nLeadingBytes, long nRecords, long nRecordFLenBytes, long nRecordVLenBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( nLeadingBytes < 64 )
                throw new com.WDataSci.WDS.WDSException("Error, AsUsualLayout requires at least 8*8 bytes for leading data");
            this.LayoutStyle = LayoutStyle;
            this.nRecords = nRecords;
            this.nRecordFLenBytes = nRecordFLenBytes;
            this.nRecordVLenBytes = nRecordVLenBytes;
            this.nDBBLeadingBytes = nLeadingBytes;
            this.nDBBFLenBytes = nRecords * nRecordFLenBytes;
            this.nDBBVLenBytes = nRecords * nRecordVLenBytes;
            this.nDBBRequiredBytes = this.nDBBLeadingBytes + this.nDBBFLenBytes + this.nDBBVLenBytes;
            if ( this.nDBBRequiredBytes > this.Length )
                throw new com.WDataSci.WDS.WDSException("Error, not enough allocated space in internal byte[] for DBB," + this.nDBBRequiredBytes + ", " + this.Length);

            this.flenoffset = this.offset + this.nDBBLeadingBytes;
            this.flenlength = this.nDBBFLenBytes;
            this.vlenoffset = this.offset + this.nDBBLeadingBytes + this.nDBBFLenBytes;
            this.vlenlength = this.nDBBVLenBytes;

            return this;
        }

        public DBB cAsUsualLayout(String LayoutStyle, long nRecords, long nRecordFLenBytes, long nRecordVLenBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            return this.cAsUsualLayout(LayoutStyle, 64, nRecords, nRecordFLenBytes, nRecordVLenBytes);
        }


        public Boolean isDirect()
        {
            //Java if ( this.bUsingByteBufferOnly ) return this.datawrap.isDirect();
            if ( this.data != null ) return true;
            else if ( this.data != null ) return true;
            return false;
        }

        private void __GetIndices(int layer, int blen, long atarg, long[] indexp)
        {
            switch ( layer ) {
                case 1:
                    if ( atarg >= 0 )
                        this.flenptr = atarg;
                    indexp[0] = this.flenoffset + this.flenptr;
                    this.flenptr += blen;
                    break;
                case 2:
                    if ( atarg >= 0 )
                        this.vlenptr = atarg;
                    indexp[0] = this.vlenoffset + this.vlenptr;
                    this.vlenptr += blen;
                    break;
                default:
                    if ( atarg >= 0 )
                        this.ptr = atarg;
                    indexp[0] = this.offset + this.ptr;
                    this.ptr += blen;
                    break;
            }
            indexp[1] = blen;
            indexp[2] = blen - 1;
        }


        private void __GetBytes(byte[] lv, long[] indexp, Boolean bIsBigEndian)
        {
            /* C# >>> */
            unsafe {
                fixed ( byte* lvp = &lv[0] ) {
                    if ( bIsBigEndian ) {
                        for ( long i = 0, j = indexp[2], k = indexp[0]; i < indexp[1]; i++, j--, k++ )
                            lv[j] = this.data[k];
                    }
                    else {
                        for ( long i = 0, k = indexp[0]; i < indexp[1]; i++, k++ ) lv[i] = this.data[k];
                    }
                }
            }
            /* <<< C# */
            /* Java >>> *
        if ( bUsingByteBufferOnly )
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) lv[i] = this.datawrap.get(k);
        else
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) lv[i] = this.data[k];
            /* <<< Java */
        }

        private void __PutBytes(byte[] lv, long[] indexp, Boolean bIsBigEndian)
        {
            /* C# >>> */
            unsafe {
                fixed ( byte* lvp = &lv[0] ) {
                    if ( bIsBigEndian ) {
                        for ( long i = 0, j = indexp[2], k = indexp[0]; i < indexp[1]; i++, j--, k++ ) this.data[k] = lv[j];
                    }
                    else {
                        for ( long i = 0, k = indexp[0]; i < indexp[1]; i++, k++ ) this.data[k] = lv[i];
                    }
                }
            }
            /* <<< C# */
            /* Java >>> *
        if ( bUsingByteBufferOnly )
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) this.datawrap.put(k, lv[i]);
        else
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) this.data[k] = lv[i];
            /* <<< Java */
        }

        public byte GetLayerByte(int layer)
        {
            byte rv;
            switch ( layer ) {
                case 1:
                    rv = this.data[(int) (this.flenoffset + this.flenptr)];
                    this.flenptr += 1;
                    return rv;
                case 2:
                    rv = this.data[(int) (this.vlenoffset + this.vlenptr)];
                    this.vlenptr += 1;
                    return rv;
                default:
                    rv = this.data[(int) (this.offset + this.ptr)];
                    this.ptr += 1;
                    return rv;
            }
        }

        public byte GetLayerByteAt(int layer, long arg)
        {
            byte rv;
            switch ( layer ) {
                case 1:
                    this.flenptr = arg;
                    rv = this.data[(int) (this.flenoffset + this.flenptr)];
                    this.flenptr += 1;
                    return rv;
                case 2:
                    this.vlenptr = arg;
                    rv = this.data[(int) (this.vlenoffset + this.vlenptr)];
                    this.vlenptr += 1;
                    return rv;
                default:
                    this.ptr = arg;
                    rv = this.data[(int) (this.offset + this.ptr)];
                    this.ptr += 1;
                    return rv;
            }
        }


        private int __GetLayerInt(int layer, long atarg)
        {
            int rv = 0;
            long[] indexp = new long[3];
            this.__GetIndices(layer, 4, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[4];
                fixed ( byte* lvp = &lv[0] ) {
                    this.__GetBytes(lv, indexp, this.bIsBigEndian);
                    rv = *(int*) ((int*) lvp);
                }
            }
            /* <<< C# */
            /* Java >>> *
            rv = this.datawrap.getInt((int) (indexp[0]));
            /* <<< Java */
            return rv;
        }

        /* Java >>> *
        public int GetLayerInt(int layer)
        {
            return this.__GetLayerInt(layer, -1);
        }

        public int GetLayerInt(int layer, long atarg)
        {
            return this.__GetLayerInt(layer, atarg);
        }
        /* <<< Java */
        /* C# >>> */
        public int? GetLayerInt(int layer)
        {
            int? rv=this.__GetLayerInt(layer, -1);
            if ( rv == int.MinValue ) return null;
            return rv;
        }

        public int? GetLayerInt(int layer, long atarg)
        {
            int? rv=this.__GetLayerInt(layer, atarg);
            if ( rv == int.MinValue ) return null;
            return rv;
        }
        /* <<< C# */

        private long __GetLayerLong(int layer, long atarg)
        {
            long rv = 0;
            long[] indexp = new long[3];
            this.__GetIndices(layer, 8, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[8];
                fixed ( byte* lvp = &lv[0] ) {
                    this.__GetBytes(lv, indexp, this.bIsBigEndian);
                    rv = *(long*) ((long*) lvp);
                }
            }
            /* <<< C# */
            /* Java >>> *
                rv = this.datawrap.getLong((int) (indexp[0]));
                /* <<< Java */
            return rv;
        }

        /* Java >>> *
        public long GetLayerLong(int layer)
        {
            return this.__GetLayerLong(layer, -1);
        }

        public long GetLayerLong(int layer, long atarg)
        {
            return this.__GetLayerLong(layer, atarg);
        }
        /* <<< Java */

        /* C# >>> */
        public long? GetLayerLong(int layer)
        {
            long? rv=this.__GetLayerLong(layer, -1);
            if ( rv == long.MinValue ) return null;
            return rv;
        }

        public long? GetLayerLong(int layer, long atarg)
        {
            long? rv= this.__GetLayerLong(layer, atarg);
            if ( rv == long.MinValue ) return null;
            return rv;
        }
        /* <<< C# */


        private double __GetLayerDouble(int layer, long atarg)
        {
            double rv = 0;
            long[] indexp = new long[3];
            this.__GetIndices(layer, 8, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[8];
                fixed ( byte* lvp = &lv[0] ) {
                    this.__GetBytes(lv, indexp, this.bIsBigEndian);
                    rv = *(double*) ((double*) lvp);
                }
            }
            /* <<< C# */
            /* Java >>> *
                rv = this.datawrap.getDouble((int) (indexp[0]));
                /* <<< Java */
            return rv;
        }

        /* Java >>> *
        public double GetLayerDouble(int layer)
        {
            return this.__GetLayerDouble(layer, -1);
        }

        public double GetLayerDouble(int layer, long atarg)
        {
            return this.__GetLayerDouble(layer, atarg);
        }
        /* <<< Java */

        /* C# >>> */
        public double? GetLayerDouble(int layer)
        {
            double rv= this.__GetLayerDouble(layer, -1);
            if ( Double.IsNaN(rv) || Double.IsPositiveInfinity(rv) || Double.IsNegativeInfinity(rv) 
                || rv == Double.MaxValue || rv == Double.MinValue) return null;
            return rv;
        }

        public double? GetLayerDouble(int layer, long atarg)
        {
            double rv= this.__GetLayerDouble(layer, atarg);
            if ( Double.IsNaN(rv) || Double.IsPositiveInfinity(rv) || Double.IsNegativeInfinity(rv) 
                || rv == Double.MaxValue || rv == Double.MinValue) return null;
            return rv;
        }
        /* <<< C# */


        private void __PutLayerInt(int layer, long atarg, int value)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, 4, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[4];
                fixed ( byte* lvp = &lv[0] ) {
                    *(int*) ((int*) lvp) = value;
                    this.__PutBytes(lv, indexp, this.bIsBigEndian);
                }
            }
            /* <<< C# */
            /* Java >>> *
                this.datawrap.putInt((int) (indexp[0]), value);
                /* <<< Java */
        }

        public void PutLayerInt(int layer, int value)
        {
            this.__PutLayerInt(layer, -1, value);
        }

        public void PutLayerInt(int layer, long atarg, int value)
        {
            this.__PutLayerInt(layer, atarg, value);
        }

        /* C# >>> */
        public void PutLayerInt(int layer, Object obj)
        {
            int value=int.MinValue;
            try {
                if (obj!=null)
                value = Convert.ToInt32(obj);
            }
            catch ( Exception ) {
                value = int.MinValue;
            }
            this.__PutLayerInt(layer, -1, value);
        }

        public void PutLayerInt(int layer, long atarg, Object obj)
        {
            int value=int.MinValue;
            try {
                if (obj!=null)
                value = Convert.ToInt32(obj);
            }
            catch ( Exception ) {
                value = int.MinValue;
            }
            this.__PutLayerInt(layer, atarg, value);
        }
        /* <<< C# */

        private void __PutLayerLong(int layer, long atarg, long value)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, 8, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[8];
                fixed ( byte* lvp = &lv[0] ) {
                    *(long*) ((long*) lvp) = value;
                    this.__PutBytes(lv, indexp, this.bIsBigEndian);
                }
            }
            /* <<< C# */
            /* Java >>> *
                this.datawrap.putLong((int) (indexp[0]), value);
            /* <<< Java */
        }

        public void PutLayerLong(int layer, long value)
        {
            this.__PutLayerLong(layer, -1, value);
        }

        public void PutLayerLong(int layer, long atarg, long value)
        {
            this.__PutLayerLong(layer, atarg, value);
        }

        /* C# >>> */
        public void PutLayerLong(int layer, Object obj)
        {
            long value=long.MinValue;
            try {
                if (obj!=null)
                value = Convert.ToInt64(obj);
            }
            catch ( Exception ) {
                value = long.MinValue;
            }
            this.__PutLayerLong(layer, -1, value);
        }

        public void PutLayerLong(int layer, long atarg, Object obj)
        {
            long value=long.MinValue;
            try {
                if (obj!=null)
                value = Convert.ToInt64(obj);
            }
            catch ( Exception ) {
                value = long.MinValue;
            }
            this.__PutLayerLong(layer, atarg, value);
        }
        /* <<< C# */

        private void __PutLayerDouble(int layer, long atarg, double value)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, 8, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[8];
                fixed ( byte* lvp = &lv[0] ) {
                    *(double*) ((double*) lvp) = value;
                    this.__PutBytes(lv, indexp, this.bIsBigEndian);
                }
            }
            /* <<< C# */
            /* Java >>> *
            this.datawrap.putDouble((int) (indexp[0]), value);
            /* <<< Java */
        }

        public void PutLayerDouble(int layer, double value)
        {
            this.__PutLayerDouble(layer, -1, value);
        }

        public void PutLayerDouble(int layer, long atarg, double value)
        {
            this.__PutLayerDouble(layer, atarg, value);
        }

        /* C# >>> */
        public void PutLayerDouble(int layer, Object obj)
        {
            double value=Double.NaN;
            try {
                if (obj!=null)
                value = Convert.ToDouble(obj);
            }
            catch ( Exception ) {
                value = Double.NaN;
            }
            this.__PutLayerDouble(layer, -1, value);
        }

        public void PutLayerDouble(int layer, long atarg, Object obj)
        {
            double value=Double.NaN;
            try {
                if (obj!=null)
                value = Convert.ToDouble(obj);
            }
            catch ( Exception ) {
                value = Double.NaN;
            }
            this.__PutLayerDouble(layer, atarg, value);
        }
        /* <<< C# */


        private void __PutLayerBytes(int layer, long atarg, byte[] value)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, value.Length, atarg, indexp);
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) this.data[k] = value[i];
        }

        public void PutLayerBytes(int layer, byte[] value)
        {
            this.__PutLayerBytes(layer, -1, value);
        }

        public void PutLayerBytes(int layer, long atarg, byte[] value)
        {
            this.__PutLayerBytes(layer, atarg, value);
        }

        private void __PutLayerZeros(int layer, long atarg, int value)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, value, atarg, indexp);
            for ( int i = 0, k = (int) indexp[0]; i < indexp[1]; i++, k++ ) this.data[k] = 0;
        }

        public void PutLayerZeros(int layer, int value)
        {
            this.__PutLayerZeros(layer, -1, value);
        }

        public void PutLayerZeros(int layer, long atarg, int value)
        {
            this.__PutLayerZeros(layer, atarg, value);
        }


        private String __GetLayerFLenString(int layer, long atarg, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            String rv = null;
            long[] indexp = new long[3];
            this.__GetIndices(layer, (int) nByteMaxLength, atarg, indexp);
            /* C# >>> */
            unsafe {
                byte[] lv=new byte[nByteMaxLength];
                fixed ( byte* lvp = &lv[0] ) {
                    this.__GetBytes(lv, indexp, false);
                }
                if ( this.bIsBigEndian ) {
                    byte[] lv2=Encoding.Convert(Encoding.BigEndianUnicode, Encoding.Default,lv,0,(int) nByteMaxLength);
                    rv = Encoding.Default.GetString(lv2);
                }
                else
                    rv = Encoding.Default.GetString(lv, 0, (int) nByteMaxLength);
            }
            /* <<< C# */
            /* Java >>> *
            try {
                byte[] lv = new byte[(int) (nByteMaxLength)];
                int lvlen = (int) nByteMaxLength;
                this.datawrap.position((int) (indexp[0]));
                for ( int k = 0; k < nByteMaxLength; k++ ) {
                    lv[k] = this.datawrap.get();
                    if ( k > 1 && lv[k - 2] == 0 && lv[k - 1] == 0 ) {
                        lvlen = k - 2;
                        break;
                    }
                }
                if ( lvlen > 0 )
                    rv = new String(lv, 0, lvlen, StandardCharsets.UTF_16BE);
                else
                    rv = "";
            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in extracting VLen String from ByteBuffer:", e);
            }
            /* <<< Java */
            return rv;
        }

        public String GetLayerFLenString(int layer, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            return this.__GetLayerFLenString(layer, -1, nByteMaxLength);
        }

        public String GetLayerFLenString(int layer, long atarg, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            return this.__GetLayerFLenString(layer, atarg, nByteMaxLength);
        }

        private String __GetLayerVLenString(int layer, long atarg, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( layer != 1 )
                throw new com.WDataSci.WDS.WDSException("Error, GetLayerVLenString can only be called from layer 1 (which points to space in layer 2)");
            long vlenptr = (long) this.GetLayerLong(layer, atarg);
            if ( vlenptr <= 0 || vlenptr > this.Length ) return "";
            /* Java >>> *
            int slen = this.GetLayerInt(2, vlenptr - this.vlenoffset);
            if ( slen <= 0 ) return "";
            return this.__GetLayerFLenString(2, -1, slen);
            /* <<< Java */
            /* C# >>> */
            int? slen = this.GetLayerInt(2, vlenptr - this.vlenoffset);
            if ( slen == null || slen <= 0 ) return "";
            return this.__GetLayerFLenString(2, -1, slen.Value);
            /* <<< C# */
        }

        public String GetLayerVLenString(int layer, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            return this.__GetLayerVLenString(layer, -1, nByteMaxLength);
        }

        public String GetLayerVLenString(int layer, long atarg, long nByteMaxLength)
        //throws com.WDataSci.WDS.WDSException
        {
            return this.__GetLayerVLenString(layer, atarg, nByteMaxLength);
        }

        private int __PutCheckStringLength(String value, int nByteMaxLength, int nZeroBytes)
        {
            //check and trim to less then nByteMaxLength, allowing 2 for 0 terminal
            int sl = value.Length;
            /* C# >>> */
            int l = Encoding.BigEndianUnicode.GetByteCount(value);
            while ( sl > 1 && l > nByteMaxLength - nZeroBytes ) {
                sl--;
                if ( this.bIsBigEndian )
                    l = Encoding.BigEndianUnicode.GetByteCount(value.Substring(0, sl - 1));
                else
                    l = Encoding.Default.GetByteCount(value.Substring(0, sl - 1));
            }
            if ( sl == value.Length ) return -1;
            else return sl;
            /* <<< C# */
            /* Java >>> *
            byte[] sb = value.getBytes(StandardCharsets.UTF_16BE);
            int l = sb.length;
            while ( sl > 1 && l > nByteMaxLength - nZeroBytes ) {
                sl--;
                sb = value.substring(0, sl - 1).getBytes(StandardCharsets.UTF_16BE);
                l = sb.length;
            }
            return sl;
            /* <<< Java */
        }

        private void __PutLayerFLenStringWithoutSizeCheck(int layer, long atarg, String value, int nByteMaxLength, int nZeroBytes)
        {
            long[] indexp = new long[3];
            this.__GetIndices(layer, (int) nByteMaxLength, atarg, indexp);
            byte[] bvalue = null;
            /* C# >>> */
            if ( this.bIsBigEndian )
                bvalue = Encoding.Convert(Encoding.Default, Encoding.BigEndianUnicode, Encoding.Default.GetBytes(value));
            else
                bvalue = Encoding.Default.GetBytes(value);
            /* <<< C# */
            /* Java >>> *
            bvalue = value.getBytes(StandardCharsets.UTF_16BE);
            /* <<< Java */
            this.__PutBytes(bvalue, indexp, false);
            if ( bvalue.Length + nZeroBytes > nByteMaxLength ) nZeroBytes = nByteMaxLength - bvalue.Length;
            if ( nZeroBytes > 0 && nZeroBytes > 1 )
                this.__PutLayerZeros(layer, -1, nZeroBytes);
        }

        private void __PutLayerFLenString(int layer, long atarg, String value, int nByteMaxLength, int nZeroBytes)
        {
            int sl = this.__PutCheckStringLength(value, nByteMaxLength, nZeroBytes);
            if ( sl > 0 ) value = value.substring(0, sl);
            this.__PutLayerFLenStringWithoutSizeCheck(layer, atarg, value, nByteMaxLength, nZeroBytes);
        }

        public void PutLayerFLenString(int layer, String value, int nByteMaxLength, int nZeroBytes)
        {
            this.__PutLayerFLenString(layer, -1, value, nByteMaxLength, nZeroBytes);
        }

        public void PutLayerFLenString(int layer, long atarg, String value, int nByteMaxLength, int nZeroBytes)
        {
            this.__PutLayerFLenString(layer, atarg, value, nByteMaxLength, nZeroBytes);
        }


        private void __PutLayerVLenString(int layer, long atarg, String value, int nByteMaxLength, int nZeroBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            if ( layer != 1 )
                throw new com.WDataSci.WDS.WDSException("Error, PutLayerVLenString can only be called from layer 1 (which points to space in layer 2)");
            if ( value == null || value.isEmpty() ) {
                this.__PutLayerLong(layer, atarg, 0);
                return;
            }
            try {

                int sl = this.__PutCheckStringLength(value, nByteMaxLength, nZeroBytes);
                if ( sl > 0 )
                    value = value.substring(0, sl);
                byte[] bvalue = null;
                /* C# >>> */
                if ( this.bIsBigEndian )
                    bvalue = Encoding.Convert(Encoding.Default, Encoding.BigEndianUnicode, Encoding.Default.GetBytes(value));
                else
                    bvalue = Encoding.Default.GetBytes(value);
                /* <<< C# */
                /* Java >>> *
                bvalue = value.getBytes(StandardCharsets.UTF_16BE);
                /* <<< Java */
                long[] indexp = new long[3];
                this.__PutLayerLong(layer, atarg, this.vlenoffset + this.vlenptr);
                this.__PutLayerInt(2, -1, bvalue.Length);
                this.__GetIndices(2, bvalue.Length, -1, indexp);
                this.__PutBytes(bvalue, indexp, false);

            }
            catch ( Exception e ) {
                throw new com.WDataSci.WDS.WDSException("Error in putVLenString ", e);
            }
        }

        public void PutLayerVLenString(int layer, String value, int nByteMaxLength, int nZeroBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            this.__PutLayerVLenString(layer, -1, value, nByteMaxLength, nZeroBytes);
        }

        public void PutLayerVLenString(int layer, long atarg, String value, int nByteMaxLength, int nZeroBytes)
        //throws com.WDataSci.WDS.WDSException
        {
            this.__PutLayerVLenString(layer, atarg, value, nByteMaxLength, nZeroBytes);
        }

        public static class Default
        {
            //Java public final static long nLeadingBytes = 8 * 8;
            //C#
            public static long nLeadingBytes = 8 * 8;
        }

    }

    /* C# >>> */
}
/* <<< C# */
