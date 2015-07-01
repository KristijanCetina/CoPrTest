using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoPrTest
{
    static class CoPrCommand
    {
        public static string softReset = "\x0004";
        public static string restoreDefaultMachineParametar = "\x0002109\x0003";
        public static string disableNumberPrintOfTheBarcode = "\x0002110B0\x0003";
        public static string selectLargeImage = "\x0002150010000000\x0003";
        public static string selectMediumImage = "\x0002149010000000\x0003";
        public static string defineBarcode = "\x000213801380070509809291034\x0003"; //"\x000213801380070509809291033\x0003"; (za karticu razduzivaca 9809291033)
        public static string defineBarcodeAsCM = "\x00021380150007050101161213122821029\x0003";
        public static string defineAString = ("\x000212246043032Kartica razduzivaca 1034 ");//("\x000212246043032ULAZ #666 "); //ovako ga nije volja, ok prvi nakon pokretanj app. btn5
        public static string defineAStringAsValidator = ("\x000212258043032ExitBarCode ");
        public static string movementWithPrint = "\x00021611\x0003";
        public static string movementWithPrintRead = "\x000216114\x0003";
        public static string movementWithPrintFromMouth = "\x00021610\x0003";
        public static string CaptureTicket = "\x00021602\x0003";
        public static string ejectTicket = "\x00021601\x0003";
        public static string captureTicketFromMouth = "\x00021602\x0003";
        public static string readBarcodePrintedByBCTIM = "\x00021605\x0003";
        public static string readBarcodePrintedByBCTCM = "\x00021605C\x0003";
    }
}
