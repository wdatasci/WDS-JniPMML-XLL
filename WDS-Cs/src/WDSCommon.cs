using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExcelDna.Integration;
using ExcelDna.Registration;
using ExcelDna.IntelliSense;

namespace com.WDataSci.WDS
{
    public class WDSCommon
    {


        [ExcelFunction(
            Name = "CrossProductEnumeration"
            , Category = "WDS"
            , Description = "Returns an array of all combinations of inputs"
            , IsThreadSafe = true
            , IsVolatile = false
            , ExplicitRegistration = true
            //, HelpTopic = "CrossProductEnumeration HelpTopic"
            //, IsClusterSafe = true
            //, IsHidden = false
            //, IsMacroType = false
            //, SuppressOverwriteError = false
            )
            ]
        public static object[,] CrossProductEnumeration(
                [ExcelArgument(Name ="Directive" ,Description = "0 for Count-Row-Values-Indices, 1 for Count")]
                Double _pDirective
                ,
                [ExcelArgument(Name = "Input" , Description = "Set of values for an enumeration dimension, add as many as needed")]
                params object [] args
            )
        {
            int nargs = args.Length;
            long? pDirective = (long?) _pDirective;

            object[,] lrv = new object[1, 2 * nargs + 2]; //used for pDirective==1

            if ( pDirective is null || pDirective < 0 || pDirective > 1 || nargs == 0 || args[0] is ExcelMissing ) {
                lrv[0, 0] = -1;
                return lrv;
            }

            int[] ndims = new int[nargs];
            int[] rollers = new int[nargs];
            object[][] data = new object[nargs][];


            int ndim = 0, nelem = 0, nrows = 0, ncols = 0;

            int i = 0, j = 0, k = 0, l = 0;
            int ntotal = 1;


            for ( i = 0 ; i < nargs ; i++ ) {
                lDimensions(ref ndim, ref nelem, ref nrows, ref ncols, args[i]);
                if ( pDirective == 1 ) {
                    lrv[0, 2 + i] = nelem;
                    lrv[0, 2 + nargs + i] = nelem;
                }
                else {
                    data[i] = new object[nelem];
                    j = -1;
                    if ( args[i] is object[,] ) {
                        for ( k = 0 ; k < nrows ; k++ )
                            for ( l = 0 ; l < ncols ; l++ ) {
                                j++;
                                data[i][j] = ((object[,]) args[i])[k, l];
                            }
                    }
                    else if ( args[i] is object[] ) {
                        for ( k = 0 ; k < nrows ; k++ ) {
                            j++;
                            data[i][j] = ((object[]) args[i])[k];
                        }
                    }
                    else if ( nelem == 1 )
                        data[i][0] = args[i];
                }

                ntotal *= nelem;
                ndims[i] = nelem;
                rollers[i] = 0;
            }

            if ( pDirective == 1 ) {
                lrv[0, 0] = ntotal;
                lrv[0, 1] = ntotal;
                return lrv;
            }

            rollers[nargs - 1] = -1;

            object[,] rv = new object[ntotal, 2 + 2 * nargs];

            k = nargs - 1;

            for ( i = 0 ; i < ntotal ; i++ ) {
                lRollersIncrement(nargs, rollers, ndims);
                rv[i, 0] = ntotal;
                rv[i, 1] = i;
                for ( k = 0 ; k < nargs ; k++ ) {
                    if ( ndims[k] == 1 )
                        rv[i, 2 + k] = args[k];
                    rv[i, 2 + k] = data[k][rollers[k]]; // (args[k] as object[])[rollers[k]];
                    rv[i, 2 + nargs + k] = rollers[k] + 1;
                }
            }

            return rv;
        }

        private static void _lDimensions_guts2(ref int ndim, ref int nelem, ref int nrows, ref int ncols, object[,] arg1)
        {
            try {
                ndim = arg1.Rank;
                if ( ndim == 1 ) {
                    nelem = arg1.GetLength(0);
                    nrows = nelem;
                    ncols = 1;
                }
                else if ( ndim == 2 ) {
                    nrows = arg1.GetLength(0);
                    ncols = arg1.GetLength(1);
                    nelem = nrows * ncols;
                }
            }
            catch {
                ndim = 0;
                nelem = 0;
                nrows = 0;
                ncols = 0;
            }
        }

        private static void lDimensions(ref int ndim, ref int nelem, ref int nrows, ref int ncols, object arg1)
        {
            try {
                if ( arg1 is object[] ) {
                    ndim = 1;
                    nelem = (arg1 as object[]).Length;
                    nrows = nelem;
                    ncols = 1;
                    return;
                }
                if ( arg1 is object[,] ) {
                    _lDimensions_guts2(ref ndim, ref nelem, ref nrows, ref ncols, arg1 as object[,]);
                    return;
                }
                if ( arg1 is ExcelMissing ) {
                    ndim = 0;
                    nelem = 0;
                    nrows = 0;
                    ncols = 0;
                    return;
                }
                ndim = -1;
                nelem = 1;
                nrows = 1;
                ncols = 1;
                return;
            }
            catch {
                ndim = 0;
                nelem = 0;
                nrows = 0;
                ncols = 0;
            }
        }

        private static void lRollersIncrement(int ndim, int[] rollers, int[] limits)
        {
            int k = ndim - 1;
            rollers[k] += 1;
            if ( rollers[k] >= limits[k] && ndim > 1 ) {
                rollers[k] = 0;
                lRollersIncrement(ndim - 1, rollers, limits);
            }
            return;
        }





    }
}
