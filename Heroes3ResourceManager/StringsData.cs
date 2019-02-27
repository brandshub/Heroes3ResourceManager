using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace h3magic
{
    public static class StringsData
    {
        public const string FNAME_JKTEXT = "JKTEXT.TXT";

        public static string[] JKTEXT;

        public static void LoadInfo(Heroes3Master master)
        {
            Unload();
            var lodFile = master.Resolve(FNAME_JKTEXT);
            var rec = lodFile.GetRecord(FNAME_JKTEXT);
            JKTEXT = Encoding.Default.GetString(rec.GetRawData(lodFile.stream)).Split(new string[] { "\r\n" }, StringSplitOptions.None);
        }

        public static void Unload()
        {
            JKTEXT = null;
        }

    }
}
