using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dash.AutoMoq.Boost.Extensions
{
    internal static class MethodBaseExtensions
    {

        public static bool IsOverridable(this MethodBase method)
        {
            /*
             * From MSDN (http://goo.gl/WvOgYq)
             * 
             * To determine if a method is overridable, it is not sufficient to check that IsVirtual is true.
             * For a method to be overridable, IsVirtual must be true and IsFinal must be false.
             * 
             * For example, interface implementations are marked as "virtual final".
             * Methods marked with "override sealed" are also marked as "virtual final".
             */

            return method.IsVirtual && !method.IsFinal;
        }

        public static bool IsSealed(this MethodBase method)
        {
            return !method.IsOverridable();
        }
    }
}
