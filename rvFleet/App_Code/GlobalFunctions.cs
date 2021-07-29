using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rvFleet.App_Code
{
    public static class GlobalFunctions
    {
        public static string FormatThousandsAndMillions(double value)
        {
            if (value >= 100000000)
                return (value / 1000000).ToString("#,0M");

            if (value >= 10000000)
                return (value / 1000000).ToString("0.#") + "M";

            if (value >= 100000)
                return (value / 1000).ToString("#,0K");

            if (value >= 1000)
                return (value / 1000).ToString("0.#") + "K";

            return value.ToString("#,0");
        }
    }
}