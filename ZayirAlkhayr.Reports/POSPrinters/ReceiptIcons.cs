using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZayirAlkhayr.Reports.POSPrinters
{
    public static class ReceiptIcons
    {
        public const string Phone =
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <path fill="#000000"
                d="M6.62 10.79a15.46 15.46 0 0 0 6.59 6.59l2.2-2.2
                   a1 1 0 0 1 1.02-.24c1.12.37 2.33.57 3.57.57
                   a1 1 0 0 1 1 1V20a1 1 0 0 1-1 1
                   C10.61 21 3 13.39 3 4a1 1 0 0 1 1-1h3.5
                   a1 1 0 0 1 1 1c0 1.25.2 2.45.57 3.57
                   a1 1 0 0 1-.25 1.02z"/>
        </svg>
        """;

        public const string Location =
            """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <path fill="#000000"
                d="M12 2a7 7 0 0 0-7 7c0 5.25 7 13 7 13s7-7.75 7-13
                   a7 7 0 0 0-7-7zm0 9.5A2.5 2.5 0 1 1 12 6
                   a2.5 2.5 0 0 1 0 5.5z"/>
        </svg>
        """;

        public const string Globe =
            """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <circle cx="12" cy="12" r="9"
                  fill="none" stroke="#000000" stroke-width="2"/>
          <path d="M3 12h18M12 3c3 3 3 15 0 18M12 3c-3 3-3 15 0 18"
                fill="none" stroke="#000000" stroke-width="1.6"/>
        </svg>
        """;

        public const string User =
            """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <circle cx="12" cy="7" r="4" fill="#000000"/>
          <path d="M4 22c0-5 3-8 8-8s8 3 8 8z" fill="#000000"/>
        </svg>
        """;

        public const string Users =
            """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <circle cx="9" cy="7" r="3.5" fill="#000000"/>
          <circle cx="17" cy="9" r="3" fill="#000000"/>
          <path d="M2 21c0-5 2.5-8 7-8s7 3 7 8z" fill="#000000"/>
          <path d="M14 21c.2-3-1-5.5-3-7 1.2-.8 2.8-1.2 4.5-1.2
                   4 0 6.5 2.8 6.5 8.2z" fill="#000000"/>
        </svg>
        """;
    }
}
